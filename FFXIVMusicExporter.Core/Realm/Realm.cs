using System.Threading;
using System.Threading.Tasks;

using SaintCoinach;

namespace FFXIVMusicExporter.Core
{
    /// <summary>
    /// Base Class for FFXIV Data Manipulation
    /// </summary>
    public class Realm : IRealm
    {

        public ARealmReversed RealmReversed { get; }

        public Realm(ARealmReversed realm) => RealmReversed = realm;

        public async Task<UpdateReport?> UpdateAsync(CancellationToken cancellationToken)
        {
            var gameVersion = RealmReversed.GameVersion;
            var definitionVersion = RealmReversed.DefinitionVersion;

            if (!RealmReversed.IsCurrentVersion)
            {
                const bool includeDataChanges = true;
                UpdateReport? updateReport = await Task.Run(() => RealmReversed.Update(includeDataChanges));
                return updateReport;
            }
            else
            {
                return null;
            }
        }
    }
}