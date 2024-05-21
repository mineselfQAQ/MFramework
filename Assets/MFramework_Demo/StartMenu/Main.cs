using System.Collections.Generic;
using Table;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MFramework.Demo
{
    public class Main : MonoBehaviour
    {
        public CanvasGroup startPanel;
        public CanvasGroup languagePanel;

        public Button startBtn;
        public Button SettingBtn;
        public Button languageBtn;
        public Button quitBtn;
        public Button backBtn;

        public Button chineseBtn;
        public Button englishBtn;

        public MLocalization loc;

        private void Start()
        {
            startPanel.alpha = 1;
            languagePanel.alpha = 0;

            languageBtn.onClick.AddListener(() =>
            {
                startPanel.alpha = 0;
                languagePanel.alpha = 1;
            });
            backBtn.onClick.AddListener(() =>
            {
                startPanel.alpha = 1;
                languagePanel.alpha = 0;
            });

            quitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
            });

            //Localization
            chineseBtn.onClick.AddListener(() =>
            {
                
            });
            englishBtn.onClick.AddListener(() =>
            {
                var table = LocalizationTable.LoadBytes();
                var loclizations = FindAllLoclization();
                foreach (var loc in loclizations)
                {
                    foreach (var item in table)
                    {
                        if (item.ID == loc.localID)
                        {
                            loc.GetComponent<TMP_Text>().text = item.ENGLISH;
                        }
                    }
                }
            });
        }

        private List<MLocalization> FindAllLoclization()
        {
            List<MLocalization> list = new List<MLocalization>();
            list.Add(loc);
            return list;
        }
    }
}