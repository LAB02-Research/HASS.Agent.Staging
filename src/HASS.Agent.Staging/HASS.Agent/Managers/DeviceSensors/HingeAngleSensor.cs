using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class HingeAngleSensor : IInternalDeviceSensor
    {
        private readonly Windows.Devices.Sensors.HingeAngleSensor _hingeAngelSensor;

        public bool Available => _hingeAngelSensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.HingeAngleSensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return _hingeAngelSensor.GetCurrentReadingAsync().AsTask().Result.AngleInDegrees.ToString();
            }
        }

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public HingeAngleSensor(Windows.Devices.Sensors.HingeAngleSensor hingeAngleSensor)
        {
            _hingeAngelSensor = hingeAngleSensor;
        }
    }
}
