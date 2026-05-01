using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

using MFramework.Util;
namespace MFramework
{
    internal static class MEditorUtility
    {
        public static void OpenFile(string path)
        {
            EditorUtility.OpenWithDefaultApp(path.ReplaceSlash(false));
        }

        public static void OpenFolder(string path)
        {
            EditorUtility.RevealInFinder(path.ReplaceSlash(false));
        }

        public static Task<int> DisplayDialogAsync(string title, string message, string ok, string cancel, string alt)
        {
            int result = EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
            return Task.FromResult(result);
        }
    }

    public static class MEditorGUIUtility
    {
        public static void DrawH2(string title)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }
    }

    public sealed class EditorDelayExecute
    {
        private static readonly EditorDelayExecute _instance = new EditorDelayExecute();

        public static EditorDelayExecute Instance => _instance;

        private EditorDelayExecute()
        {
        }

        public void DelayDo(IEnumerator enumerator)
        {
            EditorApplication.CallbackFunction callback = null;
            callback = () =>
            {
                if (enumerator.MoveNext()) return;

                EditorApplication.update -= callback;
            };
            EditorApplication.update += callback;
        }
    }
}

