using FFXIVMusicExporter.Core;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using UpdateRealm.Module.Views;

namespace UpdateRealm.Module;

public class UpdateRealmModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) => containerProvider
            .Resolve<IRegionManager>()
            .RegisterViewWithRegion(KnownRegionNames.UpdateRealmRegionName, typeof(UpdateRealmView));

    public void RegisterTypes(IContainerRegistry containerRegistry) => containerRegistry.RegisterForNavigation<UpdateRealmView>();
}
