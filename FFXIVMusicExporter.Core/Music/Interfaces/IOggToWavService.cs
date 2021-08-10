using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMusicExporter.Core.Music
{
    public interface IOggToWavService
    {
        Task ConvertToWavAsync(IEnumerable<string> oggFiles, CancellationToken cancellationToken);
    }
}