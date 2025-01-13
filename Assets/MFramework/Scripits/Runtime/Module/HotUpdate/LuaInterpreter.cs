using UnityEngine;
using XLua;

namespace MFramework
{
    public class LuaInterpreter : MonoSingleton<LuaInterpreter>
    {
        private LuaEnv luaEnv = null;
        private bool _bIsUseLocalFile = false;

        private const string LuaFileSuffix = ".lua.txt";

        void Awake()
        {
            if (luaEnv == null)
            {
                luaEnv = new LuaEnv();
                luaEnv.AddLoader(CustomLuaLoader);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (luaEnv != null)
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
        }

        public void RequireLua(string sLuaName)
        {
            if (luaEnv != null)
            {
                luaEnv.DoString(string.Format("require '{0}'", sLuaName));
            }
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
#if UNITY_EDITOR && _bIsUseLocalFile
            string sLuaPath = Application.dataPath + @"/Scripts/Lua/" + filePath + ".lua.txt";
            Debug.Log("Editor ∂¡»°Lua: " + sLuaPath);
        
            string sAllStr = File.ReadAllText(sLuaPath);
            return System.Text.Encoding.UTF8.GetBytes(sAllStr);
#else
            IResource luaResource = MResourceManager.Instance.LoadByName($"{filePath}{LuaFileSuffix}", false);
            TextAsset textAsset = luaResource.GetAsset() as TextAsset;

            return System.Text.Encoding.UTF8.GetBytes(textAsset.text);
#endif
        }
    }
}
