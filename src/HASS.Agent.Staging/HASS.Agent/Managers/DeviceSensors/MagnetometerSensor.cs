using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class MagnetometerSensor : IInternalDeviceSensor
    {
        public const string AttributeMagneticFieldX = "MagneticFieldX";
        public const string AttributeMagneticFieldY = "MagneticFieldY";
        public const string AttributeMagneticFieldZ = "MagneticFieldZ";

        private readonly Magnetometer _magnetometer;

        public bool Available => _magnetometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Magnetometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _magnetometer.GetCurrentReading();
                _attributes[AttributeMagneticFieldX] = sensorReading.MagneticFieldX.ToString();
                _attributes[AttributeMagneticFieldY] = sensorReading.MagneticFieldY.ToString();
                _attributes[AttributeMagneticFieldZ] = sensorReading.MagneticFieldZ.ToString();

                return (
                    Math.Abs(sensorReading.MagneticFieldX) +
                    Math.Abs(sensorReading.MagneticFieldY) +
                    Math.Abs(sensorReading.MagneticFieldZ)
                    ).ToString();
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public MagnetometerSensor(Magnetometer magnetometer)
        {
            _magnetometer = magnetometer;
        }
    }
}
