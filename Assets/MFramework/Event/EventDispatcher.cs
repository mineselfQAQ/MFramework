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
        //value---EventHandler，就是get器想要获取的实例
        public static Dictionary<int, EventHandler> handlerDict = new Dictionary<int, EventHandler>();
        public static EventHandler Common { get { return GetHandler(EH_COMMON); } }
        public static EventHandler Player { get { return GetHandler(EH_PLAYER); } }
        public static EventHandler Item { get { return GetHandler(EH_ITEM); } }
        public static EventHandler Combat { get { return GetHandler(EH_COMBAT); } }
        public static EventHandler UI { get { return GetHandler(EH_UI); } }
        public static EventHandler Env { get { return GetHandler(EH_ENV); } }
        public static EventHandler FX { get { return GetHandler(EH_FX); } }

        private EventDispatcher() { }

        public static void Dispatch(EventHandler handler, int id)
        {
            handler.DispatchEvent(id);
        }

        public static bool AddListener(EventHandler handler, int id, MEvent mEvent)
        {
            if (handler == null)
            {
                Log.Print("EventDispatcher.AddListener()---event handler is null.", MLogType.Error);
                return false;
            }
            handler.AddListener(id, mEvent);
            return true;
        }

        public static bool RemoveListener(EventHandler handler, int id, MEvent mEvent)
        {
            if (handler == null)
            {
                Log.Print("EventDispatcher.RemoveListener()---event handler is null.", MLogType.Error);
                return false;
            }
            handler.RemoveListener(id, mEvent);
            return true;
        }

        private static EventHandler GetHandler(int type)
        {
            //第一次获取，进行注册
            if (!handlerDict.ContainsKey(type))
            {
                RegisterHandler(new EventHandler(type), type);
            }

            //正常就直接从字典中取出EventHandler实例即可
            return GetValueInDictionary(handlerDict, type);
        }
        public static bool RegisterHandler(EventHandler handler, int id)
        {
            return InsertOrUpdateKeyValueInDictionary(handlerDict, id, handler);
        }



        private static bool InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict == null)
            {
                Log.Print("InsertOrUpdateKeyInDictionary: <Dictionary dict> is null.", MLogType.Error);
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
                Log.Print("GetValueInDictionary: <Dictionary dict> is null.", MLogType.Error);
                return default(TValue);
            }
            if (dict.ContainsKey(key))
                return dict[key];
            else
            {
                Log.Print("GetValueInDictionary: Key is not present.", MLogType.Error);
                return default(TValue);
            }
        }
    }
}