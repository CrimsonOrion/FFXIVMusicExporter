using System.Threading;
using System.Threading.Tasks;

using SaintCoinach;

namespace FFXIVMusicExporter.Core
{
    public interface IRealm
    {
        ARealmReversed RealmReversed { get; }

        Task<UpdateReport?> UpdateAsync(CancellationToken cancellationToken);
    }
}