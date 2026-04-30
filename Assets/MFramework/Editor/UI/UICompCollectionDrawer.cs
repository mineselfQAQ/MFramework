using MFramework.UI;
using UnityEditor;
using UnityEngine;

namespace MFramework.Editor.UI
{
    [CustomPropertyDrawer(typeof(UICompCollection), true)]
    public class UICompCollectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetProperty = property.FindPropertyRelative("target");
            SerializedProperty componentListProperty = property.FindPropertyRelative("compList");

            if (RemoveNulls(componentListProperty))
            {
                componentListProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float itemSpacing = 5f;
            float targetWidth = (position.width - itemSpacing) / 3f;
            float selectWidth = targetWidth * 2f;
            Rect targetRect = new(position.x, position.y, targetWidth, position.height);
            Rect selectRect = new(position.x + targetWidth + itemSpacing, position.y, selectWidth, position.height);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(targetRect, targetProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                componentListProperty.ClearArray();
                targetProperty.serializedObject.ApplyModifiedProperties();
                componentListProperty.serializedObject.ApplyModifiedProperties();
            }

            bool hasTarget = targetProperty.objectReferenceValue != null;
            using (new EditorGUI.DisabledScope(!hasTarget))
            {
                if (hasTarget)
                {
                    GameObject target = (GameObject)targetProperty.objectReferenceValue;
                    Component[] components = target.GetComponents<Component>();

                    GUIContent content = new(componentListProperty.arraySize == 0 ? "Default" : string.Empty);
                    if (GUI.Button(selectRect, content, EditorStyles.popup))
                    {
                        BuildPopupList(components, componentListProperty).DropDown(selectRect);
                    }

                    int iconIndex = 0;
                    foreach (Component component in components)
                    {
                        if (component == null) continue;
                        if (GetIndexFromSavedComponentList(componentListProperty, component) < 0) continue;

                        DrawIcon(selectRect, iconIndex, UIEditorUtility.GetIcon(component.GetType()));
                        iconIndex++;
                    }
                }
                else
                {
                    GUI.Button(selectRect, "Default", EditorStyles.popup);
                }
            }

            EditorGUI.indentLevel = oldIndent;
            EditorGUI.EndProperty();
        }

        private static GenericMenu BuildPopupList(Component[] components, SerializedProperty componentListProperty)
        {
            GenericMenu menu = new();
            foreach (Component component in components)
            {
                if (component == null) continue;

                int savedIndex = GetIndexFromSavedComponentList(componentListProperty, component);
                menu.AddItem(new GUIContent(component.GetType().Name), savedIndex >= 0, source =>
                {
                    if (savedIndex >= 0)
                    {
                        componentListProperty.GetArrayElementAtIndex(savedIndex).objectReferenceValue = null;
                        componentListProperty.DeleteArrayElementAtIndex(savedIndex);
                    }
                    else
                    {
                        componentListProperty.InsertArrayElementAtIndex(componentListProperty.arraySize);
                        componentListProperty.GetArrayElementAtIndex(componentListProperty.arraySize - 1).objectReferenceValue = source as Component;
                    }

                    componentListProperty.serializedObject.ApplyModifiedProperties();
                    EditorApplication.RepaintHierarchyWindow();
                }, component);
            }

            return menu;
        }

        private static bool RemoveNulls(SerializedProperty componentListProperty)
        {
            bool hasNull = false;
            for (int i = componentListProperty.arraySize - 1; i >= 0; i--)
            {
                if (componentListProperty.GetArrayElementAtIndex(i).objectReferenceValue != null) continue;

                componentListProperty.DeleteArrayElementAtIndex(i);
                hasNull = true;
            }

            return hasNull;
        }

        private static int GetIndexFromSavedComponentList(SerializedProperty componentListProperty, Component component)
        {
            for (int i = 0; i < componentListProperty.arraySize; i++)
            {
                Component savedComponent = componentListProperty.GetArrayElementAtIndex(i).objectReferenceValue as Component;
                if (savedComponent == component) return i;
            }

            return -1;
        }

        private static void DrawIcon(Rect selectRect, int index, Texture icon)
        {
            if (icon == null) return;

            float iconSize = EditorGUIUtility.singleLineHeight * 0.8f;
            const float leftPadding = 5f;
            const float iconSpacing = 5f;
            float x = selectRect.x + leftPadding + (iconSize + iconSpacing) * index;
            float y = selectRect.y + (selectRect.height - iconSize) / 2f;
            GUI.DrawTexture(new Rect(x, y, iconSize, iconSize), icon);
        }
    }
}
