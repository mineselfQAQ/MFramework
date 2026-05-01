using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// 自动卸载AB脚本
    /// </summary>
    public class AutoUnloadAB : MonoBehaviour
    {
        public IResource resource { get; set; }
        public MResourceManager resourceManager { get; set; }

        private void OnDestroy()
        {
            if (resource == null) return;

            resourceManager?.Unload(resource); // 卸载
            resource = null;
            resourceManager = null;
        }
    }
}
