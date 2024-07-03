using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MFramework
{
    public class FindProjectAssets : EditorWindow
    {
        public Object obj = null;

        private List<OutputInfo> outputList = new List<OutputInfo>();
        private ObjectMode objectMode = ObjectMode.None;
        private bool isStart;
        private bool isCurrentScene;
        private bool isAllScenes;

        private System.Type scriptType = null;
        private Shader shader;
        private Material material;
        private Texture texture;
        private Sprite sprite;

        private Scene defaultScene;
        private SceneAsset s = null;

        private Vector2 scrollPos;

        private List<string> scenePaths = new List<string>();
        private List<string> sceneNames = new List<string>();

        [MenuItem("MFramework/Find Project Assets", false, 903)]
        public static void Init()
        {
            EditorWindow window = GetWindow<FindProjectAssets>(true, "FindProjectAssets", false);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("寻找Project物体在场景中的挂载", GetStyle(20, TextAnchor.MiddleCenter, EditorGUIUtility.isProSkin ? Color.white : Color.black));
                EditorGUILayout.Space(5);
                obj = EditorGUILayout.ObjectField("请拖入Project中的Object：", obj, typeof(Object), false);
                if (!isStart)
                {
                    //获取当前已加载场景(复原用)
                    for (int n = 0; n < SceneManager.sceneCount; n++)
                    {
                        Scene scene = SceneManager.GetSceneAt(n);
                        scenePaths.Add(scene.path);
                        sceneNames.Add(scene.name);
                    }

                    //功能：选中物体时直接"拖入ObjectField"
                    Object activeObj = Selection.activeObject;
                    if (activeObj && (AssetDatabase.IsForeignAsset(activeObj) || AssetDatabase.IsNativeAsset(activeObj)))
                    {
                        obj = activeObj;
                    }

                    isStart = true;
                }
            }
            EditorGUILayout.EndVertical();



            //两个按钮
            EditorGUILayout.BeginHorizontal();
            //按钮1：在当前场景中寻找
            if (GUILayout.Button("在当前场景中寻找"))
            {
                EditorGUILayout.BeginVertical();
                {
                    //初始化操作
                    isCurrentScene = true;
                    isAllScenes = false;
                    outputList.Clear();

                    //寻找操作
                    if (!obj) return;

                    GetFieldAndMode();

                    //如果为单场景模式，defaultScene是确定的
                    if (SceneManager.sceneCount == 1)
                    {
                        defaultScene = SceneManager.GetActiveScene();
                        FindAssets(false);
                    }
                    //如果为多场景模式，需要重新打开每一个场景来寻找
                    //(因为寻找使用的FindObjectsOfType()是支持多场景的，这与gameObjectIndex冲突)
                    else
                    {
                        foreach (string scenePath in scenePaths)
                        {
                            defaultScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                            FindAssets(true);
                        }
                    }

                    //结束操作---复原初始场景
                    RestoreScene();
                }
                EditorGUILayout.EndVertical();
            }
            //按钮2：在所有场景中寻找
            if (GUILayout.Button("在所有场景中寻找"))
            {
                EditorGUILayout.BeginVertical();
                {
                    //初始化操作
                    isAllScenes = true;
                    isCurrentScene = false;
                    outputList.Clear();

                    //寻找操作
                    GetFieldAndMode();

                    string[] guids = AssetDatabase.FindAssets("t:Scene");
                    //依次打开所有Project中的场景搜索
                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        s = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));
                        Scene scene = EditorSceneManager.OpenScene(path);
                        defaultScene = scene;

                        FindAssets(true);
                    }

                    //结束操作---复原初始场景
                    RestoreScene();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();



            //输出结果
            EditorGUILayout.BeginVertical();
            {
                if (isCurrentScene)
                {
                    DrawTitle();
                    DrawContent_CurrentScene();
                }
                else if (isAllScenes)
                {
                    DrawTitle();
                    DrawContent_AllScenes();
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 获取一些需要的信息，也就是字段以及ObjectMode枚举
        /// </summary>
        private void GetFieldAndMode()
        {
            //脚本情况
            MonoScript ms = obj as MonoScript;
            if (ms)
            {
                objectMode = ObjectMode.Script;
                scriptType = ms.GetClass();
            }
            //Prefab情况
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                objectMode = ObjectMode.Prefab;
            }
            //Shader情况
            shader = obj as Shader;
            if (shader)
            {
                objectMode = ObjectMode.Shader;
            }
            //Material情况
            material = obj as Material;
            if (material)
            {
                objectMode = ObjectMode.Material;
            }
            //Texture情况
            texture = obj as Texture;
            if (texture)
            {
                objectMode = ObjectMode.Texture;
            }
            //Sprite情况
            sprite = obj as Sprite;
            if (sprite)
            {
                objectMode = ObjectMode.Sprite;
            }
        }
        /// <summary>
        /// 获取资源，每找到一个正确的资源就会将其放在outputList中
        /// </summary>
        private void FindAssets(bool isOutputScene)
        {
            switch (objectMode)
            {
                case ObjectMode.Prefab:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
                        {
                            GetOutputInfo("物体名：" + gameObject.name, gameObject.name,
                                GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                        }
                    }
                    break;
                case ObjectMode.Script:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            if (component == null) return;//脚本Missing情况

                            if (component.GetType() == scriptType)
                            {
                                GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                    GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Shader:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                        if (!mr) break;
                        Material[] mats = mr.sharedMaterials;
                        foreach (var mat in mats)
                        {
                            if (mat.shader == shader)
                            {
                                GetOutputInfo("所处物体：" + gameObject.name + " 材质名：" + mat.name,
                                    gameObject.name, GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Material:
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                        if (!mr) break;
                        Material[] mats = mr.sharedMaterials;
                        foreach (var mat in mats)
                        {
                            if (mat == material)
                            {
                                GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                    GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                            }
                        }
                    }
                    break;
                case ObjectMode.Texture://对于Texture来说，太复杂了，只写了某些情况
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            RawImage rawImage;
                            Image image;
                            SpriteRenderer spriteRenderer = null;
                            if (rawImage = component as RawImage)
                            {
                                if (rawImage.texture == texture)
                                {
                                    GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (image = component as Image)
                            {
                                if (image.sprite.texture == texture)
                                {
                                    GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (spriteRenderer = component as SpriteRenderer)
                            {
                                if (spriteRenderer.sprite.texture == texture)
                                {
                                    GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                        }
                    }
                    break;
                case ObjectMode.Sprite://对于Sprite来说，太复杂了，只写了某些情况
                    foreach (GameObject gameObject in FindObjectsOfType<GameObject>(true))
                    {
                        Component[] components = gameObject.GetComponents(typeof(Component));
                        foreach (Component component in components)
                        {
                            Image image;
                            SpriteRenderer spriteRenderer = null;
                            if (image = component as Image)
                            {
                                if (image.sprite == sprite)
                                {
                                    GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                            else if (spriteRenderer = component as SpriteRenderer)
                            {
                                if (spriteRenderer.sprite == sprite)
                                {
                                    GetOutputInfo("所处物体：" + gameObject.name, gameObject.name,
                                        GetHierarchyIndex(gameObject), defaultScene.name, s, isOutputScene);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void DrawTitle()
        {
            if (outputList.Count == 0)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("未找到任何内容！！！");
                return;
            }
            else
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("搜寻结果：");
                EditorGUILayout.LabelField($"共找到{outputList.Count}处");
                EditorGUILayout.Space(3);
            }
        }
        private void DrawContent_CurrentScene()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (OutputInfo output in outputList)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(output.info);
                    if (GUILayout.Button("定位"))
                    {
                        Ping(output);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        private void DrawContent_AllScenes()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (OutputInfo output in outputList)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(output.info);
                    if (GUILayout.Button("定位"))
                    {
                        if (!Ping(output))
                        {
                            EditorGUIUtility.PingObject(output.sceneAsset);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 标记物体
        /// </summary>
        private bool Ping(OutputInfo output)
        {
            int x = (int)output.gameObjectIndex.x;
            int y = (int)output.gameObjectIndex.y;
            int z = (int)output.gameObjectIndex.z;

            //查找每一个Hierarchy下场景，找到对应场景，在场景中找到对应物体
            for (int n = 0; n < SceneManager.sceneCount; n++)
            {
                Scene scene = SceneManager.GetSceneAt(n);
                if (scene.name == output.sceneName)
                {
                    GameObject[] gos = scene.GetRootGameObjects();
                    foreach (GameObject go in gos)
                    {
                        Transform pingObject = go.transform;
                        if (x == go.transform.GetSiblingIndex())
                        {
                            for (int i = 0; i < y - 1; i++)
                            {
                                pingObject = pingObject.GetChild(0);
                            }
                            if (y != 0) pingObject = pingObject.GetChild(z);

                            EditorGUIUtility.PingObject(pingObject);//传入Transform一样会指向Hierarchy物体
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 复原场景
        /// </summary>
        private void RestoreScene()
        {
            bool isLoadFirstScene = true;
            foreach (string scenePath in scenePaths)
            {
                if (isLoadFirstScene)
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    isLoadFirstScene = false;
                }
                else
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }
            }
        }

        /// <summary>
        /// 获取物体所对应的索引
        /// </summary>
        //之所以需要Vector3是这样的：
        //首先先得确定第几个物体---x
        //然后需要知道它在该物体下第几层---y
        //最后还需要知道它在这一层下第几个---z
        private Vector3 GetHierarchyIndex(GameObject go)
        {
            Transform t = go.transform;

            //获取y
            Transform child = t;
            Transform parent = t.parent;
            int indexX = 0;
            while (parent != null)
            {
                child = parent;
                parent = child.parent;
                indexX++;
            }
            //如果y为0，这意味着它是root节点，z一定为0(不加z会得到x的结果)
            if (indexX == 0)
            {
                return new Vector3(child.GetSiblingIndex(), indexX, 0);
            }
            return new Vector3(child.GetSiblingIndex(), indexX, t.GetSiblingIndex());
        }
        private void GetOutputInfo(string info, string name, Vector2 index, string sceneName, SceneAsset sceneAsset, bool isOutputScene)
        {
            OutputInfo output = new OutputInfo();

            output.info = info;
            output.gameObjectName = name;
            output.gameObjectIndex = index;
            output.sceneName = sceneName;
            if (sceneAsset) output.sceneAsset = sceneAsset;

            if (sceneName != null && sceneName != "SampleScene" && isOutputScene)
            {
                output.info += "\t所处场景：" + sceneName;
            }

            outputList.Add(output);
        }

        private GUIStyle GetStyle(int size, TextAnchor mode, Color color)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = size;
            style.alignment = mode;
            style.normal.textColor = color;

            return style;
        }
        private GUIStyle GetStyle(int size)
        {
            GUIStyle style = new GUIStyle();

            style.fontSize = size;

            return style;
        }
    }


    /// <summary>
    /// 输出结构，其中包含了需要的信息
    /// </summary>
    //info---输出内容，可以认为是提示信息
    //sceneAsset---场景所对应的Project中资源
    //之所以获取以下几个而不是GameObject和Scene是因为场景切换会导致引用丢失
    //gameObjectName---符合要求的物体名称
    //gameObjectIndex---符合要求的物体索引
    //sceneName---物体所在场景名
    public class OutputInfo
    {
        public string info;
        public string gameObjectName;
        public Vector3 gameObjectIndex;
        public string sceneName;
        public SceneAsset sceneAsset;
    }

    //要知道有哪些内容可以被放入，只要是Project中的内容基本上都是可以放入的，但是种类繁多，需要挑选最重要的一些：
    //1.Script
    //2.Shader
    //3.Material
    //4.Prefab
    //5.Texture
    //6.Sprite
    //7.Model(没做，好像有点麻烦)
    public enum ObjectMode
    {
        Script,
        Prefab,
        Shader,
        Material,
        Texture,
        Sprite,
        None
    }
}