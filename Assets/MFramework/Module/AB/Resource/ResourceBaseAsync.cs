namespace MFramework
{

    public abstract class ResourceBaseAsync : ResourceBase
    {
        protected ResourceBaseAsync(MResourceManager resourceManager, MBundleManager bundleManager)
            : base(resourceManager, bundleManager)
        {
        }

        public abstract bool Update();

        internal abstract void LoadAssetAsync();
    }
}
