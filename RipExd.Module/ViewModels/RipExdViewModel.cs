using System;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core;
using FFXIVMusicExporter.Core.Music;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;

namespace RipExd.Module.ViewModels;

public class RipExdViewModel : BindableBase
{
    private string _updateText = string.Empty;
    private IDialogCoordinator _dialogCoordinator;

    public string UpdateText
    {
        get => _updateText;
        set => SetProperty(ref _updateText, value);
    }

    public DelegateCommand RipExdCommand => new(RipExd);

    public RipExdViewModel(IRealmService realm, IDialogCoordinator dialogCoordinator, IRipBGMService ripBGMService)
    {
        _dialogCoordinator = dialogCoordinator;
    }

    private async void RipExd()
    {
        UpdateText = "Ripping EXD files...";
        ProgressDialogController controller = await _dialogCoordinator.ShowProgressAsync(this, "Opening", UpdateText);

        try
        {
            controller.SetIndeterminate();
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, "Error", ex.Message);
        }
        await controller.CloseAsync();
    }
}
