using System;
using System.IO;
using System.Reflection;
using MFramework.Core;
using UnityEngine;

namespace MFramework
{
    // TODO：使用反射，真的只能用反射吗？
    public class MLuaInterpreter : MMonoSingleton<MLuaInterpreter>
    {
        private object luaEnv = null;

        private const string LuaFileSuffix = ".lua.txt";

        private Type luaEnvType;
        private Type luaTableType;
        private PropertyInfo globalProperty;
        private MethodInfo disposeMethod;
        private MethodInfo doStringMethod;
        private MethodInfo setMethod;
        private object globalTable;
        private ABRuntimeState _runtimeState;
        private MResourceManager _resourceManager;

        public void Configure(ABRuntimeState runtimeState, MResourceManager resourceManager)
        {
            _runtimeState = runtimeState;
            _resourceManager = resourceManager;
        }

        protected override void Awake()
        {
            base.Awake();

            if (luaEnv == null)
            {
                string CSAssemblyPath = GetAssemblyCSharpDLLName();
                if (string.IsNullOrEmpty(CSAssemblyPath) || !File.Exists(CSAssemblyPath))
                {
                    MLog.Default?.W("AB warning.");
                    return;
                }

                Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

                luaEnvType = assembly.GetType("XLua.LuaEnv");
                luaTableType = assembly.GetType("XLua.LuaTable");
                if (luaEnvType == null || luaTableType == null)
                {
                    MLog.Default?.W("AB warning.");
                    return;
                }

                luaEnv = Activator.CreateInstance(luaEnvType);

                // 注入所需参数
                globalProperty = luaEnv.GetType().GetProperty("Global", BindingFlags.Public | BindingFlags.Instance);
                globalTable = globalProperty.GetValue(luaEnv);
                setMethod = luaTableType.GetMethod("Set");
                setMethod = setMethod.MakeGenericMethod(typeof(string), typeof(GameObject));

                // 需要选择无参Dispose()
                disposeMethod = luaEnv.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                doStringMethod = luaEnv.GetType().GetMethod("DoString", new Type[] { typeof(string), typeof(string), luaTableType });

                // 生成委托
                Type customLoaderType = luaEnvType.GetNestedType("CustomLoader");
                MethodInfo methodInfo = typeof(MLuaInterpreter).GetMethod("CustomLuaLoader", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate loaderDelegate = Delegate.CreateDelegate(customLoaderType, this, methodInfo);
                // 调用AddLoader（形参为CustomLoader委托）
                MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
                addLoaderMethod.Invoke(luaEnv, new object[] { loaderDelegate });
            }
        }

        private void Start()
        {
            var injections = _runtimeState?.LuaInjections;
            if (injections != null)
            {
                foreach (var injection in injections)
                {
                    Set(injection.name, injection.value);
                }
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

        private void Set(string name, GameObject go)
        {
            setMethod.Invoke(globalTable, new object[] { name, go });
        }

        private byte[] CustomLuaLoader(ref string filePath)
        {
            TextAsset textAsset = null;

            var platform = Application.platform;
            if (platform == RuntimePlatform.WindowsEditor && (_runtimeState == null || _runtimeState.LuaResourcesLoad))
            {
                textAsset = Resources.Load<TextAsset>($"{filePath}.lua");
            }
            else if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
            {
                if (_resourceManager == null)
                {
                    throw new InvalidOperationException($"{nameof(MLuaInterpreter)} requires {nameof(MResourceManager)}. Configure it from DI before loading Lua from AB.");
                }

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
            MLog.Default?.E("AB error.");
            return null;
#endif
        }
    }
}
