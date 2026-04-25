namespace MFramework.Core.CoreEx
{
    /// <summary>
    /// xxxModule：模块（更高层的组合）
    /// xxxServiceProvider：DI注册（核心为注册/反注册，可进行初始化/关闭）
    /// </summary>
    public interface IManagedService
    {
        /// <summary>
        /// 注册，在函数中应该进行IOC注册操作
        /// </summary>
        void Register();

        /// <summary>
        /// 初始化（可能是注册后操作，也可能是模块所需）
        /// </summary>
        void Initialize();

        /// <summary>
        /// 关闭，与初始化对应
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 反注册，与注册对应
        /// </summary>
        void Unregister();

    }
}
