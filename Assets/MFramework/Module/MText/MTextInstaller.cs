using MFramework.Core.CoreEx;

namespace MFramework.Text
{
    public sealed class MTextInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            var manager = new MLocalizationManager();
            context.Container.RegisterSingleton<MLocalizationManager>(manager);
            MLocalizationManager.SetActive(manager);
        }

        public void Uninstall(IModuleContext context)
        {
            MLocalizationManager.SetActive(null);
            context.Container.UnRegister<MLocalizationManager>();
        }
    }
}
