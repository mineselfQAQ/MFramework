namespace MFramework.Core
{
    public interface IServiceProvider
    {
        /// <summary>
        /// 注册，在函数中应该进行IOC注册操作
        /// </summary>
        void Register();

        /// <summary>
        /// 注册后所需进行的初始化
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 反注册，与注册对应
        /// </summary>
        void Unregister();
        
        /// <summary>
        /// 关闭，与初始化对应
        /// </summary>
        void Shutdown();
    }
}