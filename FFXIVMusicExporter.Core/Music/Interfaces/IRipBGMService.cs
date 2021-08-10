using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMusicExporter.Core.Music
{
    public interface IRipBGMService
    {
        Task GetFilesAsync(CancellationToken cancellationToken);
    }
}