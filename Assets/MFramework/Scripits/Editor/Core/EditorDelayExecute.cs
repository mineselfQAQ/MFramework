using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

namespace MFramework
{
    public class EditorDelayExecute : Singleton<EditorDelayExecute>
    {
        private EditorDelayExecute() { }
        
        public void DelayRefresh()
        {
            EditorCoroutineUtility.StartCoroutine(DelayRefreshCoroutine(), this);
        }

        public void DelayDo(IEnumerator enumerator)
        {
            EditorCoroutineUtility.StartCoroutine(enumerator, this);
        }
        public void DelayDo(Action onFinish)
        {
            EditorCoroutineUtility.StartCoroutine(DelayCoroutine(onFinish), this);
        }

        private IEnumerator DelayRefreshCoroutine()
        {
            yield return new EditorWaitForSeconds(0.2f);
            AssetDatabase.Refresh();
        }

        private IEnumerator DelayCoroutine(Action onFinish)
        {
            yield return new EditorWaitForSeconds(0.2f);

            onFinish?.Invoke();
        }
    }
}
