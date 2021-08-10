using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SaintCoinach;

namespace FFXIVMusicExporter.Core
{
    public class Realm : IRealm
    {
        private readonly string _gamePath = Path.Combine(@"J:\", "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn");
        public ARealmReversed RealmReversed { get; }

        public Realm() => RealmReversed = new(_gamePath, SaintCoinach.Ex.Language.English);

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