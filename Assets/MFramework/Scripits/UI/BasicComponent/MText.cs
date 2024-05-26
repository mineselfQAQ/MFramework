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

        /// <summary>
        /// 更改当前text的选项
        /// </summary>
        /// <param name="pos">{}位置</param>
        /// <param name="state">选项</param>
        public void ChangeState(int pos, int state)
        {

        }
    }
}