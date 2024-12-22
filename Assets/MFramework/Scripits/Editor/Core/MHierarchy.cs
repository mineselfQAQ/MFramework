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
                rect.xMin -= 0;
                rect.xMax = rect.xMin + 16;
                col = new Color(0.25f, 0.25f, 0.25f);
                EditorGUI.DrawRect(rect, col);

                GUIContent content = EditorGUIUtility.ObjectContent(go, typeof(GameObject));
                Texture tex = content.image;
                GUI.DrawTexture(rect, tex);

                if (GUI.Button(rect, "", transparentBtnStyle))
                {
                    Vector2 screenMousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                    float differenceY = screenMousePosition.y - Event.current.mousePosition.y;
                    float iconScreenY = rect.y + differenceY;
                    Vector2 windowPosition = new Vector2(rect.x + 30, iconScreenY + 16);
                    ChooseIconPopup.Show(windowPosition, go);
                }
            }
        }

        //TODO：双屏点击位置检测不正确
        public class ChooseIconPopup : EditorWindow
        {
            public float size = 20;

            public static GameObject go;
            public static GUIContent[] iconContents;
            public static GUIContent returnContent, crossContent;

            private static GUIStyle textStyle = new GUIStyle();
            private static Rect windowRect, titleRect, exitRect;

            private static Color bgColor = new Color(.1f, .1f, .1f);
            private static bool openAnim;
            private static Vector2 oPos;

            private static ChooseIconPopup window;
            public static void Show(Vector2 position, GameObject obj)
            {
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
                textStyle.fontSize = 12;
                textStyle.normal.textColor = Color.white;

                window = CreateInstance<ChooseIconPopup>();
                window.position = new Rect(position, new Vector2(250, 120));
                window.ShowPopup();

                windowRect = new Rect(0, 0, 250, 120);
                titleRect = new Rect(0, 0, 250, 16);
                exitRect = new Rect(250 - 10 - 3, 3, 10, 10);
                bgColor = new Color(.1f, .1f, .1f);

                oPos = position;
                openAnim = true;
                t = 0;
            }
            private static float t;
            void OnGUI()
            {
                if (go == null)
                {
                    window.Close();
                    return;
                }

                EditorGUI.DrawRect(windowRect, bgColor);
                EditorGUI.DrawRect(titleRect, new Color(.35f, .35f, .35f));
                textStyle.fontStyle = FontStyle.Bold;
                GUILayout.Label($"Select Icon for {go.name}", textStyle);
                textStyle.fontStyle = FontStyle.Normal;
                if (GUI.Button(exitRect, crossContent, blackStyle))
                {
                    Close();
                }
                GUILayout.Space(6);

                if (GUILayout.Button(returnContent, transparentBtnStyle, GUILayout.Width(size), GUILayout.Height(size)))
                {
                    EditorGUIUtility.SetIconForObject(go, null);
                    Close();
                }

                bool finished = false;
                int count = 0;
                while (!finished)
                {

                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < 12; j++)
                    {
                        int i = count * 12 + j;
                        if (i >= iconContents.Length || iconContents[i] == null)
                        {
                            finished = true;
                            break;
                        }
                        if (GUILayout.Button(iconContents[i], transparentBtnStyle, GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            EditorGUIUtility.SetIconForObject(go, iconContents[i].image as Texture2D);
                            Close();
                        }
                    }
                    count++;
                    EditorGUILayout.EndHorizontal();
                }
                window.Repaint();

            }
            void OnLostFocus()
            {
                Close();
            }
        }
    }
}
