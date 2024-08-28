using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ClassTypeName))]
public class ClassTypeNameDrawer : PropertyDrawer
{
    protected ClassTypeName m_classTypeName;

    protected List<string> m_names;
    protected List<string> m_formatedNames;

    protected bool m_initialized = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!m_initialized)
        {
            m_initialized = true;
            Initialize();
        }

        if (m_names.Count > 0)
        {
            InitializeProperty(property);
            HandleGUI(position, property, label);
        }
    }

    protected virtual void Initialize()
    {
        m_classTypeName = (ClassTypeName)attribute;

        //寻找m_classTypeName的子类
        var classes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(m_classTypeName.type));
        //将类名写成List
        m_names = classes
            .Select(type => type.ToString())
            .ToList();
        //在单词(首字母大写)之间添加空格
        m_formatedNames = classes
            .Select(type => type.Name)
            .Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"))
            .ToList();
    }

    protected virtual void InitializeProperty(SerializedProperty property)
    {
        //列表首创设置为第一个(默认设置)
        if (property.stringValue.Length == 0)
        {
            property.stringValue = m_names[0];
        }
    }

    /// <summary>
    /// 绘制每一个数组元素的GUI
    /// </summary>
    protected virtual void HandleGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int current = m_names.IndexOf(property.stringValue);

        //前置Label
        position = EditorGUI.PrefixLabel(position, label);
        //下拉列表
        int selected = EditorGUI.Popup(position, current, m_formatedNames.ToArray());

        property.stringValue = m_names[selected];
    }
}