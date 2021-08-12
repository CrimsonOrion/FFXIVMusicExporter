using System.Threading;
using System.Threading.Tasks;

using SaintCoinach;

namespace FFXIVMusicExporter.Core
{
    public interface IRealmService
    {
        ARealmReversed RealmReversed { get; }

        Task UpdateAsync(CancellationToken cancellationToken);
    }
}