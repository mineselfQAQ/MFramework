using System;
using System.Reflection;
using UnityEngine;

namespace MFramework
{
    public class LuaInterpreter : MonoSingleton<LuaInterpreter>
    {
        private object luaEnv = null;
        private bool _bIsUseLocalFile = false;

        private const string LuaFileSuffix = ".lua.txt";

        void Awake()
        {
            if (luaEnv == null)
            {
                // 获取 XLua 的 LuaEnv 类型
                string CSAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
                Assembly assembly = Assembly.LoadFile(CSAssemblyPath);
                Type luaEnvType = assembly.GetType("XLua.LuaEnv");

                // 创建 LuaEnv 实例
                luaEnv = Activator.CreateInstance(luaEnvType);

                // 可以进一步获取 LuaEnv 类的方法
                MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
                addLoaderMethod.Invoke(luaEnv, new object[] { ()CustomLuaLoader });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (luaEnv != null)
            {
                // 获取 LuaEnv 的 Dispose 方法并调用
                MethodInfo disposeMethod = luaEnv.GetType().GetMethod("Dispose");
                disposeMethod.Invoke(luaEnv, null);

                luaEnv = null;
            }
        }

        public void RequireLua(string sLuaName)
        {
            if (luaEnv != null)
            {
                // 获取 DoString 方法并调用
                MethodInfo doStringMethod = luaEnv.GetType().GetMethod("DoString");
                doStringMethod.Invoke(luaEnv, new object[] { $"require '{sLuaName}'" });
            }
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
#if UNITY_EDITOR && _bIsUseLocalFile
            string sLuaPath = Application.dataPath + @"/Scripts/Lua/" + filePath + ".lua.txt";
            Debug.Log("Editor 读取Lua: " + sLuaPath);
        
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
