using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MLocalization))]
    [AddComponentMenu("MFramework/MText")]
    public class MText : TextMeshProUGUI
    {
        public MLocalization mLocal;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            mLocal = GetComponent<MLocalization>();
        }
    }

    public static class MTextExtension
    {
        public static void ChangeState(this MText text, int pos, int state)
        {
            MLocalizationManager.Instance.ChangeState(text, pos, state);
        }

        public static int GetCurState(this MText text, int pos)
        {
            return MLocalizationManager.Instance.GetCurState(text, pos);
        }
    }
}