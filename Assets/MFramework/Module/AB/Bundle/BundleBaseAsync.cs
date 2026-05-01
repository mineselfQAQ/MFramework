namespace MFramework
{
    public abstract class BundleBaseAsync : BundleBase
    {
        protected BundleBaseAsync(MBundleManager bundleManager, ABRuntimeState runtimeState)
            : base(bundleManager, runtimeState)
        {
        }

        internal abstract bool Update();
    }
}
