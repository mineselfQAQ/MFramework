using System.IO;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MFramework
{
    public class MLog : INeedInit, INeedQuit
    {
        private static FileStream fs;
        private static string path = $@"{Application.dataPath}/../LogCallBack.txt";

        public void Init()
        {
#if !UNITY_EDITOR
            if (MCore.Instance.LogState)
            {
                Application.logMessageReceived += OnLogCallBack;
            }
#endif
        }
        public void Quit()
        {
#if !UNITY_EDITOR
            if (MCore.Instance.LogState)
            {
                Application.logMessageReceived -= OnLogCallBack;
            }
#endif
        }

        public static void Blank()
        {
#if UNITY_EDITOR
            Debug.Log("");
#endif
        }

        public static void Print(object message, MLogType type = MLogType.Log, Object context = null)
        {
            switch (type) 
            {
#if UNITY_EDITOR
                case MLogType.Log:
                    Debug.Log($"<b>Log:</b> {message}", context);
                    break;
                case MLogType.Warning:
                    Debug.LogWarning($"<b><color=#CC9A06FF>Warning:</color></b> {message}", context);
                    break;
                case MLogType.Error:
                    Debug.LogError($"<b><color=#FF6E40FF>Error:</color></b> {message}", context);
                    throw new System.Exception("发生错误，请检查后继续执行");
#else
                case MLogType.Log:
                    if (MCore.Instance.LogState) Debug.Log($"Log: {message}", context);
                    break;
                case MLogType.Warning:
                    if (MCore.Instance.LogState) Debug.LogWarning($"Warning: {message}", context);
                    break;
                case MLogType.Error:
                    if (MCore.Instance.LogState) Debug.LogError($"Error: {message}", context);
                    throw new System.Exception("发生错误，请检查后继续执行");
#endif
            }
        }

        public static string Bold(object message)
        {
            string resultStr = message.ToString();
#if UNITY_EDITOR
            resultStr = $"<b>{message}</b>";

            return resultStr;
#else
            return resultStr;
#endif
        }

        public static string Italic(object message)
        {
            string resultStr = message.ToString();
#if UNITY_EDITOR
            resultStr = $"<i>{message}</i>";

            return resultStr;
#else
            return resultStr;
#endif
        }

        public static string Color(object message, Color color)
        {
#if UNITY_EDITOR
            return AddColor(message, color);
#else
            return message.ToString();
#endif
        }
        public static string Color(object message, Color color, bool isBold, bool isItalic)
        {
#if UNITY_EDITOR
            string resultStr = AddColor(message, color);

            if (isBold)
            {
                resultStr = $"<b>{resultStr}</b>";
            }
            if (isItalic)
            {
                resultStr = $"<i>{resultStr}</i>";
            }
            
            return resultStr;
#else
            return message.ToString();
#endif
        }

        private static void OnLogCallBack(string logString, string stackTrace, LogType type)
        {
            //创建或打开
            fs = new FileStream(path, FileMode.OpenOrCreate);
            //放置"指针"
            fs.Position = fs.Length;

            //输出内容
            string str = null;
            str = $"Time: {GetCurrTime()}\n" +
                    $"{logString}\n\n\n";

            //编码并写入
            byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            //关闭
            fs.Close();
        }



        private static string GetCurrTime()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
        }
        private static string AddColor(object message, Color color)
        {
            string htmlColor = ConvertColorToHtml(color);
            string resultStr = $"<color=#{htmlColor}>{message}</color>";

            return resultStr;
        }
        private static string ConvertColorToHtml(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }



#if UNITY_EDITOR
        //其中(0)必须添加，是优先级的含义，如果不添加有时不会触发
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            //首先需要拿到Console窗口下双击语句的输出信息
            string stackTrace = GetStackTrace();

            //如果拿到了且有"MLog.cs"，说明语句确实会跳转错误(堆栈到顶的话会找到MLog类下的某个函数)
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("MLog.cs"))
            {
                //寻找内容：
                //(at xxx)，这其实就是追踪函数的时候定位到的脚本信息
                //举例：(at Assets/Utils/MLog.cs:53)
                var matches = Regex.Match(stackTrace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
                string pathLine = "";
                //循环查找，先会匹配第一个找到的，然后使用NextMatch()找到第二个，以此类推
                while (matches.Success)
                {
                    //组[1]，指代的就是(.+)，以上面的例子来说，就是Assets/Utils/MLog.cs:53
                    pathLine = matches.Groups[1].Value;

                    //根据堆栈调用中的内容，我们可以知道：
                    //我们需要的是第一个不在MLog.cs的内容(就是因为封装后多进入了一层，到了MLog.cs才导致的问题)
                    if (!pathLine.Contains("MLog.cs"))
                    {
                        //拆分，获得路径与行号
                        int splitIndex = pathLine.LastIndexOf(":");
                        string path = pathLine.Substring(0, splitIndex);
                        line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        //拼接得到脚本的完整路径
                        string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullPath = fullPath + path;
                        //跳转到本来应该跳转到的位置
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        private static string GetStackTrace()
        {
            System.Type ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            // 获取Console窗口实例
            FieldInfo fieldInfo = ConsoleWindowType.GetField
                ("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            object consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                //判断当前聚焦的窗口是否为Console面板
                if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                {
                    // 获取m_ActiveText成员
                    fieldInfo = ConsoleWindowType.GetField
                        ("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                    // 获取m_ActiveText的值
                    string activeText = fieldInfo.GetValue(consoleInstance).ToString();

                    return activeText;
                }
            }
            return null;
        }
#endif
    }
}
