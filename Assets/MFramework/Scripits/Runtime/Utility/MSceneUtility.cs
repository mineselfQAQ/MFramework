using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MFramework
{
    public static class MSceneUtility
    {
        public static void LoadScene(string scene, Action onFinish = null)
        {
            MCoroutineManager.Instance.BeginCoroutineNoRecord(LoadSceneRoutine(scene, onFinish));
        }

        private static IEnumerator LoadSceneRoutine(string scene, Action onFinish)
        {
            var operation = SceneManager.LoadSceneAsync(scene);

            while (!operation.isDone) yield return null;

            onFinish?.Invoke();
        }
    }
}
