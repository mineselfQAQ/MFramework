namespace MFramework
{
    //TODO:写完后改为BundleType和ResourceType
    public enum EBundleType
    {
        /// <summary>
        /// 以文件作为ab名字（最小粒度）
        /// </summary>
        File,

        /// <summary>
        /// 以目录作为ab的名字
        /// </summary>
        Directory,

        /// <summary>
        /// 以最上的
        /// </summary>
        All
    }
    public enum EResourceType
    {
        /// <summary>
        /// 在打包设置中分析到的资源
        /// </summary>
        Direct = 1,

        /// <summary>
        /// 依赖资源
        /// </summary>
        Dependency = 2,

        /// <summary>
        /// 生成的文件
        /// </summary>
        Ganerate = 3,
    }
}