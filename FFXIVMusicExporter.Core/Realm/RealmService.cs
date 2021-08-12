using System;
using System.Threading;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core.Events;

using Prism.Events;

using SaintCoinach;
using SaintCoinach.Ex.Relational.Update;

namespace FFXIVMusicExporter.Core
{
    /// <summary>
    /// Base Class for FFXIV Data Manipulation
    /// </summary>
    public class RealmService : IRealmService
    {
        public ARealmReversed RealmReversed { get; }

        private readonly IEventAggregator _eventAggregator;

        public RealmService(ARealmReversed realm, IEventAggregator eventAggregator)
        {
            RealmReversed = realm;
            _eventAggregator = eventAggregator;
        }

        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            var gameVersion = RealmReversed.GameVersion;
            var definitionVersion = RealmReversed.DefinitionVersion;

            if (!RealmReversed.IsCurrentVersion)
            {
                const bool includeDataChanges = true;
                var progress = new UpdateProgression(_eventAggregator);

                UpdateReport? updateReport = await Task.Run(() => RealmReversed.Update(includeDataChanges, progress));

                foreach (IChange? update in updateReport.Changes)
                {
                    _eventAggregator.GetEvent<UpdateRealmEvent>().Publish(update.ToString());
                }
            }
            else
            {
                _eventAggregator.GetEvent<UpdateRealmEvent>().Publish("Running Current Version");
            }
        }
    }

    internal class UpdateProgression : IProgress<UpdateProgress>
    {
        private readonly IEventAggregator _eventAggregator;
        public UpdateProgression(IEventAggregator eventAggregator) => _eventAggregator = eventAggregator;
        public void Report(UpdateProgress value) => _eventAggregator.GetEvent<UpdateRealmEvent>().Publish(value.Percentage.ToString() + "%");
    }
}