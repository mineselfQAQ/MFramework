using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// ×Ô¶¯Ð¶ÔØAB½Å±¾
    /// </summary>
    public class AutoUnloadAB : MonoBehaviour
    {
        public IResource resource { get; set; }

        private void OnDestroy()
        {
            if (resource == null) return;

            ResourceManager.Instance.Unload(resource);//Ð¶ÔØ
            resource = null;
        }
    }
}