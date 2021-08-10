using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SaintCoinach.Sound;

using SaintCoinach.Xiv;

namespace FFXIVMusicExporter.Core.Music
{
    public class RipBGMService : IRipBGMService
    {
        private readonly IRealm _realm;

        public RipBGMService(IRealm realm) => _realm = realm;

        public async Task GetFilesAsync(CancellationToken cancellationToken)
        {
            IXivSheet<XivRow>? files = _realm.RealmReversed.GameData.GetSheet("BGM");
            int success = 0, fail = 0;

            foreach (IXivRow file in files)
            {
                var path = file["File"].ToString();

                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        if (await Task.Run(() => ExportFile(path, null)))
                        {
                            var successMessage = $"{path} exported.";
                            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(successMessage));
                            //_logger.LogInformation(successMessage);
                            success++;
                        }
                        else
                        {
                            var notFoundMessage = $"File {path} not found.";
                            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(notFoundMessage));
                            //_logger.LogInformation(notFoundMessage);
                            fail++;
                        }
                    }
                    catch (Exception)
                    {
                        var errorMessage = $"Could not export {path}.";
                        //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(errorMessage));
                        //_logger.LogError(ex, errorMessage);
                        fail++;
                    }
                }
            }

            IXivSheet<XivRow>? orch = _realm.RealmReversed.GameData.GetSheet("Orchestrion");
            IXivSheet<XivRow>? orchPath = _realm.RealmReversed.GameData.GetSheet("OrchestrionPath");

            foreach (IXivRow orchInfo in orch)
            {
                XivRow? path = orchPath[orchInfo.Key];
                var name = orchInfo["Name"].ToString();
                var filePath = path["File"].ToString();

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    continue;
                }
                else
                {
                    try
                    {
                        if (await Task.Run(() => ExportFile(filePath, name)))
                        {
                            var successMessage = $"{filePath}-{name} exported.";
                            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(successMessage));
                            //_logger.LogInformation(successMessage);
                            success++;
                        }
                        else
                        {
                            var notFoundMessage = $"File {filePath}-{name} not found.";
                            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(notFoundMessage));
                            //_logger.LogInformation(notFoundMessage);
                            fail++;
                        }
                    }
                    catch (Exception)
                    {
                        var errorMessage = $"Could not export {filePath}-{name}.";
                        //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(errorMessage));
                        //_logger.LogError(ex, errorMessage);
                        fail++;
                    }
                }
            }

            var message = $"{success} files exported. {fail} files failed.";
            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            //_logger.LogInformation(message);
        }

        private bool ExportFile(string filePath, string suffix)
        {
            var result = false;
            if (_realm.RealmReversed.Packs.TryGetFile(filePath, out var file))
            {
                var scdFile = new ScdFile(file);
                var count = 0;

                for (var i = 0; i < scdFile.ScdHeader.EntryCount; ++i)
                {
                    ScdEntry? entry = scdFile.Entries[i];

                    if (entry == null)
                    {
                        continue;
                    }

                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);

                    if (suffix != null)
                    {
                        fileNameWithoutExt += $"-{suffix}";
                    }

                    if (++count > 1)
                    {
                        fileNameWithoutExt += $"-{count}";
                    }

                    foreach (var invalidChar in Path.GetInvalidFileNameChars())
                    {
                        fileNameWithoutExt = fileNameWithoutExt.Replace(invalidChar.ToString(), "");
                    }

                    var extension = entry.Header.Codec switch
                    {
                        ScdCodec.MSADPCM => ".wav",
                        ScdCodec.OGG => ".ogg",
                        _ => throw new NotSupportedException()
                    };

                    var fileInfo = new FileInfo(Path.Combine(_realm.RealmReversed.GameVersion, Path.GetDirectoryName(filePath), extension.ToUpper().Replace(".", ""), fileNameWithoutExt + extension));

                    if (!fileInfo.Directory.Exists)
                    {
                        fileInfo.Directory.Create();
                    }

                    if (fileInfo.Exists)
                    {
                        break;
                    }

                    File.WriteAllBytes(fileInfo.FullName, entry.GetDecoded());
                }

                result = true;
            }

            return result;
        }
    }
}