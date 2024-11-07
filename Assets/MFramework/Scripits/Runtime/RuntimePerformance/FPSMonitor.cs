using UnityEngine;

namespace MFramework
{
    public class FPSMonitor : IMonitor
    {
        public enum DisplayMode { FPS, MS }

        private DisplayMode _displayMode;
        private float _sampleDuration;

        private int frames;
        private float duration, bestDuration = float.MaxValue, worstDuration;
        private float B = 0, A = 0, W = 0;//最好/平均/最差

        public FPSMonitor(DisplayMode mode, float sampleDuration)
        {
            _displayMode = mode;
            _sampleDuration = sampleDuration;
        }

        public void Update()
        {
            float frameDuration = Time.unscaledDeltaTime;
            frames += 1;
            duration += frameDuration;

            if (frameDuration < bestDuration)
            {
                bestDuration = frameDuration;
            }
            if (frameDuration > worstDuration)
            {
                worstDuration = frameDuration;
            }

            if (duration >= _sampleDuration)
            {
                if (_displayMode == DisplayMode.FPS)
                {
                    B = 1f / bestDuration;
                    A = frames / duration;
                    W = 1f / worstDuration;
                }
                else
                {
                    B = 1000f * bestDuration;
                    A = 1000f * duration / frames;
                    W = 1000f * worstDuration;
                }
                frames = 0;
                duration = 0f;
                bestDuration = float.MaxValue;
                worstDuration = 0f;
            }
        }

        public void GetCurPerformance(out float best, out float average, out float worst)
        {
            best = B;
            average = A;
            worst = W;
        }
    }
}
