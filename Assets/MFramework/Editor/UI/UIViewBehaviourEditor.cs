using System.Collections.Generic;
using System.IO;
using System.Text;
using MFramework.Core;
using MFramework.UI;
using MFramework.Util;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework.Editor.UI
{
    [CustomEditor(typeof(UIViewBehaviour), true)]
    public abstract class UIViewBehaviourEditor : UnityEditor.Editor
    {
        protected void DrawCompCollections()
        {
            GUIContent label = new("Component Collections", "Collected UI components.");
            SerializedProperty property = serializedObject.FindProperty("compCollections");
            EditorGUILayout.PropertyField(property, label);
        }

        protected void DrawExportButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Export Base"))
                {
                    GenerateUIBaseCode();
                    GUIUtility.ExitGUI();
                }

                if (GUILayout.Button("Export Main"))
                {
                    GenerateUIMainCode();
                    GUIUtility.ExitGUI();
                }
            }

            if (GUILayout.Button("Export Both"))
            {
                GenerateBaseAndMainCode();
                GUIUtility.ExitGUI();
            }
        }

        private void GenerateBaseAndMainCode()
        {
            if (!CanExport(out string prefabPath)) return;

            string fullPrefabPath = Path.GetFullPath(prefabPath);
            string directoryPath = UIEditorUtility.DisplayGenerateFileDialog(Path.GetDirectoryName(fullPrefabPath));
            if (directoryPath == null) return;

            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            string baseClassName = $"{prefabName}Base";
            string mainClassName = prefabName;
            string basePath = Path.Combine(directoryPath, $"{baseClassName}.cs");
            string mainPath = Path.Combine(directoryPath, $"{mainClassName}.cs");

            if (File.Exists(basePath) || File.Exists(mainPath))
            {
                bool overwrite = EditorUtility.DisplayDialog("Overwrite", "Base or Main file already exists. Overwrite?", "Confirm", "Cancel");
                if (!overwrite) return;
            }

            MSerializationUtil.SaveToFile(basePath, GetBaseCode(baseClassName, GetViewBaseClassName()));
            MSerializationUtil.SaveToFile(mainPath, GetMainCode(mainClassName));
            AssetDatabase.Refresh();
            MLog.Default?.D($"Generated {Path.GetFileName(basePath)} and {Path.GetFileName(mainPath)}.");
        }

        private void GenerateUIBaseCode()
        {
            if (!CanExport(out string prefabPath)) return;

            string fullPrefabPath = Path.GetFullPath(prefabPath);
            string className = $"{Path.GetFileNameWithoutExtension(prefabPath)}Base";
            string fileName = $"{className}.cs";
            string filePath = UIEditorUtility.DisplayGenerateFileDialog(Path.Combine(Path.GetDirectoryName(fullPrefabPath), fileName), fileName);
            if (filePath == null) return;

            MSerializationUtil.SaveToFile(filePath, GetBaseCode(className, GetViewBaseClassName()));
            AssetDatabase.Refresh();
            MLog.Default?.D($"Generated {fileName}.");
        }

        private void GenerateUIMainCode()
        {
            if (!CanExport(out string prefabPath)) return;

            string fullPrefabPath = Path.GetFullPath(prefabPath);
            string className = Path.GetFileNameWithoutExtension(prefabPath);
            string fileName = $"{className}.cs";
            string filePath = UIEditorUtility.DisplayGenerateFileDialog(Path.Combine(Path.GetDirectoryName(fullPrefabPath), fileName), fileName);
            if (filePath == null) return;

            if (File.Exists(filePath))
            {
                MLog.Default?.E($"{fileName} already exists. Delete it first if you want to regenerate it.");
                return;
            }

            MSerializationUtil.SaveToFile(filePath, GetMainCode(className));
            AssetDatabase.Refresh();
            MLog.Default?.D($"Generated {fileName}.");
        }

        private bool CanExport(out string prefabPath)
        {
            prefabPath = null;
            if (Application.isPlaying)
            {
                MLog.Default?.W("UI code export is only available outside Play Mode.");
                return false;
            }

            prefabPath = UIPanelUtility.GetPrefabPath(target);
            if (!string.IsNullOrEmpty(prefabPath)) return true;

            MLog.Default?.E("Selected object is not a prefab. Create or select a prefab first.");
            return false;
        }

        private string GetBaseCode(string className, string baseClassName)
        {
            string code = UIBaseCode;
            code = code.Replace("{ClassName}", className);
            code = code.Replace("{BaseClassName}", baseClassName);

            GenerateAllBaseCodePart(out string fields, out string bindComps, out string bindEvents, out string unbindEvents, out string unbindComps);
            code = code.Replace("{FieldsDefine}", fields);
            code = code.Replace("{BindComps}", bindComps);
            code = code.Replace("{BindEvents}", bindEvents);
            code = code.Replace("{UnbindEvents}", unbindEvents);
            code = code.Replace("{UnbindComps}", unbindComps);
            return code;
        }

        private void GenerateAllBaseCodePart(out string fields, out string bindComps, out string bindEvents, out string unbindEvents, out string unbindComps)
        {
            UIViewBehaviour behaviour = (UIViewBehaviour)target;
            HashSet<string> targetNames = new();

            foreach (UICompCollection collection in behaviour.CompCollections)
            {
                if (collection.Target == null) continue;
                if (targetNames.Add(collection.Target.name)) continue;

                MLog.Default?.E("UI code export requires unique target names in Component Collections.");
                fields = bindComps = bindEvents = unbindEvents = unbindComps = string.Empty;
                return;
            }

            fields = GenerateFieldsDefine(behaviour);
            bindComps = GenerateBindComps(behaviour);
            bindEvents = GenerateBindEvents(behaviour);
            unbindEvents = GenerateUnbindEvents(behaviour);
            unbindComps = GenerateUnbindComps(behaviour);
        }

        private string GenerateFieldsDefine(UIViewBehaviour behaviour)
        {
            StringBuilder builder = new();
            foreach (UICompCollection collection in behaviour.CompCollections)
            {
                if (collection.Target == null) continue;

                foreach (Component component in collection.CompList)
                {
                    if (component == null) continue;
                    string name = GetFieldName(collection.Target.name, component.GetType().Name);
                    builder.Append("    protected ").Append(component.GetType().Name).Append(' ').Append(name).AppendLine(";");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private string GenerateBindComps(UIViewBehaviour behaviour)
        {
            StringBuilder builder = new();
            for (int i = 0; i < behaviour.CompCollections.Count; i++)
            {
                UICompCollection collection = behaviour.CompCollections[i];
                if (collection.Target == null) continue;

                for (int j = 0; j < collection.CompList.Count; j++)
                {
                    Component component = collection.CompList[j];
                    if (component == null) continue;

                    string name = GetFieldName(collection.Target.name, component.GetType().Name);
                    builder.Append("        ").Append(name).Append(" = (")
                        .Append(component.GetType().Name).Append(")viewBehaviour.GetComp(")
                        .Append(i).Append(", ").Append(j).AppendLine(");");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private string GenerateBindEvents(UIViewBehaviour behaviour)
        {
            StringBuilder builder = new();
            foreach (UICompCollection collection in behaviour.CompCollections)
            {
                if (collection.Target == null) continue;

                foreach (Component component in collection.CompList)
                {
                    if (component == null || !CanBindEvent(component)) continue;

                    string name = GetFieldName(collection.Target.name, component.GetType().Name);
                    builder.Append("        BindEvent(").Append(name).AppendLine(");");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private string GenerateUnbindEvents(UIViewBehaviour behaviour)
        {
            StringBuilder builder = new();
            foreach (UICompCollection collection in behaviour.CompCollections)
            {
                if (collection.Target == null) continue;

                foreach (Component component in collection.CompList)
                {
                    if (component == null || !CanBindEvent(component)) continue;

                    string name = GetFieldName(collection.Target.name, component.GetType().Name);
                    builder.Append("        UnbindEvent(").Append(name).AppendLine(");");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private string GenerateUnbindComps(UIViewBehaviour behaviour)
        {
            StringBuilder builder = new();
            foreach (UICompCollection collection in behaviour.CompCollections)
            {
                if (collection.Target == null) continue;

                foreach (Component component in collection.CompList)
                {
                    if (component == null) continue;

                    string name = GetFieldName(collection.Target.name, component.GetType().Name);
                    builder.Append("        ").Append(name).AppendLine(" = null;");
                }
            }

            return builder.ToString().TrimEnd();
        }

        private static bool CanBindEvent(Component component)
        {
            return component is Button ||
                   component is MButton ||
                   component is Toggle ||
                   component is Dropdown ||
                   component is InputField ||
                   component is Slider ||
                   component is Scrollbar ||
                   component is ScrollRect ||
                   component is TMP_Dropdown ||
                   component is TMP_InputField;
        }

        private static string GetFieldName(string targetName, string componentName)
        {
            return $"{GetComponentFieldPrefix(componentName)}{targetName}";
        }

        private static string GetComponentFieldPrefix(string name)
        {
            return name switch
            {
                "Button" => "_btn_",
                "MButton" => "_btn_",
                "Toggle" => "_tgl_",
                "Dropdown" => "_dropdown_",
                "TMP_Dropdown" => "_dropdown_",
                "InputField" => "_input_",
                "TMP_InputField" => "_input_",
                "Slider" => "_slider_",
                "Scrollbar" => "_scrollbar_",
                "ScrollRect" => "_scrollRect_",
                "Image" => "_img_",
                "MImage" => "_img_",
                "RawImage" => "_rawImg_",
                "Text" => "_txt_",
                "MText" => "_txt_",
                "TextMeshProUGUI" => "_txt_",
                "CanvasGroup" => "_canvasGroup_",
                "VerticalLayoutGroup" => "_vLayout_",
                "HorizontalLayoutGroup" => "_hLayout_",
                "GridLayoutGroup" => "_gLayout_",
                _ => $"_{GetComponentShortName(name)}_",
            };
        }

        private static string GetComponentShortName(string name)
        {
            return name switch
            {
                "VerticalLayoutGroup" => "VLayoutGroup",
                "HorizontalLayoutGroup" => "HLayoutGroup",
                "GridLayoutGroup" => "GLayoutGroup",
                "TextMeshProUGUI" => "TMPText",
                "TMP_Dropdown" => "TMPDropdown",
                "TMP_InputField" => "TMPInputField",
                _ => name,
            };
        }

        private string GetMainCode(string className)
        {
            string code = UIMainCode;
            code = code.Replace("{ClassName}", className);
            code = code.Replace("{BaseClassName}", $"{className}Base");
            code = code.Replace("{PanelUniqueFunction}", target is UIPanelBehaviour ? PanelUniqueFunction : string.Empty);
            return code;
        }

        private string GetViewBaseClassName()
        {
            return target is UIPanelBehaviour ? nameof(UIPanel) : nameof(UIWidget);
        }

        private const string UIBaseCode =
@"using MFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class {ClassName} : {BaseClassName}
{
{FieldsDefine}

    protected override void OnBindCompsAndEvents()
    {
{BindComps}
{BindEvents}
    }

    protected override void OnUnbindCompsAndEvents()
    {
{UnbindEvents}
{UnbindComps}
    }
}";

        private const string UIMainCode =
@"using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class {ClassName} : {BaseClassName}
{
    protected override void OnClicked(Button button)
    {
    }

    // Used when Open Animation Mode is SelfControl.
    protected override IEnumerator OnOpenAnim()
    {
        yield break;
    }

    // Used when Close Animation Mode is SelfControl.
    protected override IEnumerator OnCloseAnim()
    {
        yield break;
    }

    protected override GameObject LoadPrefab(string prefabPath)
    {
        return base.LoadPrefab(prefabPath);
    }

    protected override void OnCreating() { }
    protected override void OnCreated() { }
    protected override void OnDestroying() { }
    protected override void OnDestroyed() { }

{PanelUniqueFunction}
}";

        private const string PanelUniqueFunction =
@"    protected override void OnVisibleChanged(bool visible) { }
    protected override void OnFocusChanged(bool focus) { }";
    }
}
