using System;
using System.Collections.Generic;

namespace MFramework
{
    public static class MEventSystem
    {
        private static Dictionary<int, Action> eventDict = new Dictionary<int, Action>();
        //builtInEventDict---一般事件用
        //builtInEventDict2---OnApplicationFocus与OnApplicationPause用，带有bool形参
        private static Dictionary<int, Action> builtInEventDict = new Dictionary<int, Action>();
        private static Dictionary<int, Action<bool>> builtInEventDict2 = new Dictionary<int, Action<bool>>();

        public static void Dispatch(int id)
        {
            if (!eventDict.ContainsKey(id))//没有调用过AddListener()情况
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能分发，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }
            if (eventDict[id] == null)//调用过AddListener()但是Action已完全移除
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能分发，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }

            eventDict[id].Invoke();
        }
        internal static void DispatchBuiltIn(BuiltInEvent e)
        {
            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))//没有调用过AddListener()情况
            {
                return;
            }
            if (builtInEventDict[id] == null)//没有Action不执行
            {
                return;
            }

            builtInEventDict[id].Invoke();
        }
        internal static void DispatchBuiltIn(BuiltInEvent e, bool b)
        {
            int id = (int)e;

            if (!builtInEventDict2.ContainsKey(id))//没有调用过AddListener()情况
            {
                return;
            }
            if (builtInEventDict2[id] == null)//没有Action不执行
            {
                return;
            }

            builtInEventDict2[id].Invoke(b);
        }

        public static void AddListener(int id, Action action)
        {
            if (!eventDict.ContainsKey(id))
            {
                eventDict.Add(id, action);
                return;
            }

            eventDict[id] += action;
        }
        public static void AddListener(BuiltInEvent e, Action action)
        {
            if (e == BuiltInEvent.ONAPPLICATIONFOCUS || e == BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}：{typeof(BuiltInEvent).Name}.{e}请使用Action<bool>而非Action", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))
            {
                builtInEventDict.Add(id, action);
                return;
            }

            builtInEventDict[id] += action;
        }
        public static void AddListener(BuiltInEvent e, Action<bool> action)
        {
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}：{typeof(BuiltInEvent).Name}.{e}请使用Action而非Action<bool>", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))
            {
                builtInEventDict2.Add(id, action);
                return;
            }

            builtInEventDict2[id] += action;
        }

        public static void RemoveListener(int id, Action action)
        {
            if (!eventDict.ContainsKey(id))//没有调用过AddListener()情况
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }
            if(eventDict[id] == null)//调用过AddListener()但是Action已完全移除
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }

            eventDict[id] -= action;
        }
        public static void RemoveListener(BuiltInEvent e, Action action)
        {
            if (e == BuiltInEvent.ONAPPLICATIONFOCUS || e == BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}：{typeof(BuiltInEvent).Name}.{e}请使用Action<bool>而非Action", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict.ContainsKey(id))//没有调用过AddListener()情况
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }
            if (builtInEventDict[id] == null)//调用过AddListener()但是Action已完全移除
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }

            builtInEventDict[id] -= action;
        }
        public static void RemoveListener(BuiltInEvent e, Action<bool> action)
        {
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                MLog.Print($"{typeof(MEventSystem)}：{typeof(BuiltInEvent).Name}.{e}请使用Action而非Action<bool>", MLogType.Warning);
                return;
            }

            int id = (int)e;

            if (!builtInEventDict2.ContainsKey(id))//没有调用过AddListener()情况
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }
            if (builtInEventDict2[id] == null)//调用过AddListener()但是Action已完全移除
            {
                MLog.Print($"{typeof(MEventSystem)}：id-<{id}>内没有Action不能移除，请先调用AddListener()后使用", MLogType.Warning);
                return;
            }

            builtInEventDict2[id] -= action;
        }

        public static void RemoveAllListener(int id)
        {
            if (!eventDict.ContainsKey(id))//没有调用过AddListener()，也就没有Action
            {
                return;
            }

            eventDict[id] = null;
        }
        public static void RemoveAllListener(BuiltInEvent e)
        {
            int id = (int)e;
            if (e != BuiltInEvent.ONAPPLICATIONFOCUS && e != BuiltInEvent.ONAPPLICATIONPAUSE)
            {
                if (!builtInEventDict.ContainsKey(id))//没有调用过AddListener()，也就没有Action
                {
                    return;
                }

                builtInEventDict[id] = null;
            }
            else
            {
                if (!builtInEventDict2.ContainsKey(id))//没有调用过AddListener()，也就没有Action
                {
                    return;
                }

                builtInEventDict2[id] = null;
            }
        }
    }
}