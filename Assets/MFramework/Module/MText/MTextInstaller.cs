using MFramework.Core.CoreEx;

namespace MFramework.Text
{
    public sealed class MTextInstaller : IModuleInstaller
    {
        public void Install(IModuleContext context)
        {
            var manager = new MLocalizationManager();
            manager.LoadGeneratedTextTable();
            context.Container.RegisterSingleton<MLocalizationManager>(manager);
        }

        public void Uninstall(IModuleContext context)
        {
            context.Container.UnRegister<MLocalizationManager>();
        }
    }
}
