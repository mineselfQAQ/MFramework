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
                // ��ȡ XLua �� LuaEnv ����
                string CSAssemblyPath = $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
                Assembly assembly = Assembly.LoadFile(CSAssemblyPath);
                Type luaEnvType = assembly.GetType("XLua.LuaEnv");

                // ���� LuaEnv ʵ��
                luaEnv = Activator.CreateInstance(luaEnvType);

                // ���Խ�һ����ȡ LuaEnv ��ķ���
                MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
                addLoaderMethod.Invoke(luaEnv, new object[] { ()CustomLuaLoader });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (luaEnv != null)
            {
                // ��ȡ LuaEnv �� Dispose ����������
                MethodInfo disposeMethod = luaEnv.GetType().GetMethod("Dispose");
                disposeMethod.Invoke(luaEnv, null);

                luaEnv = null;
            }
        }

        public void RequireLua(string sLuaName)
        {
            if (luaEnv != null)
            {
                // ��ȡ DoString ����������
                MethodInfo doStringMethod = luaEnv.GetType().GetMethod("DoString");
                doStringMethod.Invoke(luaEnv, new object[] { $"require '{sLuaName}'" });
            }
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
#if UNITY_EDITOR && _bIsUseLocalFile
            string sLuaPath = Application.dataPath + @"/Scripts/Lua/" + filePath + ".lua.txt";
            Debug.Log("Editor ��ȡLua: " + sLuaPath);
        
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
