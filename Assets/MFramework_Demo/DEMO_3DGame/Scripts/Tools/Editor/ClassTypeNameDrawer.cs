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

        //Ѱ��m_classTypeName������
        var classes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(m_classTypeName.type));
        //������д��List
        m_names = classes
            .Select(type => type.ToString())
            .ToList();
        //�ڵ���(����ĸ��д)֮����ӿո�
        m_formatedNames = classes
            .Select(type => type.Name)
            .Select(name => Regex.Replace(name, "(\\B[A-Z])", " $1"))
            .ToList();
    }

    protected virtual void InitializeProperty(SerializedProperty property)
    {
        //�б��״�����Ϊ��һ��(Ĭ������)
        if (property.stringValue.Length == 0)
        {
            property.stringValue = m_names[0];
        }
    }

    /// <summary>
    /// ����ÿһ������Ԫ�ص�GUI
    /// </summary>
    protected virtual void HandleGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int current = m_names.IndexOf(property.stringValue);

        //ǰ��Label
        position = EditorGUI.PrefixLabel(position, label);
        //�����б�
        int selected = EditorGUI.Popup(position, current, m_formatedNames.ToArray());

        property.stringValue = m_names[selected];
    }
}