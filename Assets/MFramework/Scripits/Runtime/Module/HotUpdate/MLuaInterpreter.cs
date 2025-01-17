using System;
using System.Reflection;
using UnityEngine;

namespace MFramework
{
    //TODO��ʹ�÷��䣬���ֻ���÷�����
    public class MLuaInterpreter : MonoSingleton<MLuaInterpreter>
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

        private void Awake()
        {
            if (luaEnv == null)
            {
                string CSAssemblyPath = GetAssemblyCSharpDLLName();
                Assembly assembly = Assembly.LoadFile(CSAssemblyPath);

                luaEnvType = assembly.GetType("XLua.LuaEnv");
                luaTableType = assembly.GetType("XLua.LuaTable");
                luaEnv = Activator.CreateInstance(luaEnvType);

                //ע���������
                globalProperty = luaEnv.GetType().GetProperty("Global", BindingFlags.Public | BindingFlags.Instance);
                globalTable = globalProperty.GetValue(luaEnv); 
                setMethod = luaTableType.GetMethod("Set");
                setMethod = setMethod.MakeGenericMethod(typeof(string), typeof(GameObject));

                //��Ҫѡ���޲�Dispose()
                disposeMethod = luaEnv.GetType().GetMethod("Dispose", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                doStringMethod = luaEnv.GetType().GetMethod("DoString", new Type[] { typeof(string), typeof(string), luaTableType });

                //����ί��
                Type customLoaderType = luaEnvType.GetNestedType("CustomLoader");
                MethodInfo methodInfo = typeof(MLuaInterpreter).GetMethod("CustomLuaLoader", BindingFlags.NonPublic | BindingFlags.Instance);
                Delegate loaderDelegate = Delegate.CreateDelegate(customLoaderType, this, methodInfo);
                //����AddLoader(�β�ΪCustomLoaderί��)
                MethodInfo addLoaderMethod = luaEnvType.GetMethod("AddLoader");
                addLoaderMethod.Invoke(luaEnv, new object[] { loaderDelegate });
            }
        }

        private void Start()
        {
            var injections = MCore.Instance.LuaInjections;
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
            if (platform == RuntimePlatform.WindowsEditor && MCore.Instance.LuaResourcesLoad)
            {
                textAsset = Resources.Load<TextAsset>($"{filePath}.lua");
            }
            else if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsEditor)
            {
                IResource luaResource = MResourceManager.Instance.LoadByName($"{filePath}{LuaFileSuffix}", false);
                textAsset = luaResource.GetAsset() as TextAsset;
            }
            else
            {
                throw new NotSupportedException();
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
            MLog.Print($"{typeof(LuaInterpreter)}����δʵ��", MLogType.Error);
#endif
        }
    }
}
