using System;
using System.IO;
using System.Reflection;
using MFramework.Core;
using UnityEngine;

namespace MFramework
{
    public class MLuaInterpreter : IDisposable
    {
        private const string LuaFileSuffix = ".lua.txt";

        private readonly ABRuntimeState _runtimeState;
        private readonly MResourceManager _resourceManager;
        private object luaEnv;
        private Type luaEnvType;
        private Type luaTableType;
        private PropertyInfo globalProperty;
        private MethodInfo disposeMethod;
        private MethodInfo doStringMethod;
        private MethodInfo setMethod;
        private object globalTable;

        public MLuaInterpreter(ABRuntimeState runtimeState, MResourceManager resourceManager)
        {
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
        }

        public void Initialize()
        {
            if (luaEnv != null) return;

            string CSAssemblyPath = GetAssemblyCSharpDLLName();
            if (string.IsNullOrEmpty(CSAssemblyPath) || !File.Exists(CSAssemblyPath))
            {
                MLog.Default?.W($"Lua hot update initialization skipped: Assembly-CSharp.dll not found, path={CSAssemblyPath}");
                return;
            }

            Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

            luaEnvType = assembly.GetType("XLua.LuaEnv");
            luaTableType = assembly.GetType("XLua.LuaTable");
            if (luaEnvType == null || luaTableType == null)
            {
                MLog.Default?.W("Lua hot update initialization skipped: XLua.LuaEnv or XLua.LuaTable type not found.");
                return;
            }

            luaEnv = Activator.CreateInstance(luaEnvType);

            globalProperty = luaEnv.GetType().GetProperty("Global", BindingFlags.Public | BindingFlags.Instance);
            globalTable = globalProperty.GetValue(luaEnv);
            setMethod = luaTableType.GetMethod("Set");
            setMethod = setMethod.MakeGenericMethod(typeof(string), typeof(GameObject));

            disposeMethod = luaEnv.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            doStringMethod = luaEnv.GetType().GetMethod("DoString", new Type[] { typeof(string), typeof(string), luaTableType });

            Type customLoaderType = luaEnvType.GetNestedType("CustomLoader");
            MethodInfo methodInfo = typeof(MLuaInterpreter).GetMethod("CustomLuaLoader", BindingFlags.NonPublic | BindingFlags.Instance);
            Delegate loaderDelegate = Delegate.CreateDelegate(customLoaderType, this, methodInfo);
            MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
            addLoaderMethod.Invoke(luaEnv, new object[] { loaderDelegate });

            ApplyInjections();
        }

        public void Dispose()
        {
            if (luaEnv == null) return;

            disposeMethod.Invoke(luaEnv, null);
            luaEnv = null;
        }

        public void RequireLua(string sLuaName)
        {
            if (luaEnv != null)
            {
                doStringMethod.Invoke(luaEnv, new object[] { $"require '{sLuaName}'", "chunk", null });
            }
        }

        private void ApplyInjections()
        {
            var injections = _runtimeState.LuaInjections;
            if (injections == null) return;

            foreach (var injection in injections)
            {
                Set(injection.name, injection.value);
            }
        }

        private void Set(string name, GameObject go)
        {
            setMethod.Invoke(globalTable, new object[] { name, go });
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
            TextAsset textAsset = null;

            var platform = Application.platform;
            if (platform == RuntimePlatform.WindowsEditor && _runtimeState.LuaResourcesLoad)
            {
                textAsset = Resources.Load<TextAsset>($"{filePath}.lua");
            }
            else if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
            {
                IResource luaResource = _resourceManager.LoadByName($"{filePath}{LuaFileSuffix}", false);
                textAsset = luaResource.GetAsset() as TextAsset;
            }
            else
            {
                throw new NotSupportedException();
            }

            if (textAsset == null)
            {
                throw new FileNotFoundException($"Lua file not found: {filePath}{LuaFileSuffix}");
            }

            return System.Text.Encoding.UTF8.GetBytes(textAsset.text);
        }

        private string GetAssemblyCSharpDLLName()
        {
#if UNITY_EDITOR
            return $"{Application.dataPath}/../Library/ScriptAssemblies/Assembly-CSharp.dll";
#elif UNITY_STANDALONE
            return $"{Application.dataPath}/Managed/Assembly-CSharp.dll";
#else
            MLog.Default?.E($"Lua hot update does not support current platform: platform={Application.platform}");
            return null;
#endif
        }
    }
}
