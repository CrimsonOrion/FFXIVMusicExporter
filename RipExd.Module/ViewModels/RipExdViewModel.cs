using System;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core;
using FFXIVMusicExporter.Core.Music;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;

namespace RipExd.Module.ViewModels
{
    public class RipExdViewModel : BindableBase
    {
        private readonly IRealm _realm;
        IRipBGMService _ripBGMService;

        private string _updateText = string.Empty;
        private IDialogCoordinator _dialogCoordinator;

        public string UpdateText
        {
            get => _updateText;
            set => SetProperty(ref _updateText, value);
        }

        public DelegateCommand RipExdCommand => new(RipExd);

        public RipExdViewModel(IRealm realm, IDialogCoordinator dialogCoordinator, IRipBGMService ripBGMService)
        {
            _realm = realm;
            _dialogCoordinator = dialogCoordinator;
            _ripBGMService = ripBGMService;
        }

        private async void RipExd()
        {
            UpdateText = "Ripping EXD files...";
            ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, "Opening", "Opening Browser");

            try
            {
                controller.SetIndeterminate();
                await _ripBGMService.GetFilesAsync(new System.Threading.CancellationToken());
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
            }
            await controller.CloseAsync();
        }
    }
}