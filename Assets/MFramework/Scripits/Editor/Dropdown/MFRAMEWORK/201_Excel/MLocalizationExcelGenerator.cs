using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using MFramework.UI;
using UnityEditor.SceneManagement;

namespace MFramework
{
    public class MLocalizationExcelGenerator : EditorWindow
    {
        private bool FirstOpen = true;
        private List<LocalizationTableInfo> infos;

        private int pos;
        private Vector2 scrollPos1;

        [MenuItem("MFramework/MLocalizationExcelGenerator", priority = 203)]
        public static void Init()
        {
            EditorWindow window = GetWindow<MLocalizationExcelGenerator>(true, "MLocalizationExcelGenerator", false);
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            MGUIUtility.DrawH1("本地化生成器");

            DrawCreateBtn();

            MGUIUtility.DrawH2("查询场景中所有的MLocalization");
            EditorGUILayout.LabelField("Tip:Prefab部分不得更改，请打开预制体进行修改", MGUIStyleUtility.ColorStyle(Color.red));

            EditorGUILayout.BeginHorizontal();
            {
                DrawResetSortBtn();
                //DrawAutoSortBtn();//TODO
            }
            EditorGUILayout.EndHorizontal();

            DrawJumpToPosBtn();
            DrawMLocalizationChecker();
        }

        private void OnDestroy()
        {
            FirstOpen = true;
        }

        private void DrawJumpToPosBtn()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("跳转至ID:", GUILayout.Width(60));
                pos = EditorGUILayout.IntField(pos, GUILayout.Width(50));
                EditorGUILayout.LabelField("处", GUILayout.Width(50));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("跳转", GUILayout.Width(100)))
                {
                    GUI.FocusControl(null);//取消聚焦
                    int index = FindPosIndex(pos);
                    int y = index * 50;
                    scrollPos1.y = y;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        private int FindPosIndex(int pos)
        {
            for(int i = 0; i < infos.Count; i++)
            {
                if (infos[i].id == pos)
                {
                    return i;
                }
            }
            MLog.Print($"{typeof(MLocalizationExcelGenerator)}：未找到id为{pos}的物体，请检查", MLogType.Warning);
            return -1;
        }
        private void DrawAutoSortBtn()
        {
            if (GUILayout.Button("一键排序"))
            {
                //TODO:收集所有场景以及所有Prefab中的MLocalization并进行初始化排序
            }
        }
        private void DrawResetSortBtn()
        {
            if (GUILayout.Button("重置排序"))
            {
                infos = GetMLocalizationTabelInfo(true); 
                GUI.FocusControl(null);//取消聚焦
                return;
            }
        }
        private void DrawCreateBtn()
        {
            if (GUILayout.Button("创建Excel文件"))
            {
                string fileName = MSettings.LocalizationTableName;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    int flag = EditorUtility.DisplayDialogComplex("Generating",
                        "本地化Excel文件已存在，是否需要进行何种操作", "覆盖", "取消", "更新");
                    if (flag == 1) return;//取消
                    else if (flag == 2)//更新
                    {
                        //TODO:未完成
                        MLog.Print("TODO", MLogType.Warning);
                        return;
                    }
                    else if (flag == 0)//覆盖---删除重创
                    {
                        file.Delete();
                        file = new FileInfo(fileName);
                    }
                }

                //寻找所有MLocalization脚本
                List<LocalizationTableInfo> infos = GetMLocalizationTabelInfo(true);

                //创建Excel文件
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//创建表1
                    worksheet.Cells["A1"].LoadFromDataTable(GetLocalizationTable(GetValidInfos(infos)), true);//创建初始表内容

                    int row = infos.Count + 3;
                    worksheet.Cells[$"A1:E{row}"].AutoFitColumns();//调整行宽
                    worksheet.Cells[$"A1:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//居中
                    worksheet.Cells["A1:E3"].Style.Font.Bold = true;//加粗

                    package.Save();
                }

                System.Diagnostics.Process.Start(fileName);

                return;
            }
        }

