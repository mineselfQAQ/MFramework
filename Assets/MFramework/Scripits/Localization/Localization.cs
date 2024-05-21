using UnityEngine;

namespace MFramework
{
    public class MLocalization : MonoBehaviour
    {
        public LocalizationMode mode = LocalizationMode.On;
        public int localID = -1;
    }

    public enum LocalizationMode
    {
        On,
        Off
    }
}