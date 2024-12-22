using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MFramework
{
    public static class MTextureLibrary
    {
        private static Texture2D m_LOGOTex;

        public static Texture2D LOGOTex
        {
            get
            {
                if(m_LOGOTex == null)
                {
                    m_LOGOTex = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorResourcesPath.LOGOPath);
                }
                return m_LOGOTex;
            }
        }

        private static Dictionary<string, Texture2D> cachedTexDic = new Dictionary<string, Texture2D>();

        public static Texture2D LoadTexture(string name)
        {
            string path = $"{EditorResourcesPath.BuiltInResourcesFolder}/{name}.png";
            if (!cachedTexDic.ContainsKey(path))
            {
                cachedTexDic.Add(path, AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
            return cachedTexDic[path];
        }
    }
}
