using FFXIVMusicExporter.Core;

using Prism.Commands;
using Prism.Mvvm;

namespace UpdateRealm.Module.ViewModels
{
    public class UpdateRealmViewModel : BindableBase
    {
        private readonly IRealm _realm;

        private string _updateText = string.Empty;
        public string UpdateText
        {
            get => _updateText;
            set => SetProperty(ref _updateText, value);
        }

        public DelegateCommand UpdateRealmCommand => new(UpdateRealm);

        public UpdateRealmViewModel(IRealm realm) => _realm = realm;

        private async void UpdateRealm()
        {
            SaintCoinach.UpdateReport? updateReport = await _realm.UpdateAsync(new System.Threading.CancellationToken());

            if (updateReport is not null)
            {
                foreach (SaintCoinach.Ex.Relational.Update.IChange? change in updateReport.Changes)
                {
                    UpdateText += $"{change}\r\n\r\n";
                }
            }
            else
            {
                UpdateText = "Running Current Version";
            }
        }
    }
}