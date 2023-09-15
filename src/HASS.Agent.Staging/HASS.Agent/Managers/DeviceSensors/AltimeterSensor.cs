using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class AltimeterSensor : IInternalDeviceSensor
    {
        private readonly Altimeter _altimeter;

        public bool Available => _altimeter != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Altimeter;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return _altimeter.GetCurrentReading().AltitudeChangeInMeters.ToString();
            }
        }

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public AltimeterSensor(Altimeter altimeter)
        {
            _altimeter = altimeter;
        }
    }
}
