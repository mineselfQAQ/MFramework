using System.Collections.Generic;
using UnityEngine;

namespace MFramework
{
    public static class MLocalizationUtility
    {
        public static List<MLocalization> FindAllLoclizations()
        {
            List<MLocalization> list = new List<MLocalization>(GameObject.FindObjectsOfType<MLocalization>(true));
            return list;
        }
        public static List<MLocalization> FindLoclizations(GameObject root)
        {
            List<MLocalization> list = new List<MLocalization>(root.GetComponentsInChildren<MLocalization>(true));
            return list;
        }
    }
}