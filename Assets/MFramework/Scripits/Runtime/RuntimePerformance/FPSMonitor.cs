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

        public void Draw()
        {
            GetCurPerformance(out float best, out float average, out float worst);

            var style24 = MGUIStyleUtility.GetStyle(24);
            var style30 = MGUIStyleUtility.GetStyle(30);

            //最大帧率
            int maxFrameRate = Application.targetFrameRate;
            int vSync = QualitySettings.vSyncCount;
            string maxStr = null;
            if (vSync == 0)//关闭
            {
                maxStr = maxFrameRate == -1 ? "Inf" : maxFrameRate.ToString();
            }
            else//开启
            {
                maxStr = ((int)(Screen.currentResolution.refreshRateRatio.value / vSync)).ToString();
            }

            //TODO:添加改位置功能(关键是各个Monitor的组合)
            //左上
            //GUI.Label(new Rect(10, 10, 200, 30), "FPS", style30);
            //GUI.Label(new Rect(10, 50, 200, 30), string.Format("Average:{0:0}", average), style24);
            //GUI.Label(new Rect(10, 80, 200, 30), string.Format("Best:{0:0}", best), style24);
            //GUI.Label(new Rect(10, 110, 200, 30), string.Format("Worst:{0:0}", worst), style24);
            //左下
            int bottom = Screen.height - 24;//24：字体高度
            GUI.Label(new Rect(10, bottom - 110, 200, 30), $"FPS(Max: {maxStr})", style30);
            GUI.Label(new Rect(10, bottom - 70, 200, 30), string.Format("Average:{0:0}", average), style24);
            GUI.Label(new Rect(10, bottom - 40, 200, 30), string.Format("Best:{0:0}", best), style24);
            GUI.Label(new Rect(10, bottom - 10, 200, 30), string.Format("Worst:{0:0}", worst), style24);
        }

        private void GetCurPerformance(out float best, out float average, out float worst)
        {
            best = B;
            average = A;
            worst = W;
        }
    }
}
