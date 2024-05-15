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
                }

                if (GUILayout.Button("输出核心文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning);
                        return;
                    }
                    GenerateUIMainCode();
                }
            }
            GUILayout.EndHorizontal();
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

        private void GenerateUIBaseCode()
        {
            string prefabPath = GetPrefabPath();
            if (string.IsNullOrEmpty(prefabPath))
            {
                MLog.Print("该物体并非Prefab，请先设置为Prefab后重试", MLogType.Warning);
                return;
            }

            string fullPrefabPath = Path.GetFullPath(prefabPath);

            string className = Path.GetFileNameWithoutExtension(prefabPath);
            string fileName = $"{className}Base.cs";
            string baseClassName = target is UIPanelBehaviour ? "UIPanel" : "UIWidget";

            string filePath = Path.Combine(Path.GetDirectoryName(fullPrefabPath), fileName);
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{filePath}处吗？", "确认", "取消", "更改路径");
            if (state == 1)//取消
            {
                MLog.Print($"已取消生成{fileName}文件.", MLogType.Warning);
                GUIUtility.ExitGUI();
                return;
            }

            string code = UIBASECODE;

            code = code.Replace("{ClassName}", $"{className}Base");
            code = code.Replace("{BaseClassName}", baseClassName);

            string fieldsDefine, bindComps, bindEvents, unbindEvents, unbindComps;
            bool flag = GenerateAllBaseCode(out fieldsDefine, out bindComps, out bindEvents, out unbindEvents, out unbindComps);
            if (!flag) return;

            code = code.Replace("{FieldsDefine}", fieldsDefine);
            code = code.Replace("{BindComps}", bindComps);
            code = code.Replace("{BindEvents}", bindEvents);
            code = code.Replace("{UnbindEvents}", unbindEvents);
            code = code.Replace("{UnbindComps}", unbindComps);

            
            if (state == 0)//确认
            {
                MPathUtility.SaveFile(filePath, code);
            }
            else//更改路径
            {
                filePath = MEditorUtitlity.ChangePath();
                filePath = Path.Combine(filePath, fileName);
                MPathUtility.SaveFile(filePath, code);
                GUIUtility.ExitGUI();
            }

            AssetDatabase.Refresh();

            MLog.Print($"已成功生成{fileName}文件");
        }

        private bool GenerateAllBaseCode(out string fieldsDefine, out string bindComps, out string bindEvents, out string unbindEvents, out string unbindComps)
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
            int state = EditorUtility.DisplayDialogComplex("Generating",
                    $"确定文件将生成在{filePath}处吗？", "确认", "取消", "更改路径");
            if (state == 1)//取消
            {
                MLog.Print($"已取消生成{fileName}文件.", MLogType.Warning);
                GUIUtility.ExitGUI();
                return;
            }

            if (File.Exists(filePath))
            {
                MLog.Print($"{fileName}已存在，如需重新创建，请删除后再试", MLogType.Warning);
                GUIUtility.ExitGUI();
                return;
            }

            string code = UIMAINCODE;

            code = code.Replace("{ClassName}", className);
            code = code.Replace("{BaseClassName}", $"{className}Base");
            code = code.Replace("{PanelUniqueFunction}", target is UIPanelBehaviour ? PANELUNIQUEFUNCTION : "");

            if (state == 0)//确认
            {
                MPathUtility.SaveFile(filePath, code);
            }
            else//更改路径
            {
                filePath = MEditorUtitlity.ChangePath();
                filePath = Path.Combine(filePath, fileName);
                MPathUtility.SaveFile(filePath, code);
            }

            AssetDatabase.Refresh();

            MLog.Print($"已成功生成{fileName}文件");
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