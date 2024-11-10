using UnityEngine;
using OpenHardwareMonitor.Hardware;

namespace MFramework
{
    public class HardwareMonitor : IMonitor
    {
        private Computer _computer;

        private float _sampleDuration;

        public HardwareMonitor(float sampleDuration)
        {
            _sampleDuration = sampleDuration;

            _computer = new Computer();
            _computer.Open();

            _computer.CPUEnabled = true;
            _computer.GPUEnabled = true;
            _computer.RAMEnabled = true;
            //computer.MainboardEnabled = true;
            //computer.FanControllerEnabled = true;
            //computer.HDDEnabled = true;
        }

        public void Draw()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                    {
                        MLog.Print(sensor.Name + " " + sensor.Value.Value + " " + sensor.Min.Value + " " + sensor.Max.Value);
                    }
                }
            }
        }
    }
}
