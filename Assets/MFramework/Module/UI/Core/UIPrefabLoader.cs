using UnityEngine;

namespace MFramework.UI
{
    public interface IUIPrefabLoader
    {
        GameObject Load(string prefabPath);
    }

    public sealed class ResourcesUIPrefabLoader : IUIPrefabLoader
    {
        public GameObject Load(string prefabPath)
        {
            return Resources.Load<GameObject>(prefabPath);
        }
    }
}
