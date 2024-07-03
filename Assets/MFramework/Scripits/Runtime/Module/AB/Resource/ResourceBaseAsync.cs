namespace MFramework
{

    public abstract class ResourceBaseAsync : ResourceBase
    {
        public abstract bool Update();

        internal abstract void LoadAssetAsync();
    }
}