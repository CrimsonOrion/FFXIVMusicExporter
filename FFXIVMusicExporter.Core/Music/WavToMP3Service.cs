using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using NAudio.Lame;

using NAudio.Wave;

using WavFile;

namespace FFXIVMusicExporter.Core.Music
{
    public class WavToMP3Service : IWavToMP3Service
    {
        //private readonly ICustomLogger _logger;
        //private readonly ISendMessageEvent _sendMessageEvent;

        public WavToMP3Service()//ICustomLogger logger, ISendMessageEvent sendMessageEvent)
        {
            //_logger = logger;
            //_sendMessageEvent = sendMessageEvent;
        }

        public void WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 192)
        {
            using var reader = new AudioFileReader(waveFileName);
            using var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate);
            reader.CopyTo(writer);
            var message = $"{mp3FileName} created.";
            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            //_logger.LogInformation(message);
        }

        public void MP3ToWave(string mp3FileName, string waveFileName)
        {
            using var reader = new Mp3FileReader(mp3FileName);
            using var writer = new WaveFileWriter(waveFileName, reader.WaveFormat);
            reader.CopyTo(writer);
            var message = $"{waveFileName} created.";
            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            //_logger.LogInformation(message);
        }

        public async Task ConvertToMP3Async(IEnumerable<string> wavFiles, CancellationToken cancellationToken)
        {
            int processed = 0, skipped = 0, failed = 0;
            foreach (var file in wavFiles)
            {
                string album, year;

                var folder = Path.GetDirectoryName(file);
                DirectoryInfo? parentFolder = Directory.GetParent(folder);
                var mp3Folder = Path.Combine(parentFolder.FullName, "MP3");

                if (!Directory.Exists(mp3Folder))
                {
                    Directory.CreateDirectory(mp3Folder);
                }

                var mp3File = Path.Combine(mp3Folder, Path.GetFileName(file).Replace(".wav", ".mp3"));

                var fileExists = File.Exists(mp3File);
                var fileExistsMultiChannel = File.Exists(mp3File.Replace(".mp3", ".Battle.mp3")) && File.Exists(mp3File.Replace(".mp3", ".Dungeon.mp3"));
                var fileChannelSplit = mp3File.Contains("CH0");

                if (fileExists || fileExistsMultiChannel || fileChannelSplit)
                {
                    var skipMessage = await Task.Run(() => fileExists ? $"{mp3File} exists. Skipping." : fileExistsMultiChannel ? $"{mp3File}.Battle & {mp3File}.Dungeon exists. Skipping." : $"{mp3File} is channel split. Skipping.");
                    //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(skipMessage));
                    //_logger.LogInformation(skipMessage);
                    skipped++;
                    continue;
                }

                if (file.Contains("_EX4_")) { album = "FFXIV:EW DAT Rip"; year = "2021"; }
                else if (file.Contains("_EX3_")) { album = "FFXIV:ShB DAT Rip"; year = "2019"; }
                else if (file.Contains("_EX2_")) { album = "FFXIV:SB DAT Rip"; year = "2017"; }
                else if (file.Contains("_EX1_")) { album = "FFXIV:HW DAT Rip"; year = "2015"; }
                else if (file.Contains("_ORCH_")) { album = "FFXIV:ORCH DAT Rip"; year = "2021"; }
                else { album = "FFXIV:ARR DAT Rip"; year = "2013"; }

                try
                {
                    await ConvertAsync(file, mp3File, albumArtist: "Square Enix", album: album, year: year);
                    processed++;
                }
                catch (Exception)
                {
                    var errorMessage = $"Unable to convert {mp3File}";
                    //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(errorMessage));
                    //_logger.LogError(ex, errorMessage);
                    failed++;
                }
            }
            var message = $"Completed MP3 Conversion. {processed} converted. {skipped} skipped. {failed} failed.";
            //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
            //_logger.LogInformation(message);
        }

        private async Task ConvertAsync(
            string waveFileName,
            string mp3FileName,
            int bitRate = 192,
            string title = "",
            string subtitle = "",
            string comment = "",
            string artist = "",
            string albumArtist = "",
            string album = "",
            string year = "",
            string track = "",
            string genre = "",
            byte[] albumArt = null)
        {
            var tag = new ID3TagData
            {
                Title = title,
                Artist = artist,
                Album = album,
                Year = year,
                Comment = comment,
                Genre = genre.Length == 0 ? LameMP3FileWriter.Genres[36] : genre, // 36 is game.  Full list @ http://ecmc.rochester.edu/ecmc/docs/lame/id3.html
                Subtitle = subtitle,
                AlbumArt = albumArt,
                AlbumArtist = albumArtist,
                Track = track
            };
            var reader = new AudioFileReader(waveFileName);
            if (reader.WaveFormat.Channels <= 2)
            {
                using (reader)
                using (var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate, tag))
                {
                    await reader.CopyToAsync(writer);
                }
                var createMessage = $"{mp3FileName} created";
                //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(createMessage));
                //_logger.LogInformation(createMessage);
            }
            else if (reader.WaveFormat.Channels == 4 || reader.WaveFormat.Channels == 6)
            {
                reader.Dispose();
                mp3FileName = string.Empty;
                await Task.Run(() => SplitWav(waveFileName));
                var fileNames = MixSixChannel(waveFileName);
                foreach (var fileName in fileNames)
                {
                    using (reader = new AudioFileReader(fileName))
                    {
                        using (var writer = new LameMP3FileWriter(fileName.Replace(".wav", ".mp3"), reader.WaveFormat, bitRate: bitRate, id3: tag))
                        {
                            await reader.CopyToAsync(writer);
                        }
                        mp3FileName += string.IsNullOrEmpty(mp3FileName) ? fileName.Replace(".wav", ".mp3") + " & " : fileName.Replace(".wav", ".mp3");
                    }
                }
                var createMessage = $"{mp3FileName} created";
                //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(createMessage));
                //_logger.LogInformation(createMessage);
            }
            else
            {
                throw new Exception($"Could not convert {mp3FileName.Trim()}: It has {reader.WaveFormat.Channels} channels.");
            }
        }

        private string[] MixSixChannel(string fileName)
        {
            fileName = fileName.Replace(".wav", "");
            var fileNames = new string[2];

            using (var input1 = new WaveFileReader($"{fileName}.CH01.wav"))
            {
                using var input2 = new WaveFileReader($"{fileName}.CH02.wav");
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1, input2 }, 2);
                waveProvider.ConnectInputToOutput(0, 0);
                waveProvider.ConnectInputToOutput(1, 1);
                WaveFileWriter.CreateWaveFile($"{fileName}.Dungeon.wav", waveProvider);
                fileNames[0] = $"{fileName}.Dungeon.wav";
            }

            using (var input1 = new WaveFileReader($"{fileName}.CH03.wav"))
            {
                using var input2 = new WaveFileReader($"{fileName}.CH04.wav");
                var waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1, input2 }, 2);
                waveProvider.ConnectInputToOutput(0, 0);
                waveProvider.ConnectInputToOutput(1, 1);
                WaveFileWriter.CreateWaveFile($"{fileName}.Battle.wav", waveProvider);
                fileNames[1] = $"{fileName}.Battle.wav";
            }

            return fileNames;

            /* TO PLAY IT OUTRIGHT:
            WasapiOut outDevice = new WasapiOut();
            outDevice.Init(waveProvider);
            outDevice.Play(); */
        }

        private void SplitWav(string fileName)
        {
            var outputPath = fileName.Remove(fileName.Length - Path.GetFileName(fileName).Length);

            try
            {
                long bytesTotal = 0;
                var splitter = new WavFileSplitter(value => string.Format("Progress: {0:0.0}%", value));//value => _logger.LogInformation(string.Format("Progress: {0:0.0}%", value)));
                var sw = Stopwatch.StartNew();
                bytesTotal = splitter.SplitWavFile(fileName, outputPath, CancellationToken.None);
                sw.Stop();
                var message = $"Data bytes processed: {bytesTotal} ({Math.Round((double)bytesTotal / (1024 * 1024), 1)} MB)\r\nElapsed time: {sw.Elapsed}";
                //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(message));
                //_logger.LogInformation(message);
            }
            catch (Exception)
            {
                var errorMessage = $"Problem splitting {fileName}.";
                //_sendMessageEvent.OnSendMessageEvent(new SendMessageEventArgs(errorMessage));
                //_logger.LogError(ex, errorMessage);
            }
        }
    }
}