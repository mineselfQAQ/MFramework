using UnityEngine;

namespace MFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable, "#BuiltInEventManager#")]
    public class BuiltInEventManager : MonoSingleton<BuiltInEventManager>
    {
        private void Awake()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.AWAKE);
        }

        private void Start()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.START);
        }

        private void FixedUpdate()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.FIXEDUPDATE);
        }

        private void Update()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.UPDATE);
        }

        private void LateUpdate()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.LATEUPDATE);
        }

        private void OnApplicationFocus(bool focus)
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONFOCUS, focus);
        }

        private void OnApplicationPause(bool pause)
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONPAUSE, pause);
        }

        private void OnApplicationQuit()
        {
            EventSystem.DispatchBuiltIn(BuiltInEvent.ONAPPLICATIONQUIT);
        }
    }
}
