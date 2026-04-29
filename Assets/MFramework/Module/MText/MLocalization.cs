using UnityEngine;

namespace MFramework.Text
{
    public enum MTextLocalizationMode
    {
        Off,
        On,
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("MFramework/MText/MLocalization")]
    public sealed class MLocalization : MonoBehaviour
    {
        [SerializeField] private MTextLocalizationMode mode = MTextLocalizationMode.On;
        [SerializeField] private string key;

        public MTextLocalizationMode Mode
        {
            get => mode;
            set => mode = value;
        }

        public string Key
        {
            get => key;
            set => key = value;
        }
    }
}
