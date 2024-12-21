using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace MFramework
{
    public class SpriteAtlasGenerator : EditorWindow
    {
        private Object inputFolder;
        private Object outputFolder;

        private bool isFirstOpen = true;//�״δ�

        [MenuItem("MFramework/Sprite Atlas Generator", false, 910)]
        public static void Init()
        {
            var window = GetWindow<SpriteAtlasGenerator>(true, "SpriteAtlasGenerator");
            window.minSize = new Vector2(400, 400);
            window.maxSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            MEditorGUIUtility.DrawH1("ͼ���������");

            EditorGUILayout.BeginHorizontal();
            {
                MEditorGUIUtility.DrawText("����ļ��У�");

                //TODO����չΪList
                if (isFirstOpen)
                {
                    isFirstOpen = false;

                    Object selectedFolder = GetSelectedFolder();
                    if (selectedFolder != null)
                    {
                        inputFolder = selectedFolder;
                    }
                }
                inputFolder = EditorGUILayout.ObjectField(inputFolder, typeof(object), false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                MEditorGUIUtility.DrawText("����ļ��У�");
                
                outputFolder = EditorGUILayout.ObjectField(outputFolder, typeof(object), false);
                //�������FolderҪ������ѡ��
                if (outputFolder != null && !isFolder(outputFolder))
                {
                    outputFolder = null;
                    MLog.Print($"{typeof(SpriteAtlasGenerator)}��ֻ֧���ļ��У�������ѡ��", MLogType.Warning);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("����"))
            {
                if (inputFolder == null || outputFolder == null)
                {
                    MLog.Print($"{typeof(SpriteAtlasGenerator)}�����������ļ����ٽ�������", MLogType.Warning);
                }

                string name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(inputFolder));

                SpriteAtlas atlas = new SpriteAtlas();
                atlas.Add(new Object[] { inputFolder });
                SetAtlasInfo(ref atlas);
                string path = $"{AssetDatabase.GetAssetPath(outputFolder)}/Atlas_{name}.spriteatlas";
                AssetDatabase.CreateAsset(atlas, path);
            }
        }

        private Object GetSelectedFolder()
        {
            Object[] objs = MSelection.projectObjects;
            if (objs.Length != 1) return null;

            Object obj = objs[0];
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsValidFolder(path))
            {
                return obj;
            }

            return null;
        }

        private bool isFolder(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsValidFolder(path))
            {
                return true;
            }
            return false;
        }

        private void SetAtlasInfo(ref SpriteAtlas atlas)
        {
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            };
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            TextureImporterPlatformSettings platformSetting = new TextureImporterPlatformSettings()
            {

                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50,
            };

            atlas.SetIncludeInBuild(true);
            atlas.SetPackingSettings(packSetting);
            atlas.SetTextureSettings(textureSetting);
            atlas.SetPlatformSettings(platformSetting);
        }
    }
}
