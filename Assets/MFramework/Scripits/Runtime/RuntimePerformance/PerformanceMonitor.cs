using System.Collections.Generic;
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

        private List<IMonitor> _moniters = new List<IMonitor>();

        public PerformanceMonitor(FPSMonitor.DisplayMode mode, float sampleDuration)
        {
            AddMonitor(new FPSMonitor(mode, sampleDuration));
        }

        private void AddMonitor(IMonitor monitor)
        {
            if (!_moniters.Contains(monitor))
            {
                _moniters.Add(monitor);
            }
        }

        public void Update()
        {
            foreach (var monitor in _moniters)
            {
                monitor.Update();
            }
        }

        public void Draw()
        {
            foreach (var monitor in _moniters)
            {
                monitor.Draw();
            }
        }
    }
}
