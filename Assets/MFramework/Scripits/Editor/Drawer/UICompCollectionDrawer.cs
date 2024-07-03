using UnityEditor;
using UnityEngine;

namespace MFramework
{
    /// <summary>
    /// UICompCollection的绘制更改
    /// </summary>
    [CustomPropertyDrawer(typeof(UICompCollection), true)]
    public class UICompCollectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetSP = property.FindPropertyRelative("target");
            SerializedProperty compListSP = property.FindPropertyRelative("compList");

            //清除被删除的组件
            //大致是这样的：
            //物体上可能挂载组件1/2/3/4，同时列表选择了组件1/2/3/4，此时组件3被删除了，
            //此时会：将其引用设置为null，但其实还是占用位置，应该删除才行
            if (RemoveNulls(compListSP))
            {
                //刷新一次
                compListSP.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            {
                //注意：
                //如EditorGUI.PrefixLabel()，它会接收Rect，
                //要注意的是它会返回Rect，这决定了下一个GUI绘制的位置

                //名字(前缀)
                //id---通过GetControlID()获取，关键为FocusType:
                //FocusType.Keyboard---可以选中，按Tab可以跳转
                //FocusType.Passive---不可选中，按Tab会跳过该部分
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                //自适应的宽度（除间距外，target:1/3, select:2/3）
                float itemSpacing = 5;
                float targetWidth = (position.width - itemSpacing) / 3;
                float selectWidth = targetWidth * 2;
                //target和compList的Rect
                Rect targetRect = new Rect(position.x, position.y, targetWidth, position.height);
                Rect selectRect = new Rect(position.x + targetWidth + itemSpacing, position.y, selectWidth, position.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;//无缩进段
                {
                    //左侧的GameObject
                    EditorGUI.BeginChangeCheck();
                    {
                        GUI.Box(targetRect, GUIContent.none);//绘制底框(会被压住无意义，可以不绘制)
                        //GameObject框
                        EditorGUI.PropertyField(targetRect, targetSP, GUIContent.none);
                        //当GameObject框发生改变时
                        if (EditorGUI.EndChangeCheck())
                        {
                            //componentListSP不再使用，同时改变targetSP
                            compListSP.ClearArray();
                            targetSP.serializedObject.ApplyModifiedProperties();
                            compListSP.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    //右侧的下拉列表
                    bool isExsistTarget = targetSP.objectReferenceValue != null;
                    //不存在targetSP就禁用componentListSP控件(但是无论是否禁用都会渲染控件)
                    using (new EditorGUI.DisabledScope(!isExsistTarget))
                    {
                        if (isExsistTarget)
                        {
                            GameObject target = targetSP.objectReferenceValue as GameObject;//获取物体
                            Component[] existComps = target.GetComponents<Component>();//获取物体上的组件

                            //按钮(铺在下方)
                            GUIContent guiContent = new GUIContent();
                            guiContent.text = (compListSP.arraySize == 0) ? "Default" : string.Empty;//按钮文本
                            if (GUI.Button(selectRect, guiContent, EditorStyles.popup))
                            {
                                //创建下拉列表并展开
                                BuildPopupList(existComps, compListSP).DropDown(selectRect);
                            }

                            //图标列表(覆盖在上方)
                            int posIndex = 0;
                            for (int i = 0; i < existComps.Length; i++)//用它遍历能够保证组件在Inspector上的顺序
                            {
                                Component comp = existComps[i];

                                int savedIndex = GetIndexFromSavedComponentList(compListSP, comp);//只显示已保存的组件
                                if (savedIndex >= 0)
                                {
                                    Texture icon = MEditorUtility.GetIcon(comp.GetType());
                                    DrawIcon(selectRect, posIndex, icon);
                                    posIndex++;
                                }
                            }
                        }
                        else
                        {
                            //不存在targetSP时的按钮
                            if (GUI.Button(selectRect, "Default", EditorStyles.popup)) { };
                        }
                    }
                }
                EditorGUI.indentLevel = oldIndent;//缩进还原
            }
            EditorGUI.EndProperty();
        }

        private GenericMenu BuildPopupList(Component[] existComps, SerializedProperty componentListSP)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Component comp in existComps)
            {
                //获取index
                int savedIndex = GetIndexFromSavedComponentList(componentListSP, comp);

                //添加每一个comp
                //on---true时为勾选状态，false时为非勾选状态
                menu.AddItem(new GUIContent(comp.GetType().Name), savedIndex >= 0, (source) =>
                {
                    //点击Item时，若Item已存在执行移除，否则执行添加
                    //其实就是多选列表的选择与取消
                    if (savedIndex >= 0)
                    {
                        //注意，删除元素前必须先将其置为null，
                        //否则直接调用DeleteArrayElementAtIndex会使元素变为null而不是从列表中移除
                        componentListSP.GetArrayElementAtIndex(savedIndex).objectReferenceValue = null;
                        componentListSP.DeleteArrayElementAtIndex(savedIndex);
                    }
                    else
                    {
                        componentListSP.InsertArrayElementAtIndex(componentListSP.arraySize);
                        componentListSP.GetArrayElementAtIndex(componentListSP.arraySize - 1).objectReferenceValue = source as Component;
                    }
                    componentListSP.serializedObject.ApplyModifiedProperties();
                    EditorApplication.RepaintHierarchyWindow();//需要强刷Hierarchy
                }, comp);
            }

            return menu;
        }
        
        /// <summary>
        /// 移除空引用
        /// </summary>
        private bool RemoveNulls(SerializedProperty componentListSP)
        {
            bool hasNull = false;
            for (int i = componentListSP.arraySize - 1; i >= 0; i--)
            {
                if (componentListSP.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    componentListSP.DeleteArrayElementAtIndex(i);
                    hasNull = true;
                }
            }
            return hasNull;
        }

        //组件在已保存组件列表中的位置
        private int GetIndexFromSavedComponentList(SerializedProperty componentListSP, Component comp)
        {
            //compListSP---UIOpElement的组件
            //comp---GameObject的组件
            int index = -1;
            for (int i = 0; i < componentListSP.arraySize; i++)
            {
                Component savedComp = componentListSP.GetArrayElementAtIndex(i).objectReferenceValue as Component;
                if (savedComp.Equals(comp))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void DrawIcon(Rect selectRect, int posIndex, Texture icon)
        {
            float iconWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float iconHeight = iconWidth;
            float leftPadding = 5;
            float iconSpacing = 5;

            float iconX = selectRect.x + leftPadding + (iconWidth + iconSpacing) * posIndex;//水平居左
            float iconY = selectRect.y + (selectRect.height - iconHeight) / 2;//竖直居中

            Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);

            GUI.DrawTexture(iconRect, icon);
        }
    }
}