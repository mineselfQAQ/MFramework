using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

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

        private void ApplyLocalization(SupportLanguage language)
        {
            if (mLocal == null || mLocal.LocalID == -1) return;


        }
    }
}