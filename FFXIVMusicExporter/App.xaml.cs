using ControlzEx.Theming;

using FFXIVMusicExporter.Views;

using MahApps.Metro.Controls.Dialogs;

using Prism.DryIoc;
using Prism.Ioc;

using Prism.Modularity;

using System.Windows;

namespace FFXIVMusicExporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry
                .RegisterInstance<IDialogCoordinator>(new DialogCoordinator())
;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

        }
    }
}