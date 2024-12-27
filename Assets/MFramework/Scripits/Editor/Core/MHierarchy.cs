using UnityEditor;
using UnityEngine;

namespace MFramework
{
    [InitializeOnLoad]
    public class MHierarchy
    {
        private static GUIStyle transparentBtnStyle;
        private static GUIStyle blackStyle;

        static MHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;

            //---透明按钮Style---
            //默认完全透明，悬停时略显
            Texture2D normalTex = new Texture2D(1, 1);
            normalTex.SetPixel(0, 0, Color.clear);
            normalTex.Apply();
            Texture2D hoverTex = new Texture2D(1, 1);
            hoverTex.SetPixel(0, 0, new Color(1, 1, 1, 0.2f));
            hoverTex.Apply();
            transparentBtnStyle = new GUIStyle();
            transparentBtnStyle.normal.background = normalTex;
            transparentBtnStyle.hover.background = hoverTex;

            //---黑色按钮Style---
            normalTex = new Texture2D(1, 1);
            normalTex.SetPixel(0, 0, new Color(0.1f, 0.1f, 0.1f, 1));
            normalTex.Apply();
            hoverTex = new Texture2D(1, 1);
            hoverTex.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f, 1));
            hoverTex.Apply();
            blackStyle = new GUIStyle();
            blackStyle.normal.background = normalTex;
            blackStyle.hover.background = hoverTex;
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            //selectionRect---物体名字到最右侧，高16

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null)
            {
                //---绘制层级竖线---
                //TODO：增加不同层级渐变并对子层级位置微调
                Rect rect = new Rect(selectionRect);
                rect.xMin -= 28;
                rect.xMax = rect.xMin + 3;
                Color col = Color.white;
                EditorGUI.DrawRect(rect, col);

                //---绘制激活按钮---
                rect = new Rect(selectionRect);
                rect.xMax += 16;
                rect.xMin = rect.xMax - 16;

                bool preState = go.activeSelf;
                bool nowState = GUI.Toggle(rect, preState, "");
                if (preState != nowState)
                {
                    go.SetActive(nowState);
                    EditorUtility.SetDirty(go);
                }

                //---绘制图标更改按钮(透明)---
                rect = new Rect(selectionRect);
                rect.xMin += 0;
                rect.xMax = rect.xMin + 15;
                col = new Color(0.25f, 0.25f, 0.25f);
                EditorGUI.DrawRect(rect, col);

                rect.xMax += 1;
                GUIContent content = EditorGUIUtility.ObjectContent(go, typeof(GameObject));
                Texture tex = content.image;
                GUI.DrawTexture(rect, tex);

                //TODO：不支持多屏(可能是我把显示器检测关了？)
                if (GUI.Button(rect, "", transparentBtnStyle))
                {
                    //这些参数都很神秘：
                    //Event.current.mousePosition---x起始点为Scene名上
                    //GUIUtility.GUIToScreenPoint()---加上最上到面板名下
                    //那么两者结合的screenMousePosition+Scene名高(16)为点击的下侧
                    Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition) + new Vector2(0, 16);//获取点击位置
                    Vector2 windowPosition = new Vector2(rect.x + 24, screenMousePosition.y + 12);
                    ChooseIconPopup.Show(windowPosition, go);
                }
            }
        }

        public class ChooseIconPopup : EditorWindow
        {
            public static GameObject go;
            public static GUIContent[] iconContents;
            public static GUIContent returnContent, crossContent;

            private static Rect windowRect = new Rect(0, 0, 250, 120);
            private static Rect titleRect = new Rect(0, 0, 250, 16);
            private static Rect exitRect = new Rect(250 - 10 - 3, 3, 10, 10);
            private static Color bgColor = new Color(.1f, .1f, .1f);

            private static ChooseIconPopup window;
            public static void Show(Vector2 position, GameObject obj)
            {
                //已打开则重新开启
                if (window != null) window.Close();

                go = obj;

                iconContents = new GUIContent[50];
                for (int i = 0; i < iconContents.Length; i++)
                {
                    Texture texture = MTextureLibrary.LoadTexture($"HierarchyIcons/Icon_{i}");
                    if (texture == null)
                        break;
                    iconContents[i] = new GUIContent(texture);
                }
                returnContent = new GUIContent(MTextureLibrary.LoadTexture("CommonIcons/Return"));
                crossContent = new GUIContent(MTextureLibrary.LoadTexture("CommonIcons/Cross"));

                window = CreateInstance<ChooseIconPopup>();
                window.position = new Rect(position, new Vector2(250, 120));
                window.ShowPopup();
            }

            private void OnGUI()
            {
                if (go == null)
                {
                    window.Close();
                    return;
                }

                EditorGUI.DrawRect(windowRect, bgColor);
                EditorGUI.DrawRect(titleRect, new Color(0.35f, 0.35f, 0.35f));
                GUILayout.Label($"Select Icon for {go.name}", MEditorGUIStyleUtility.LeftH3Style);
                if (GUI.Button(exitRect, crossContent, blackStyle))
                {
                    Close();
                }

                EditorGUILayout.Space(5);

                if (GUILayout.Button(returnContent, transparentBtnStyle, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    EditorGUIUtility.SetIconForObject(go, null);
                    Close();
                }

                EditorGUILayout.Space(5);

                bool isFinished = false;
                int row = 0;
                while (!isFinished)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        //一行12个
                        for (int col = 0; col < 12; col++)
                        {
                            int i = row * 12 + col;
                            if (i >= iconContents.Length || iconContents[i] == null)
                            {
                                isFinished = true;
                                break;
                            }
                            if (GUILayout.Button(iconContents[i], transparentBtnStyle, GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                //重绘
                                EditorGUIUtility.SetIconForObject(go, iconContents[i].image as Texture2D);
                                Close();
                            }
                        }
                        row++;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                window.Repaint();
            }

            private void OnLostFocus()
            {
                Close();
            }
        }
    }
}
