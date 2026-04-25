namespace MFramework.Core.CoreEx
{
    public interface IManagedUpdateService
    {
        void Update();
    }

    public interface IManagedFixedUpdateService
    {
        void FixedUpdate();
    }

    public interface IManagedLateUpdateService
    {
        void LateUpdate();
    }

    public interface IManagedApplicationFocusService
    {
        void OnApplicationFocus(bool hasfocus);
    }

    public interface IManagedApplicationPauseService
    {
        void OnApplicationPause(bool pauseStatus);
    }

    public interface IManagedServiceWithContext
    {
        void BindContext(IManagedServiceContext context);
    }
}
