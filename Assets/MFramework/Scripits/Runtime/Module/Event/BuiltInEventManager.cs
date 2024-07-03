using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#BuiltInEventManager#")]
    public class BuiltInEventManager : MonoSingleton<BuiltInEventManager>
    {
        private void Awake()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.AWAKE);
        }

        private void Start()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.START);
        }

        private void FixedUpdate()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.FIXEDUPDATE);
        }

        private void Update()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.UPDATE);
        }

        private void LateUpdate()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.LATEUPDATE);
        }

        private void OnApplicationFocus(bool focus)
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONFOCUS, focus);
        }

        private void OnApplicationPause(bool pause)
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONPAUSE, pause);
        }

        private void OnApplicationQuit()
        {
            MEventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONQUIT);
        }
    }
}
