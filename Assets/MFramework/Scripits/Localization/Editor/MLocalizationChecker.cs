using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public class MLocalizationChecker : EditorWindow
    {
        public static void Init()
        {
            EditorWindow window = GetWindow<MLocalizationChecker>(true, "MLocalizationChecker", false);
            window.minSize = new Vector2(500, 200);
            window.Show();
        }

        private void OnGUI()
        {
            var locals = MLocalizationUtility.FindAllLoclizations();
            foreach (var local in locals)
            {
                if (local.LocalMode == LocalizationMode.Off) continue;

                //local.LocalID
            }
        }
    }
}