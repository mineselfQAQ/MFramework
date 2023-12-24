using System;
using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public class EventSystem
    {
        private static Dictionary<int, Action> eventDict = new Dictionary<int, Action>();

        public static void Dispatch(int id)
        {
            if (!eventDict.ContainsKey(id))
            {
                Debug.Log("Error");
                return;
            }

            if (eventDict[id] == null)
            {
                Debug.Log("Error");
                return;
            }

            eventDict[id]();
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

        public static void RemoveListener(int id, Action action)
        {
            if (!eventDict.ContainsKey(id))
            {
                Debug.Log("Error");
                return;
            }

            if(eventDict[id] == null)
            {
                Debug.Log("Error");
                return;
            }

            eventDict[id] -= action;
        }
    }
}