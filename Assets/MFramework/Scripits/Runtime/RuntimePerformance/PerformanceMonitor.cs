using UnityEngine;

namespace MFramework
{
    public class PerformanceMonitor : IMonitor
    {
        public enum PKeycode
        {
            Space,
            E,
            Backspace
        }
        public static KeyCode ToKeycode(PKeycode keycode)
        {
            switch (keycode)
            {
                case PKeycode.Space:
                    return KeyCode.Space;
                case PKeycode.E:
                    return KeyCode.E;
                case PKeycode.Backspace:
                    return KeyCode.Backspace;
                default:
                    return KeyCode.None;
            }
        }

        private IMonitor fpsMonitor;

        public bool CheckFPS => fpsMonitor != null;

        public PerformanceMonitor(FPSMonitor.DisplayMode mode, float sampleDuration)
        {
            fpsMonitor = new FPSMonitor(mode, sampleDuration);
        }

        public void Update()
        {
            fpsMonitor.Update();
        }

        public void FPSResult(out float best, out float average, out float worst)
        {
            ((FPSMonitor)fpsMonitor).GetCurPerformance(out float B, out float A, out float W);
            best = B;
            average = A;
            worst = W;
        }
    }
}
