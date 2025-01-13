using System;
using System.Reflection;
using UnityEngine;

namespace MFramework
{
    //TODO：使用反射，真的只能用反射吗？
    public class MLuaInterpreter : MonoSingleton<MLuaInterpreter>
    {
        private object luaEnv = null;

        private const string LuaFileSuffix = ".lua.txt";

        private Type luaEnvType;
        private Type luaTableType;
        private MethodInfo disposeMethod;
        private MethodInfo doStringMethod;

        void Awake()
        {
            if (luaEnv == null)
            {
                string CSAssemblyPath = GetAssemblyCSharpDLLName();
                Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

                luaEnvType = assembly.GetType("XLua.LuaEnv");
                luaTableType = assembly.GetType("XLua.LuaTable");
                luaEnv = Activator.CreateInstance(luaEnvType);
                //需要选择无参Dispose()
                disposeMethod = luaEnv.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                doStringMethod = luaEnv.GetType().GetMethod("DoString", new Type[] { typeof(string), typeof(string), luaTableType });

                //生成委托
                Type customLoaderType = luaEnvType.GetNestedType("CustomLoader");
                MethodInfo methodInfo = typeof(MLuaInterpreter).GetMethod("CustomLuaLoader", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate loaderDelegate = Delegate.CreateDelegate(customLoaderType, this, methodInfo);
                //调用AddLoader(形参为CustomLoader委托)
                MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
                addLoaderMethod.Invoke(luaEnv, new object[] { loaderDelegate });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (luaEnv != null)
            {
                disposeMethod.Invoke(luaEnv, null);

                luaEnv = null;
            }
        }

        public void RequireLua(string sLuaName)
        {
            if (luaEnv != null)
            {
                doStringMethod.Invoke(luaEnv, new object[] { $"require '{sLuaName}'", "chunk", null });
            }
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
#if UNITY_EDITOR && false
            TODO：添加本地测试版本？
#else
            IResource luaResource = MResourceManager.Instance.LoadByName($"{filePath}{LuaFileSuffix}", false);
            TextAsset textAsset = luaResource.GetAsset() as TextAsset;

            return System.Text.Encoding.UTF8.GetBytes(textAsset.text);
#endif
        }

        private string GetAssemblyCSharpDLLName()
        {
#if UNITY_EDITOR
            return $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
#elif UNITY_STANDALONE
            return $"{Application.dataPath}/Managed/Assembly-CSharp.dll";
#else
            MLog.Print($"{typeof(LuaInterpreter)}：暂未实现", MLogType.Error);
#endif
        }
    }
}
