using FFXIVMusicExporter.Core;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using RipMusic.Module.Views;

namespace RipMusic.Module;

public class RipMusicModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) => containerProvider
            .Resolve<IRegionManager>()
            .RegisterViewWithRegion(KnownRegionNames.RipMusicRegionName, typeof(RipMusicView));

    public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterForNavigation<RipMusicView>();
}
