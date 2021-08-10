using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMusicExporter.Core.Music
{
    public interface IWavToMP3Service
    {
        Task ConvertToMP3Async(IEnumerable<string> wavFiles, CancellationToken cancellationToken);
        void MP3ToWave(string mp3FileName, string waveFileName);
        void WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 192);
    }
}