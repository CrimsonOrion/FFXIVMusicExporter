using System.Collections.ObjectModel;
using System.Linq;

using FFXIVMusicExporter.Core;
using FFXIVMusicExporter.Core.Events;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace UpdateRealm.Module.ViewModels;

public class UpdateRealmViewModel : BindableBase
{
    private ObservableCollection<string> _updateText = new();
    private readonly IRealmService _realm;
    private readonly IEventAggregator _eventAggregator;

    public ObservableCollection<string> UpdateText
    {
        get => _updateText;
        set => SetProperty(ref _updateText, value);
    }

    public DelegateCommand UpdateRealmCommand => new(UpdateRealm);

    public UpdateRealmViewModel(IEventAggregator eventAggregator, IRealmService realm)
    {
        _realm = realm;
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<UpdateRealmEvent>().Subscribe(PublishMessage);
    }

    private async void UpdateRealm()
    {
        _eventAggregator.GetEvent<UpdateRealmEvent>().Publish("Test");
        //await _realm.UpdateAsync(new System.Threading.CancellationToken());

        //if (updateReport is not null)
        //{
        //    foreach (SaintCoinach.Ex.Relational.Update.IChange? change in updateReport.Changes)
        //    {
        //        var updateMessage
        //        UpdateText.Add(change);
        //    }
        //}
        //else
        //{
        //    UpdateText.Add("Running Current Version");
        //}
    }

    private void PublishMessage(string message) => UpdateText.Add(message);
}
