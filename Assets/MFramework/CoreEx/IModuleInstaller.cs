namespace MFramework.Core.CoreEx
{
    /// <summary>
    /// Owns module dependency registration and unregistration.
    /// </summary>
    public interface IModuleInstaller
    {
        void Install();

        void Uninstall();
    }
}
