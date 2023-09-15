using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class SimpleOrientationSensor : IInternalDeviceSensor
    {
        private readonly Windows.Devices.Sensors.SimpleOrientationSensor _simpleOrientationSensor;

        public bool Available => _simpleOrientationSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.SimpleOrientationSensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return _simpleOrientationSensor.GetCurrentOrientation().ToString();
            }
        }

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public SimpleOrientationSensor(Windows.Devices.Sensors.SimpleOrientationSensor simpleOrientationSensor)
        {
            _simpleOrientationSensor = simpleOrientationSensor;
        }
    }
}
