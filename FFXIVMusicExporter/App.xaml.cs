using System.IO;
using System.Windows;

using ControlzEx.Theming;

using FFXIVMusicExporter.Core;
using FFXIVMusicExporter.Core.Music;
using FFXIVMusicExporter.Views;

using MahApps.Metro.Controls.Dialogs;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

using RipExd.Module;

using RipMusic.Module;

using SaintCoinach;

using UpdateRealm.Module;

namespace FFXIVMusicExporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private readonly string _gamePath = Path.Combine(@"E:\", "SquareEnix", "FINAL FANTASY XIV - A Realm Reborn");
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ARealmReversed _realm = new(_gamePath, SaintCoinach.Ex.Language.English);
            //_realm.DefinitionVersion = "2021.07.15.0000.0000";
            containerRegistry
                .RegisterInstance<IDialogCoordinator>(new DialogCoordinator())
                .RegisterInstance(_realm)
                .RegisterSingleton<IRealm, Realm>()
                
                .Register<IRipBGMService, RipBGMService>()
                .Register<IOggToWavService, OggToWavService>()
                .Register<IWavToMP3Service, WavToMP3Service>()
                ;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog
                .AddModule<UpdateRealmModule>()
                .AddModule<RipMusicModule>()
                .AddModule<RipExdModule>();
        }
    }
}