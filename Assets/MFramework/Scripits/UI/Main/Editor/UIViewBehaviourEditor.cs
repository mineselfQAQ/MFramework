using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIViewBehaviour))]
    public abstract class UIViewBehaviourEditor : Editor
    {
        #region 代码生成
        protected void DrawExportBtn()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("输出Base文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning); 
                        return; 
                    }
                    GenerateUIBaseCode();
                    GUIUtility.ExitGUI();
                }

                if (GUILayout.Button("输出核心文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning);
                        return;
                    }
                    GenerateUIMainCode();
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("完整输出"))
            {
                if (Application.isPlaying)
                {
                    MLog.Print("UI：请在非运行时导出", MLogType.Warning);
                    return;
                }

                GenerateBaseAndMainCode();
                GUIUtility.ExitGUI();
            }
        }
        
        private string GetPrefabPath()
        {
            //大致就是：
            //在不同状态下点击Inspector的Export按钮，使用同一方法并不都能获取到路径
            //1---最普通的，就是点击Project面板下的Prefab，此时就是最常规的AssetDatabase.GetAssetPath()
            //2---点击Hierarchy下的Prefab，此时只有通过PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot()才能获取
            //3---在预制体面板中点击，prefabStage.assetPath可以获取

            PrefabAssetType singlePrefabType = PrefabUtility.GetPrefabAssetType(target);
            PrefabInstanceStatus singleInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(target);
            string targetAssetPath = AssetDatabase.GetAssetPath(target);
            string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            //需要覆盖并正确判断这三种情况
            string finalPrefabPath = null;
            if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(targetAssetPath))
            {
                finalPrefabPath = targetAssetPath;//点击预设时
            }
            else if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(prefabAssetPath))
            {
                finalPrefabPath = prefabAssetPath;//预设拖入Hierarchy并选择时
            }
            else if (prefabStage != null)
            {
                finalPrefabPath = prefabStage.assetPath;//双击预设并在Hierarchy上选择时
            }

            return finalPrefabPath;
        }

        private void GenerateBaseAndMainCode()
        {
            string prefabPath = GetPrefabPath();
            if (string.IsNullOrEmpty(prefabPath))
            {
                MLog.Print("该物体并非Prefab，请先设置为Prefab后重试", MLogType.Warning);
                return;
            }
            string fullPrefabPath = Path.GetFullPath(prefabPath);
            string directoryPath = Path.GetDirectoryName(fullPrefabPath);
            directoryPath = MEditorUtility.DisplayGenerateFileDialog(directoryPath);
            if (directoryPath == null) return;

            //Base变量
            string baseClassName = $"{Path.GetFileNameWithoutExtension(prefabPath)}Base";
            string baseFileName = $"{baseClassName}.cs";
            string baseBaseClassName = target is UIPanelBehaviour ? "UIPanel" : "UIWidget";
            string baseFilePath = Path.Combine(directoryPath, baseFileName);
            //Main变量
            string mainClassName = $"{Path.GetFileNameWithoutExtension(prefabPath)}";
            string mainFileName = $"{mainClassName}.cs";
            string mainFilePath = Path.Combine(directoryPath, mainFileName);

            if(File.Exists(baseFilePath) || File.Exists(mainFilePath)) 
            {
                bool state = EditorUtility.DisplayDialog("警告！！！", 
                    "该路径下已存在Base文件或Main文件，是否直接覆盖", "确认", "取消");
                if (!state) 
                {
                    MLog.Print("已取消生成", MLogType.Warning);
                    return;
                }
            }

            string baseCode = GetBaseCode(baseClassName, baseBaseClassName);
            if (baseCode == null) return;
            MPathUtility.SaveFile(baseFilePath, baseCode);
            string mainCode = GetMainCode(mainClassName);
            if (baseCode == null) return;
            MPathUtility.SaveFile(mainFilePath, mainCode);
            AssetDatabase.Refresh();

            MLog.Print($"已成功生成{baseFileName}与{mainFileName}文件");
        }

        private void GenerateUIBaseCode()
        {
            string prefabPath = GetPrefabPath();
            if (string.IsNullOrEmpty(prefabPath))
            {
                MLog.Print("该物体并非Prefab，请先设置为Prefab后重试", MLogType.Warning);
                return;
            }
            string fullPrefabPath = Path.GetFullPath(prefabPath);

            string className = $"{Path.GetFileNameWithoutExtension(prefabPath)}Base";
            string fileName = $"{className}.cs";
            string baseClassName = target is UIPanelBehaviour ? "UIPanel" : "UIWidget";

            string filePath = Path.Combine(Path.GetDirectoryName(fullPrefabPath), fileName);
            filePath = MEditorUtility.DisplayGenerateFileDialog(filePath, fileName);
            if (filePath == null) return;

            string code = GetBaseCode(className, baseClassName);
            if (code == null) return;

            MPathUtility.SaveFile(filePath, code);
            AssetDatabase.Refresh();

            MLog.Print($"已成功生成{fileName}文件");
        }

        private string GetBaseCode(string className, string baseClassName)
        {
            string code = UIBASECODE;

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{BaseClassName}", baseClassName);

            string fieldsDefine, bindComps, bindEvents, unbindEvents, unbindComps;
            bool flag = GenerateAllBaseCodePart(out fieldsDefine, out bindComps, out bindEvents, out unbindEvents, out unbindComps);
            if (!flag) return null;

            code = code.Replace("{FieldsDefine}", fieldsDefine);
            code = code.Replace("{BindComps}", bindComps);
            code = code.Replace("{BindEvents}", bindEvents);
            code = code.Replace("{UnbindEvents}", unbindEvents);
            code = code.Replace("{UnbindComps}", unbindComps);

            return code;
        }

        private bool GenerateAllBaseCodePart(out string fieldsDefine, out string bindComps, out string bindEvents, out string unbindEvents, out string unbindComps)
        {
            UIViewBehaviour behaviour = (UIViewBehaviour)target;
            HashSet<string> targetSet = new HashSet<string>();

            for (int i = 0; i < behaviour.CompCollections.Count; i++)
            {
                var collection = behaviour.CompCollections[i];

                if (targetSet.Contains(collection.Target.name))
                {
                    MLog.Print("自动生成不能同时存在同名Target，请重试", MLogType.Error);
                    fieldsDefine = null; bindComps = null; bindEvents = null; unbindEvents = null; unbindComps = null;
                    return false;
                }
                targetSet.Add(collection.Target.name);
            }

            fieldsDefine = GenerateFieldsDefine(behaviour);
            bindComps = GenerateBindComps(behaviour);
            bindEvents = GenerateBindEvents(behaviour);
            unbindEvents = GenerateUnbindEvents(behaviour);
            unbindComps = GenerateUnbindComps(behaviour);

            return true;
        }

        private string GenerateFieldsDefine(UIViewBehaviour target)
        {
            StringBuilder res = new StringBuilder();

            foreach (var collection in target.CompCollections)
            {
                foreach (var comp in collection.CompList)
                {
                    string name = $"m_{collection.Target.name}_{GetCompShortName(comp.GetType().Name)}";
                    string type = comp.GetType().Name;

                    string tempLine = FIELDBASECODE;
                    tempLine = tempLine.Replace("{Type}", type);
                    tempLine = tempLine.Replace("{Name}", name);

                    res.Append(tempLine + "\n\t");
                }
            }
            string resStr = res.ToString();
            resStr = resStr.TrimEnd('\t', '\n');

            return resStr;
        }
        private string GenerateBindComps(UIViewBehaviour target)
        {
            StringBuilder res = new StringBuilder();

            int i = 0;
            foreach (var collection in target.CompCollections)
            {
                int j = 0;
                foreach (var comp in collection.CompList)
                {
                    string name = $"m_{collection.Target.name}_{GetCompShortName(comp.GetType().Name)}";
                    string type = comp.GetType().Name;

                    string tempLine = BINDCOMPSBASECODE;
                    tempLine = tempLine.Replace("{Name}", name);
                    tempLine = tempLine.Replace("{Type}", type);
                    tempLine = tempLine.Replace("{i}", i.ToString());
                    tempLine = tempLine.Replace("{j}", j.ToString());

                    res.Append(tempLine + "\n\t\t");

                    j++;
                }
                i++;
            }

            return res.ToString();
        }
        private string GenerateBindEvents(UIViewBehaviour target)
        {
            StringBuilder res = new StringBuilder();

            foreach (var collection in target.CompCollections)
            {
                foreach (var comp in collection.CompList)
                {
                    if (!CanBindOrUnbind(comp.GetType().Name)) continue;

                    string name = $"m_{collection.Target.name}_{GetCompShortName(comp.GetType().Name)}";

                    string tempLine = BINDEVENTSBASECODE;
                    tempLine = tempLine.Replace("{Name}", name);

                    res.Append(tempLine + "\n\t\t");
                }
            }
            string resStr = res.ToString();
            resStr = resStr.TrimEnd('\t', '\n');

            return resStr;
        }
        private string GenerateUnbindEvents(UIViewBehaviour target)
        {
            StringBuilder res = new StringBuilder();

            foreach (var collection in target.CompCollections)
            {
                foreach (var comp in collection.CompList)
                {
                    if (!CanBindOrUnbind(comp.GetType().Name)) continue;

                    string name = $"m_{collection.Target.name}_{GetCompShortName(comp.GetType().Name)}";

                    string tempLine = UNBINDEVENTSBASECODE;
                    tempLine = tempLine.Replace("{Name}", name);

                    res.Append(tempLine + "\n\t\t");
                }
            }

            return res.ToString();
        }
        private string GenerateUnbindComps(UIViewBehaviour target)
        {
            StringBuilder res = new StringBuilder();

            foreach (var collection in target.CompCollections)
            {
                foreach (var comp in collection.CompList)
                {
                    string name = $"m_{collection.Target.name}_{GetCompShortName(comp.GetType().Name)}";

                    string tempLine = UNBINDCOMPSBASECODE;
                    tempLine = tempLine.Replace("{Name}", name);

                    res.Append(tempLine + "\n\t\t");
                }
            }

            string resStr = res.ToString();
            resStr = resStr.TrimEnd('\t', '\n');

            return resStr;
        }

        private bool CanBindOrUnbind(string typeName)
        {
            HashSet<string> set = new HashSet<string>()
            {
                "Button", "Toggle", "Dropdown", "InputField", "Slider", "Scrollbar", "ScrollRect",
                "TMP_Dropdown", "TMP_InputField",
            };

            if (!set.Contains(typeName))
            {
                return false;
            }
            return true;
        }
        private string GetCompShortName(string name)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"VerticalLayoutGroup", "VLayoutGroup"},
                {"HorizontalLayoutGroup","HLayoutGroup"},
                {"GridLayoutGroup", "GLayoutGroup"},

                {"TextMeshProUGUI", "TMPText"},
                {"TMP_Dropdown", "TMPDropdown"},
                {"TMP_InputField", "TMPInputField"},
            };

            return dict.ContainsKey(name) ? dict[name] : name;
        }

        private void GenerateUIMainCode()
        {
            string prefabPath = GetPrefabPath();
            if (string.IsNullOrEmpty(prefabPath))
            {
                MLog.Print("该物体并非Prefab，请先设置为Prefab后重试", MLogType.Warning);
                return;
            }

            string fullPrefabPath = Path.GetFullPath(prefabPath);
            string className = Path.GetFileNameWithoutExtension(prefabPath);
            string fileName = $"{className}.cs";

            string filePath = Path.Combine(Path.GetDirectoryName(fullPrefabPath), fileName);
            filePath = MEditorUtility.DisplayGenerateFileDialog(filePath, fileName);
            if (filePath == null) return;

            if (File.Exists(filePath))
            {
                MLog.Print($"{fileName}已存在，如需重新创建，请删除后再试", MLogType.Warning);
                return;
            }

            string code = GetMainCode(className);

            MPathUtility.SaveFile(filePath, code);
            AssetDatabase.Refresh();

            MLog.Print($"已成功生成{fileName}文件");
        }
        private string GetMainCode(string className)
        {
            string code = UIMAINCODE;

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{BaseClassName}", $"{className}Base");
            code = code.Replace("{PanelUniqueFunction}", target is UIPanelBehaviour ? PANELUNIQUEFUNCTION : "");

            return code;
        }

        private const string UIBASECODE =
    @"using MFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        private const string FIELDBASECODE = "protected {Type} {Name};";
        private const string BINDCOMPSBASECODE = "{Name} = ({Type})viewBehaviour.GetComp({i}, {j});";
        private const string BINDEVENTSBASECODE = "BindEvent({Name});";
        private const string UNBINDEVENTSBASECODE = "UnbindEvent({Name});";
        private const string UNBINDCOMPSBASECODE = "{Name} = null;";

        private const string UIMAINCODE =
    @"using UnityEngine.UI;

public class {ClassName} : {BaseClassName}
{
    public void Init()
    {
        
    }

    protected override void OnClicked(Button button) { }

    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
    
    {PanelUniqueFunction}
}";
        private const string PANELUNIQUEFUNCTION = 
    @"protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool focus) { }";
        #endregion
    }
}