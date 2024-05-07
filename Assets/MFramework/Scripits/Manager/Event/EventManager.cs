using System.Collections.Generic;

namespace MFramework
{
    public class EventManager
    {
        public int uid = -1;

        //key---int值，通过该值(id)可以找到对应的MEvent
        //value---MEvent，指代的是某一模块下的某一组事件
        public Dictionary<int, MEvent> dict = new Dictionary<int, MEvent>();

        public EventManager(int uid)
        {
            this.uid = uid;
        }

        //分发事件
        public bool DispatchEvent(int id)
        {
            if (dict.ContainsKey(id))
            {
                if (dict[id].action != null)
                    dict[id].action.Invoke();
                return true;
            }
            else
            {
                RegisterEvent(new MEvent(), id);
                return false;
            }
        }

        //添加监听
        public bool AddListener(int id, MEvent mEvent)
        {
            //已经AddListener()过了，所以注册完毕，可以直接添加MEvent
            if (dict.ContainsKey(id))
            {
                dict[id].action += mEvent.action;
                return true;
            }
            else//该id还没有进行过操作，需要创建新的(关键是放入字典)
            {
                RegisterEvent(new MEvent(), id);
                dict[id].action += mEvent.action;
                return false;
            }
        }

        //移除监听
        public bool RemoveListener(int id, MEvent mEvent)
        {
            if (dict.ContainsKey(id))
            {
                dict[id].action -= mEvent.action;
                return true;
            }
            else
            {
                MLog.Print($"EventManager.RemoveListener() --- target event id not found.", MLogType.Error);
                return false;
            }
        }

        //注册事件
        public bool RegisterEvent(MEvent mEvent, int id)
        {
            return InsertOrUpdateKeyValueInDictionary(dict, id, mEvent);
        }

        //移除事件
        public bool RemoveEvent(int id)
        {
            return RemoveKeyInDictionary(dict, id);
        }



        private bool InsertOrUpdateKeyValueInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
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
        private bool RemoveKeyInDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null)
            {
                MLog.Print("RemoveKeyInDictionary: <Dictionary dict> is null.", MLogType.Error);
                return false;
            }
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
                return true;
            }
            else
                return false;
        }
    }

}