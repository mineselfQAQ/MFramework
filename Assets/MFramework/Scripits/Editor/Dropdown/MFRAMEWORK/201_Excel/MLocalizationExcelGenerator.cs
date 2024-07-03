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
            MGUIUtility.DrawH1("���ػ�������");

            DrawCreateBtn();

            MGUIUtility.DrawH2("��ѯ���������е�MLocalization");
            EditorGUILayout.LabelField("Tip:Prefab���ֲ��ø��ģ����Ԥ��������޸�", MGUIStyleUtility.ColorStyle(Color.red));

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
                EditorGUILayout.LabelField("��ת��ID:", GUILayout.Width(60));
                pos = EditorGUILayout.IntField(pos, GUILayout.Width(50));
                EditorGUILayout.LabelField("��", GUILayout.Width(50));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("��ת", GUILayout.Width(100)))
                {
                    GUI.FocusControl(null);//ȡ���۽�
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
            MLog.Print($"{typeof(MLocalizationExcelGenerator)}��δ�ҵ�idΪ{pos}�����壬����", MLogType.Warning);
            return -1;
        }
        private void DrawAutoSortBtn()
        {
            if (GUILayout.Button("һ������"))
            {
                //TODO:�ռ����г����Լ�����Prefab�е�MLocalization�����г�ʼ������
            }
        }
        private void DrawResetSortBtn()
        {
            if (GUILayout.Button("��������"))
            {
                infos = GetMLocalizationTabelInfo(true); 
                GUI.FocusControl(null);//ȡ���۽�
                return;
            }
        }
        private void DrawCreateBtn()
        {
            if (GUILayout.Button("����Excel�ļ�"))
            {
                string fileName = MSettings.LocalizationTableName;
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                FileInfo file = new FileInfo(fileName);
                if (file.Exists)
                {
                    int flag = EditorUtility.DisplayDialogComplex("Generating",
                        "���ػ�Excel�ļ��Ѵ��ڣ��Ƿ���Ҫ���к��ֲ���", "����", "ȡ��", "����");
                    if (flag == 1) return;//ȡ��
                    else if (flag == 2)//����
                    {
                        //TODO:δ���
                        MLog.Print("TODO", MLogType.Warning);
                        return;
                    }
                    else if (flag == 0)//����---ɾ���ش�
                    {
                        file.Delete();
                        file = new FileInfo(fileName);
                    }
                }

                //Ѱ������MLocalization�ű�
                List<LocalizationTableInfo> infos = GetMLocalizationTabelInfo(true);

                //����Excel�ļ�
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");//������1
                    worksheet.Cells["A1"].LoadFromDataTable(GetLocalizationTable(GetValidInfos(infos)), true);//������ʼ������

                    int row = infos.Count + 3;
                    worksheet.Cells[$"A1:E{row}"].AutoFitColumns();//�����п�
                    worksheet.Cells[$"A1:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;//����
                    worksheet.Cells["A1:E3"].Style.Font.Bold = true;//�Ӵ�

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

            //�����ȴ��У����������
            table.Columns.Add("���");
            table.Columns.Add("������");
            table.Columns.Add("����");
            table.Columns.Add("����");
            table.Columns.Add("Ӣ��");
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
            //�������ڼ��ֿ��ܣ�
            //1.���������---�������嶼��ǰ����Hierarchy�У���ô��ʱȫ����ȡ���ļ���
            //2.Ԥ���������������Ԥ���壬���Ա�Ȼ�ڳ�����û�����壬��ô��ʱ�޷�����
            //2.1.�����з���---Ԥ�����Ȼ��������ʱ����������Ԥ�����ʱ�Ѿ��������ɣ��޷���������
            //2.2.���з���---��Ԥ����������һ�����볡������в��������Ǵ�ʱ��Ҫ����|����ļ���״̬|Ԥ����ı���|
            //2.3.���з���---�����Ԥ���壬ֱ�ӽ���Ԥ����������

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
                        EditorGUILayout.LabelField($"���֣�{info.text}");

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.ObjectField(info.go, typeof(GameObject), true, GUILayout.Width(175));
                            GUI.enabled = true;

                            if (info.prefabParent != null)
                            {
                                if (GUILayout.Button("��ת"))
                                {
                                    Selection.activeGameObject = info.prefabParent;
                                }
                                if (GUILayout.Button("����Prefab"))
                                {
                                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(info.prefabParent);
                                    PrefabStageUtility.OpenPrefab(prefabPath);
                                }
                            }

                            GUILayout.FlexibleSpace();
                            if (info.id == -1)
                            {
                                if (GUILayout.Button("�Ƴ�MLocalization"))
                                {
                                    info.go.GetComponent<MLocalization>().LocalMode = LocalizationMode.Off;
                                    //infos.Remove(info);

                                    infos = GetMLocalizationTabelInfo(true);
                                    GUI.FocusControl(null);//ȡ���۽�
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
        /// ��ȡ����ɸѡ���MLocalization��Ϣ
        /// </summary>
        /// <returns></returns>
        private List<LocalizationTableInfo> GetMLocalizationTabelInfo(bool isSort)
        {
            List<LocalizationTableInfo> infos = new List<LocalizationTableInfo>();
            List<MLocalization> mLocalList = MLocalizationUtility.FindAllLoclizations();
            foreach (var mLocal in mLocalList)
            {
                if (mLocal.LocalMode == LocalizationMode.Off) continue;//localIDΪ-1Ҳ��Ҫ���У���Ϊ�����ǻ�û��

                GameObject parent = GetPrefabParent(mLocal);
                LocalizationTableInfo info = new LocalizationTableInfo(mLocal, mLocal.LocalID, mLocal.gameObject, mLocal.GetComponent<MText>().text, parent);
                infos.Add(info);
            }

            if(isSort) infos = infos.OrderBy(info => info.id).ToList();//����

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
