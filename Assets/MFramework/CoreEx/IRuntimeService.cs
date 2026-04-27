namespace MFramework.Core.CoreEx
{
    /// <summary>
    /// Owns runtime lifecycle only.
    /// </summary>
    public interface IRuntimeService
    {
        void Initialize();

        void Shutdown();
    }

    public interface IRuntimeUpdateService
    {
        void Update();
    }

    public interface IRuntimeFixedUpdateService
    {
        void FixedUpdate();
    }

    public interface IRuntimeLateUpdateService
    {
        void LateUpdate();
    }

    public interface IRuntimeApplicationFocusService
    {
        void OnApplicationFocus(bool hasfocus);
    }

    public interface IRuntimeApplicationPauseService
    {
        void OnApplicationPause(bool pauseStatus);
    }

    public interface IRuntimeServiceWithContext
    {
        void BindContext(IRuntimeServiceContext context);
    }
}
