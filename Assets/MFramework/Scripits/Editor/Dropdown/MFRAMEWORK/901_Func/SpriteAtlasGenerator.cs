using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace MFramework
{
    public enum AtlasGeneratorMode
    {
        File,//单一文件打包
        Directory,//以文件夹打包
        All//打包所有内部文件夹
    }

    public class SpriteAtlasGenerator : EditorWindow
    {
        private Object inputObj;
        private Object outputFolder;

        private bool isFirstOpen = true;//首次打开
        private Vector2 settingPos;

        //===设置===
        //我的设置
        private string prefix = "Atlas";
        private AtlasGeneratorMode mode = AtlasGeneratorMode.Directory;
        //基本设置
        //private bool _includeInBuild = true;//开启即可，并不需要LateBinding
        //打包设置
        private int _blockOffset = 0;
        private int _padding = 2;
        private bool _allowRotation = false;
        private bool _tightPacking = false;
        private bool _alphaDilation = false;
        //贴图设置
        private bool _readable = false;
        private bool _generateMipMaps = false;
        private bool _sRGB = true;
        private FilterMode _filterMode = FilterMode.Bilinear;
        //平台设置
        private int _maxTextureSize = 2048;
        private TextureImporterFormat _textureImporterFormat = TextureImporterFormat.Automatic;
        private TextureImporterCompression _textureImporterCompression = TextureImporterCompression.Compressed;
        private bool _crunchedCompression = true;
        private int _compressionQuality = 50;

        [MenuItem("MFramework/Sprite Atlas Generator", false, 910)]
        public static void Init()
        {
            var window = GetWindow<SpriteAtlasGenerator>(true, "SpriteAtlasGenerator");
            window.minSize = new Vector2(400, 560);
            window.maxSize = new Vector2(400, 560);
            window.Show();
        }

        private void OnGUI()
        {
            MEditorGUIUtility.DrawH1("图集打包工具");
            {
                DrawInputOutputSettings();

                EditorGUILayout.Space(20);

                MEditorGUIUtility.DrawLeftH2("设置");
                {
                    settingPos = EditorGUILayout.BeginScrollView(settingPos);
                    {
                        DrawMainSettings();
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space(10);

                DrawGenerateBtn();
            }
        }

        private void DrawInputOutputSettings()
        {
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("打包内容：");

                GUILayout.FlexibleSpace();

                //TODO：扩展为List
                if (isFirstOpen)
                {
                    isFirstOpen = false;

                    Object obj = GetSelectedProjectObject();
                    obj = CheckFolderOrSprite(obj);
                    if (obj != null)
                    {
                        inputObj = obj;
                    }
                }
                inputObj = EditorGUILayout.ObjectField(inputObj, typeof(object), false, GUILayout.Width(110));
                inputObj = CheckFolderOrSprite(inputObj);

                mode = (AtlasGeneratorMode)EditorGUILayout.EnumPopup(mode, GUILayout.Width(75));
            });

            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("输出文件夹：");

                outputFolder = EditorGUILayout.ObjectField(outputFolder, typeof(object), false);
                outputFolder = CheckFolder(outputFolder);
            });
        }

        private void DrawMainSettings()
        {
            //基本设置
            MEditorGUIUtility.DrawLeftH3("基本设置");
            DrawBaseSettings();
            //打包设置
            MEditorGUIUtility.DrawLeftH3("打包设置");
            DrawPackingSettings();
            //贴图设置
            MEditorGUIUtility.DrawLeftH3("贴图设置");
            DrawTextureSettings();
            //平台设置
            MEditorGUIUtility.DrawLeftH3("平台设置");
            DrawPlatformSettings();
        }
        private void DrawBaseSettings()
        {

            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("前缀名：", GUILayout.Width(50));
                prefix = EditorGUILayout.TextField(prefix);
                MEditorGUIUtility.DrawText($"例：{prefix}_XXX");
            });
        }
        private void DrawPackingSettings()
        {
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("块间距BlockOffset：", "指的是一组Sprite的周围添加间距");
                GUILayout.FlexibleSpace();
                _blockOffset = EditorGUILayout.IntField(_blockOffset, GUILayout.Width(30));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("精灵间距Padding：", "指的是每个Sprite的周围添加间距");
                GUILayout.FlexibleSpace();
                _padding = EditorGUILayout.IntField(_padding, GUILayout.Width(30));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否允许旋转AllowRotation：");
                GUILayout.FlexibleSpace();
                _allowRotation = EditorGUILayout.Toggle(_allowRotation, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否紧凑打包TightPacking：");
                GUILayout.FlexibleSpace();
                _tightPacking = EditorGUILayout.Toggle(_tightPacking, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("设置边界Alpha为0(AlphaDilation)：");
                GUILayout.FlexibleSpace();
                _alphaDilation = EditorGUILayout.Toggle(_alphaDilation, GUILayout.Width(15));
            });
        }
        private void DrawTextureSettings()
        {
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否可读Readable：");
                GUILayout.FlexibleSpace();
                _readable = EditorGUILayout.Toggle(_readable, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否生成Mipmaps：");
                GUILayout.FlexibleSpace();
                _generateMipMaps = EditorGUILayout.Toggle(_generateMipMaps, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否使用sRGB：");
                GUILayout.FlexibleSpace();
                _sRGB = EditorGUILayout.Toggle(_sRGB, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("过滤模式FilterMode：");
                GUILayout.FlexibleSpace();
                _filterMode = (FilterMode)EditorGUILayout.EnumPopup(_filterMode, GUILayout.Width(100));
            });
        }
        private void DrawPlatformSettings()
        {
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("最大贴图大小MaxTextureSize：");
                GUILayout.FlexibleSpace();
                _maxTextureSize = EditorGUILayout.IntField(_maxTextureSize, GUILayout.Width(45));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("格式Format：");
                GUILayout.FlexibleSpace();
                _textureImporterFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup(_textureImporterFormat, GUILayout.Width(100));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("压缩格式Compression：");
                GUILayout.FlexibleSpace();
                _textureImporterCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup(_textureImporterCompression, GUILayout.Width(100));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("是否开启有损压缩CrunchedCompression：");
                GUILayout.FlexibleSpace();
                _crunchedCompression = EditorGUILayout.Toggle(_crunchedCompression, GUILayout.Width(15));
            });
            MEditorGUIUtility.Horizontal(() =>
            {
                MEditorGUIUtility.DrawText("有损压缩质量CompressionQuality：");
                GUILayout.FlexibleSpace();
                _compressionQuality = EditorGUILayout.IntField(_compressionQuality, GUILayout.Width(30));
            });
        }

        private void DrawGenerateBtn()
        {
            if (GUILayout.Button("生成"))
            {
                if (inputObj == null || outputFolder == null)
                {
                    MLog.Print($"{typeof(SpriteAtlasGenerator)}：请先填入文件夹再进行生成", MLogType.Warning);
                }

                switch (mode)
                {
                    case AtlasGeneratorMode.File:
                    {
                        if (!IsSprite(inputObj))
                        {
                            MLog.Print($"{typeof(SpriteAtlasGenerator)}：File模式请传入Sprite", MLogType.Warning);
                            return;
                        }

                        string name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(inputObj));
                        CreateAtlas(name, inputObj);

                        break;
                    }
                    case AtlasGeneratorMode.Directory:
                    {
                        if (!IsFolder(inputObj))
                        {
                            MLog.Print($"{typeof(SpriteAtlasGenerator)}：Directory模式请传入文件夹", MLogType.Warning);
                            return;
                        }

                        string name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(inputObj));
                        CreateAtlas(name, inputObj);

                        break;
                    }
                    case AtlasGeneratorMode.All:
                    {
                        if (!IsFolder(inputObj))
                        {
                            MLog.Print($"{typeof(SpriteAtlasGenerator)}：All模式请传入文件夹", MLogType.Warning);
                            return;
                        }

                        //All模式收集逻辑：
                        //收集最深处文件夹
                        //TODO：可以考虑收集孤立Sprite
                        List<string> deepestFolders = new List<string>();
                        string path = AssetDatabase.GetAssetPath(inputObj);
                        GetSubFolders(path, deepestFolders);

                        Object[] objs = new Object[deepestFolders.Count];
                        int n = 0;
                        foreach (var folderPath in deepestFolders)
                        {
                            var folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                            objs[n++] = folder;

                            string name = Path.GetFileNameWithoutExtension(folderPath);
                            CreateAtlas(name, folder);
                        }
                        //TODO：考虑是一个Atlas中添加多个文件夹，还是目前的多个Atlas

                        break;
                    }
                }

                MLog.Print($"{typeof(SpriteAtlasGenerator)}：已生成");
            }
        }

        private void GetSubFolders(string path, List<string> deepestFolders)
        {
            string[] subPaths = AssetDatabase.GetSubFolders(path);

            if (subPaths.Length == 0)
            {
                //说明为最深文件夹，可以收集
                string[] assets = AssetDatabase.FindAssets("", new[] { path });
                if (assets.Length != 0)//有资源才添加
                {
                    deepestFolders.Add(path);
                }
            }
            else
            {
                foreach (var subPath in subPaths)
                {
                    GetSubFolders(subPath, deepestFolders);
                }
            }
        }

        private void CreateAtlas(string name, params Object[] objs)
        {
            SpriteAtlas atlas = new SpriteAtlas();
            atlas.Add(objs);
            SetAtlasInfo(ref atlas);
            string path = $"{AssetDatabase.GetAssetPath(outputFolder)}/Atlas_{name}.spriteatlas";
            AssetDatabase.CreateAsset(atlas, path);
        }

        private void SetAtlasInfo(ref SpriteAtlas atlas)
        {
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = _blockOffset,
                enableRotation = _allowRotation,
                enableTightPacking = _tightPacking,
                enableAlphaDilation = _alphaDilation,
                padding = _padding,
            };
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = _readable,
                generateMipMaps = _generateMipMaps,
                sRGB = _sRGB,
                filterMode = _filterMode,
            };
            TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
            {

                maxTextureSize = _maxTextureSize,
                format = _textureImporterFormat,
                crunchedCompression = _crunchedCompression,
                textureCompression = _textureImporterCompression,
                compressionQuality = _compressionQuality,
            };

            atlas.SetIncludeInBuild(true);
            atlas.SetPackingSettings(packSetting);
            atlas.SetTextureSettings(textureSetting);
            atlas.SetPlatformSettings(platformSetting);
        }

        private bool IsTexture2D(Object obj)
        {
            return obj is Texture2D;
        }
        private bool IsSprite(Object obj)
        {
            return obj is Sprite;
        }
        private bool IsFolder(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsValidFolder(path))
            {
                return true;
            }
            return false;
        }

        private Object CheckFolderOrSprite(Object obj)
        {
            if (obj == null) return null;

            if (IsFolder(obj) || IsSprite(obj))
            {
                return obj;
            }
            else if (IsTexture2D(obj))
            {
                Sprite sprite = GetSprite((Texture2D)obj);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            MLog.Print($"{typeof(SpriteAtlasGenerator)}：只支持文件夹/精灵，请重新选择", MLogType.Warning);
            return null;
        }
        private Object CheckFolder(Object obj)
        {
            if (obj == null) return null;

            if (IsFolder(obj))
            {
                return obj;
            }

            MLog.Print($"{typeof(SpriteAtlasGenerator)}：只支持文件夹，请重新选择", MLogType.Warning);
            return null;
        }

        private Sprite GetSprite(Texture2D texture)
        {
            if (IsTexture2D(texture))
            {
                string path = AssetDatabase.GetAssetPath(texture);
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                //仅对Texture2D与Sprite对应关系情况设置
                //Tip：对于Sprite模式，可以获取到Texture2D资源与Sprite资源
                if (subAssets.Length == 2)
                {
                    if (subAssets[1] is Sprite sprite) return sprite;
                }
            }
            return null;
        }

        private Object GetSelectedProjectObject()
        {
            Object[] objs = MSelection.projectObjects;
            if (objs.Length != 1) return null;

            return objs[0];
        }
    }
}
