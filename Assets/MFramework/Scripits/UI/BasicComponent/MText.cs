using TMPro;
using UnityEngine;

namespace MFramework.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MLocalization))]
    [AddComponentMenu("MFramework/MText")]
    public class MText : TextMeshProUGUI
    {
        private MLocalization localization;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            localization = GetComponent<MLocalization>();
        }
    }
}