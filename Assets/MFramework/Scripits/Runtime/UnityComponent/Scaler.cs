using UnityEngine;

namespace MFramework
{
    public class Scaler : MonoBehaviour
    {
        public float scaleMultipler = 1;
        public float frequency = 1;

        protected void Start()
        {
            transform.SinScaleLoopNoRecord(MCurve.Linear, scaleMultipler, frequency);
        }
    }
}
