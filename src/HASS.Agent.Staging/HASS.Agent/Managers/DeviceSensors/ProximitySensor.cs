using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class ProximitySensor : IInternalDeviceSensor
    {
        private Windows.Devices.Sensors.ProximitySensor _proximitySensor;

        public bool Available => _proximitySensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.ProximitySensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return _proximitySensor.GetCurrentReading().DistanceInMillimeters.ToString();
            }
        }

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public ProximitySensor(Windows.Devices.Sensors.ProximitySensor proximitySensor)
        {
            _proximitySensor = proximitySensor;
        }

        public void UpdateInternalSensor(Windows.Devices.Sensors.ProximitySensor proximitySensor)
        {
            _proximitySensor = proximitySensor;
        }
    }
}
