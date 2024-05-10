using MFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MFramework
{
    [CustomEditor(typeof(UIViewBehaviour))]
    public abstract class UIViewBehaviourEditor : Editor
    {
        //CompCollectionList重绘
        /*
        //ReorderableList---可交换列表(像ProjectSettings--->ScriptExecutionOrder中的列表)
        protected ReorderableList compCollectionListRL;

        private HashSet<GameObject> m_CurObjSet_ForSort;
        private List<GameObject> m_CurObjList_ForSort;

        protected virtual void OnEnable()
        {
            compCollectionListRL = CreateReorderableList(serializedObject.FindProperty("compCollection"));
        }

        #region 收集栏
        protected void DrawUICompCollectionListRL()
        {
            compCollectionListRL.DoLayoutList();
        }

        private ReorderableList CreateReorderableList(SerializedProperty compCollectionListSP)
        {
            ReorderableList reorderableList = new ReorderableList(serializedObject, compCollectionListSP)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.2f//一个元素的层高(与内部GUI无关)
            };

            //每个元素的绘制方式
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight * 0.1f;//同步向下移动一点

                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };

            //标题的绘制方式
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "OpElementList");
            };

            //重写原因（相对ReorderableList源码中的默认实现DrawFooter）：
            //1、删除 list.displayAdd 和 list.displayRemove 的判断逻辑。（不需要，要让+-按钮永远保留）
            //2、删除 增加+按钮时的 onAddDropdownCallback相关逻辑。（不需要）
            //4、增加一个清空按钮。
            //3、增加一个整理按钮。
            reorderableList.drawFooterCallback = (Rect rect) =>
            {
                ReorderableList list = reorderableList;
                ReorderableList.Defaults defaults = ReorderableList.defaultBehaviours;

                float rightMargin = 10f;
                float leftPading = 10f;
                float rightPading = 10f;
                float singleWidth = 25f;
                float singleHeight = 16f;
                float spacing = 5f;
                int btnsCount = 4;

                float rightEdge = rect.xMax - rightMargin;
                float btnsWidth = leftPading + rightPading + singleWidth * btnsCount + spacing * (btnsCount - 1);
                Rect btnsRect = new Rect(rightEdge - btnsWidth, rect.y, btnsWidth, rect.height);

                Rect addRect = new Rect(btnsRect.x + leftPading, btnsRect.y, singleWidth, singleHeight);
                Rect removeRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 1, btnsRect.y, singleWidth, singleHeight);
                Rect trashRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 2, btnsRect.y, singleWidth, singleHeight);
                Rect refreshRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 3, btnsRect.y, singleWidth, singleHeight);

                //Tip：大部分都是ReorderableList的源码

                //当为重绘状态时，把页脚背景根据btnsRect绘制一下
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    defaults.footerBackground.Draw(btnsRect, false, false, false, false);
                }
                //自行添加onCanAddCallback且结果为false时禁用(就是发生了不能按加号按钮的情况)
                using (new EditorGUI.DisabledScope(list.onCanAddCallback != null && !list.onCanAddCallback(list)))
                {
                    if (GUI.Button(addRect, defaults.iconToolbarPlus, defaults.preButton))
                    {
                        //defaults.DoAddButton(list);
                        list.onAddCallback(list);
                    }
                }
                //减号
                using (new EditorGUI.DisabledScope(list.index < 0 || list.index >= list.count || (list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list))))
                {
                    if (GUI.Button(removeRect, defaults.iconToolbarMinus, defaults.preButton))
                    {
                        defaults.DoRemoveButton(list);
                        //list.onRemoveCallback(list);
                    }
                }
                //删除
                using (new EditorGUI.DisabledScope(list.count <= 0))
                {
                    Texture icon = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
                    if (GUI.Button(trashRect, new GUIContent(icon), defaults.preButton))
                    {
                        TrashCompCollectionList(list);
                    }
                }
                //刷新
                using (new EditorGUI.DisabledScope(list.count <= 0))
                {
                    Texture icon = EditorGUIUtility.IconContent("TreeEditor.Refresh").image;
                    if (GUI.Button(refreshRect, new GUIContent(icon), defaults.preButton))
                    {
                        RefreshCompCollectionList(list);
                    }
                }
            };

            //重写原因（相对ReorderableList源码中的默认实现DoAddButton）：
            //1、去掉元素为 IList时，实际类型不明的丑陋构造方式，只需将list长度自增即可。
            //2、新增的元素，需要清空。
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                SerializedProperty listSP = list.serializedProperty;
                listSP.arraySize++;
                list.index = listSP.arraySize - 1;//看起来像添加元素

                //找到新增元素的target(就是放入物体的那个)，并清除引用
                SerializedProperty newElementSP = listSP.GetArrayElementAtIndex(listSP.arraySize - 1);
                SerializedProperty targetSP = newElementSP.FindPropertyRelative("target");
                targetSP.objectReferenceValue = null;
                newElementSP.serializedObject.ApplyModifiedProperties();
            };

            return reorderableList;
        }

        private void TrashCompCollectionList(ReorderableList list)
        {
            SerializedProperty listSP = list.serializedProperty;
            listSP.ClearArray();

            listSP.serializedObject.ApplyModifiedProperties();
        }

        private void RefreshCompCollectionList(ReorderableList list)
        {
            SerializedProperty listSP = list.serializedProperty;

            //merge components From I to J.
            //将 在I中且不在J中的component加入J，然后将I的Target置为Null。
            for (int i = 1; i < listSP.arraySize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    SerializedProperty elementSP_I = listSP.GetArrayElementAtIndex(i);
                    SerializedProperty elementSP_J = listSP.GetArrayElementAtIndex(j);

                    SerializedProperty targetSP_I = elementSP_I.FindPropertyRelative("m_Target");
                    SerializedProperty targetSP_J = elementSP_J.FindPropertyRelative("m_Target");

                    if (targetSP_I.objectReferenceValue == null) { continue; }
                    if (targetSP_J.objectReferenceValue == null) { continue; }

                    if (!targetSP_I.objectReferenceValue.Equals(targetSP_J.objectReferenceValue)) { continue; }

                    SerializedProperty componentListSP_I = elementSP_I.FindPropertyRelative("m_ComponentList");
                    SerializedProperty componentListSP_J = elementSP_J.FindPropertyRelative("m_ComponentList");

                    for (int m = 0; m < componentListSP_I.arraySize; m++)
                    {
                        bool isExistInJ = false;
                        SerializedProperty componentSP_IM = componentListSP_I.GetArrayElementAtIndex(m);
                        for (int n = 0; n < componentListSP_J.arraySize; n++)
                        {
                            SerializedProperty componentSP_JN = componentListSP_J.GetArrayElementAtIndex(n);
                            if (componentSP_IM.objectReferenceValue.Equals(componentSP_JN.objectReferenceValue))
                            {
                                isExistInJ = true;
                                break;
                            }
                        }
                        if (!isExistInJ)
                        {
                            componentListSP_J.InsertArrayElementAtIndex(componentListSP_J.arraySize);
                            componentListSP_J.GetArrayElementAtIndex(componentListSP_J.arraySize - 1).objectReferenceValue = componentSP_IM.objectReferenceValue;
                        }
                    }
                    targetSP_I.objectReferenceValue = null;
                }
            }

            //移除所有target为Null 或 componentList 为空的元素
            for (int i = listSP.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty elementSP = listSP.GetArrayElementAtIndex(i);
                SerializedProperty targetSP = elementSP.FindPropertyRelative("m_Target");
                SerializedProperty componentListSP = elementSP.FindPropertyRelative("m_ComponentList");

                if (targetSP.objectReferenceValue == null || componentListSP.arraySize == 0)
                {
                    //注意：这里删除直接DeleteArrayElementAtIndex即可。（不要先置为null，会报错）
                    listSP.DeleteArrayElementAtIndex(i);
                }
            }

            #region 按照 Hierarchy 从上到下（深度优先）顺序排序
            //将待操作元素加入 HashSet，用于从所有元素中筛选操作元素
            if (m_CurObjSet_ForSort == null) { m_CurObjSet_ForSort = new HashSet<GameObject>(); }
            m_CurObjSet_ForSort.Clear();
            for (int i = 0; i < listSP.arraySize; i++)
            {
                SerializedProperty elementSP = listSP.GetArrayElementAtIndex(i);
                SerializedProperty targetSP = elementSP.FindPropertyRelative("m_Target");
                m_CurObjSet_ForSort.Add((GameObject)targetSP.objectReferenceValue);
            }

            if (m_CurObjList_ForSort == null) { m_CurObjList_ForSort = new List<GameObject>(); }
            m_CurObjList_ForSort.Clear();

            //从上到下（深度优先）遍历子物体，筛选待操作元素，加入有序列表
            GameObject root = ((Component)target).gameObject;
            MEditorUtitlity.DFS(root, (go) =>
            {
                if (!m_CurObjSet_ForSort.Contains(go)) { return; }
                m_CurObjList_ForSort.Add(go);
            });

            //倒序遍历有序列表，依次移动元素至首位置
            for (int i = m_CurObjList_ForSort.Count - 1; i >= 0; i--)
            {
                GameObject go = m_CurObjList_ForSort[i];
                int srcIndex = -1;
                for (int j = 0; j < listSP.arraySize; j++)
                {
                    SerializedProperty elementSP = listSP.GetArrayElementAtIndex(j);
                    SerializedProperty targetSP = elementSP.FindPropertyRelative("m_Target");
                    if (((GameObject)targetSP.objectReferenceValue).Equals(go))
                    {
                        srcIndex = j;
                        break;
                    }
                }
                Debug.Assert(srcIndex >= 0);
                listSP.MoveArrayElement(srcIndex, 0);
            }
            #endregion

            listSP.serializedObject.ApplyModifiedProperties();
        }
        #endregion
        */

        #region 代码生成
        protected void DrawExportButton()
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
                    //RefreshCompCollectionList(compCollectionListRL);
                    //GenerateUIBaseCode();
                }

                if (GUILayout.Button("输出核心文件"))
                {
                    if (Application.isPlaying)
                    {
                        MLog.Print("UI：请在非运行时导出", MLogType.Warning);
                        return;
                    }
                    //RefreshCompCollectionList(compCollectionListRL);
                    //GenerateUITempCode();
                }
            }
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}