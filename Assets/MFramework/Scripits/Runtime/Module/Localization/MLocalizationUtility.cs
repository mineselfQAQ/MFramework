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

        //public static List<MLocalization> FindAllLoclizations()
        //{
        //    if (PrefabStageUtility.GetCurrentPrefabStage() == null)//Scene Mode
        //    {
        //        List<MLocalization> list = new List<MLocalization>(GameObject.FindObjectsOfType<MLocalization>(true));
        //        return list;
        //    }
        //    else//Prefab Mode
        //    {
        //        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        //        GameObject instance = stage.prefabContentsRoot;
        //        string prefabPath = AssetDatabase.GetAssetPath(instance);
        //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        //        List<MLocalization> list = new List<MLocalization>(prefab.GetComponentsInChildren<MLocalization>(true));
        //        return list;
        //    }
        //}
    }
}