        private List<LocalizationTableInfo> GetValidInfos(List<LocalizationTableInfo> infos)
        {
            List<LocalizationTableInfo> res = new List<LocalizationTableInfo>();
            foreach (var info in infos)
            {
                if (info.mLocal.LocalMode == LocalizationMode.Off || info.mLocal.LocalID == -1)
                {
                    continue;
                }
                res.Add(info);
            }
            return res;
        }
        private DataTable GetLocalizationTable(List<LocalizationTableInfo> infos)
        {
            DataTable table = new DataTable();

            //必须先创列，才能添加行
            table.Columns.Add("编号");
            table.Columns.Add("物体名");
            table.Columns.Add("描述");
            table.Columns.Add("中文");
            table.Columns.Add("英文");
            table.Rows.Add(new object[] { "ID", "GOName", "Desc", "Chinese", "English" });
            table.Rows.Add(new object[] { "int", "none", "none", "string", "string" });
            foreach (var info in infos)
            {
                table.Rows.Add(new object[] { info.id, info.go.name, "", "", "" });
            }

            return table;
        }

        private void DrawMLocalizationChecker()
        {
            //场景存在几种可能：
            //1.最正常情况---所有物体都提前创建Hierarchy中，那么此时全部获取更改即可
            //2.预制体情况，由于是预制体，所以必然在场景中没有物体，那么此时无法创建
            //2.1.不可行方案---预制体必然会在运行时创建，但是预制体此时已经复制生成，无法更改数据
            //2.2.可行方案---将预制体像正常一样拖入场景后进行操作，但是此时需要处理|物体的激活状态|预制体的保存|
            //2.3.可行方案---如果是预制体，直接进入预制体界面更改

            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();

            if (FirstOpen || infos == null)
            {
                infos = GetMLocalizationTabelInfo(true);
                FirstOpen = false;
            }
            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
            {
                foreach (var info in infos)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField($"文字：{info.text}");

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.ObjectField(info.go, typeof(GameObject), true, GUILayout.Width(175));
                            GUI.enabled = true;

                            if (info.prefabParent != null)
                            {
                                if (GUILayout.Button("跳转"))
                                {
                                    Selection.activeGameObject = info.prefabParent;
                                }
                                if (GUILayout.Button("进入Prefab"))
                                {
                                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(info.prefabParent);
                                    PrefabStageUtility.OpenPrefab(prefabPath);
                                }
                            }

                            GUILayout.FlexibleSpace();
                            if (info.id == -1)
                            {
                                if (GUILayout.Button("移除MLocalization"))
                                {
                                    info.go.GetComponent<MLocalization>().LocalMode = LocalizationMode.Off;
                                    //infos.Remove(info);

                                    infos = GetMLocalizationTabelInfo(true);
                                    GUI.FocusControl(null);//取消聚焦
                                }
                            }

                            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(info.go);
                            int oldID = info.id;
                            int newID = -1;
                            newID = EditorGUILayout.IntField(oldID, GUILayout.Width(50));

                            if (newID != oldID)
                            {
                                info.id = newID;
                                info.mLocal.LocalID = newID;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(10);
                }
            }
            EditorGUILayout.EndScrollView();

            SaveModify();
        }

        private void SaveModify()
        {

        }

        /// <summary>
        /// 获取排序并筛选后的MLocalization信息
        /// </summary>
        /// <returns></returns>
        private List<LocalizationTableInfo> GetMLocalizationTabelInfo(bool isSort)
        {
            List<LocalizationTableInfo> infos = new List<LocalizationTableInfo>();
            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();
            foreach (var mLocal in mLocalList)
            {
                if (mLocal.LocalMode == LocalizationMode.Off) continue;//localID为-1也需要进行，因为可能是还没改

                GameObject parent = GetPrefabParent(mLocal);
                LocalizationTableInfo info = new LocalizationTableInfo(mLocal, mLocal.LocalID, mLocal.gameObject, mLocal.GetComponent<MText>().text, parent);
                infos.Add(info);
            }

            if(isSort) infos = infos.OrderBy(info => info.id).ToList();//排序

            return infos;
        }
        private GameObject GetPrefabParent(MLocalization mLocal)
        {
            GameObject go = mLocal.gameObject;

            GameObject curRoot = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            while (curRoot != null)
            {
                Transform rootParent = curRoot.transform.parent;
                GameObject parentRoot = null;
                if (rootParent != null)
                {
                    parentRoot = PrefabUtility.GetNearestPrefabInstanceRoot(rootParent.gameObject);
                }

                if (parentRoot == null) return curRoot;
                curRoot = parentRoot;
            }

            return null;
        }

        private class LocalizationTableInfo
        {
            public MLocalization mLocal;
            public int id;
            public GameObject go;
            public string text;
            public GameObject prefabParent;

            public LocalizationTableInfo(MLocalization mLocal, int id, GameObject go, string text, GameObject parent)
            {
                this.mLocal = mLocal;
                this.id = id;
                this.go = go;
                this.text = text;
                this.prefabParent = parent;
            }
        }
    }
}
