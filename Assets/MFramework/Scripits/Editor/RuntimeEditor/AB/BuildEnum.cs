namespace MFramework
{
    public enum BundleType
    {
        File,//文件级别
        Directory,//文件夹级别
        All//都包含
    }

    public enum ResourceType
    {
        Direct = 1,//直接资源文件
        Dependency = 2//依赖资源文件
    }
}