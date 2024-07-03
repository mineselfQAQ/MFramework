using UnityEngine;

namespace MFramework
{
    public class MLocalization : MonoBehaviour
    {
        [SerializeField] private LocalizationMode localMode = LocalizationMode.On;
        [SerializeField] private int localID = -1;

        public LocalizationMode LocalMode
        {
            /*internal*/ set { localMode = value; }
            get { return localMode; }
        }
        public int LocalID
        {
            /*internal*/ set { localID = value; }
            get { return localID; }
        }
    }

    public enum LocalizationMode
    {
        On,
        Off
    }
}