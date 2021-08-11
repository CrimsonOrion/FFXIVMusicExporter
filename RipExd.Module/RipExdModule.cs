using FFXIVMusicExporter.Core;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using RipExd.Module.Views;

namespace RipExd.Module;

public class RipExdModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) => containerProvider
            .Resolve<IRegionManager>()
            .RegisterViewWithRegion(KnownRegionNames.RipExdRegionName, typeof(RipExdView));

    public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterForNavigation<RipExdView>();
}
