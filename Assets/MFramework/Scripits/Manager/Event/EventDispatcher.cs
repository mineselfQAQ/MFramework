using System.Collections.Generic;

namespace MFramework
{
    public sealed class EventDispatcher : Singleton<EventDispatcher>
    {
        //用于下方字典handlerDict，作为key进行检索
        public const int EH_COMMON = 0;
        public const int EH_PLAYER = 1;
        public const int EH_ITEM = 2;
        public const int EH_COMBAT = 3;
        public const int EH_UI = 4;
        public const int EH_ENV = 5;
        public const int EH_FX = 6;

        //key---int值，与上方的const值对应
        //value---EventManager，就是get器想要获取的实例
        public static Dictionary<int, EventManager> handlerDict = new Dictionary<int, EventManager>();
        public static EventManager Common { get { return GetHandler(EH_COMMON); } }
        public static EventManager Player { get { return GetHandler(EH_PLAYER); } }
        public static EventManager Item { get { return GetHandler(EH_ITEM); } }
        public static EventManager Combat { get { return GetHandler(EH_COMBAT); } }
        public static EventManager UI { get { return GetHandler(EH_UI); } }
        public static EventManager Env { get { return GetHandler(EH_ENV); } }
        public static EventManager FX { get { return GetHandler(EH_FX); } }

        private EventDispatcher() { }

        public static void Dispatch(EventManager handler, int id)
        {
            handler.DispatchEvent(id);
        }

        public static bool AddListener(EventManager handler, int id, MEvent mEvent)
        {
            if (handler == null)
            {
                MLog.Print("EventDispatcher.AddListener()---event handler is null.", MLogType.Error);
                return false;
            }
            handler.AddListener(id, mEvent);
            return true;
        }

        public static bool RemoveListener(EventManager handler, int id, MEvent mEvent)
        {
            if (handler == null)
            {
                MLog.Print("EventDispatcher.RemoveListener()---event handler is null.", MLogType.Error);
                return false;
            }
            handler.RemoveListener(id, mEvent);
            return true;
        }

        private static EventManager GetHandler(int type)
        {
            //第一次获取，进行注册
            if (!handlerDict.ContainsKey(type))
            {
                RegisterHandler(new EventManager(type), type);
            }

            //正常就直接从字典中取出EventHandler实例即可
            return GetValueInDictionary(handlerDict, type);
        }
        public static bool RegisterHandler(EventManager handler, int id)
        {
            return InsertOrUpdateKeyValueInDictionary(handlerDict, id, handler);
        }



        private static bool InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict == null)
            {
                MLog.Print("InsertOrUpdateKeyInDictionary: <Dictionary dict> is null.", MLogType.Error);
                return false;
            }
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);

            return true;
        }
        private static TValue GetValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null)
            {
                MLog.Print("GetValueInDictionary: <Dictionary dict> is null.", MLogType.Error);
                return default(TValue);
            }
            if (dict.ContainsKey(key))
                return dict[key];
            else
            {
                MLog.Print("GetValueInDictionary: Key is not present.", MLogType.Error);
                return default(TValue);
            }
        }
    }
}