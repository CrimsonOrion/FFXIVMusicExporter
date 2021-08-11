﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core;

using MahApps.Metro.Controls.Dialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace FFXIVMusicExporter.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly IRealm _realm;
    private readonly IDialogCoordinator _dialogCoordinator;

    public string Title => "FFXIV Music Exporter";

    public DelegateCommand LaunchGitHubSiteCommand => new(LaunchGitHubSite);

    public MainWindowViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator, IRealm realm)
    {
        _regionManager = regionManager;
        _realm = realm;
        _dialogCoordinator = dialogCoordinator;
    }

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

    private void Navigate(string navigationRegion, string navigationView, NavigationParameters navigationParameters) => _regionManager.RequestNavigate(navigationRegion, navigationView, navigationParameters);
    private void Navigate(string navigationRegion, string navigationView) => _regionManager.RequestNavigate(navigationRegion, navigationView);
}
