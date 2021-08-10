using System;
using System.Diagnostics;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;

namespace FFXIVMusicExporter.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogCoordinator _dialogCoordinator;

        public string Title => "FFXIV Music Exporter";

        public IRealm Realm { get; set; }

        public DelegateCommand LaunchGitHubSiteCommand => new(LaunchGitHubSite);
        public DelegateCommand UpdateRealmCommand => new(UpdateRealm);

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            Realm = new Realm();
            _dialogCoordinator = dialogCoordinator;
        }

        private async void UpdateRealm() => await Realm.UpdateAsync(default);

        private async void LaunchGitHubSite()
        {
            var uri = "https://github.com/CrimsonOrion/FFXIVMusicExporter";
            ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, "Opening", "Opening Browser");

            try
            {
                controller.SetIndeterminate();
                await Task.Delay(2000);

                ProcessStartInfo info = new() { FileName = uri, UseShellExecute = true };

                Process.Start(info);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    // need a width of 700 to see all 4 selections, 535 to see 3, 355 to see 2.
                    MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, "No Default Browser Set", noBrowser.Message, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                        new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No", FirstAuxiliaryButtonText = "Maybe", SecondAuxiliaryButtonText = "So" });

                }
            }
            catch (Exception other)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", other.Message);
            }
            await controller.CloseAsync();
        }
    }
}