using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class InclinometerSensor : IInternalDeviceSensor
    {
        public const string AttributePitchDegrees = "PitchDegrees";
        public const string AttributeYawDegrees = "YawDegrees";
        public const string AttributeRollDegrees = "RollDegrees";

        public const string AttributeYawAccuracy = "YawAccuracy";

        private readonly Inclinometer _inclinometer;

        public bool Available => _inclinometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Inclinometer;
        public string Measurement
        {
            get
            {
                if(!Available)
                    return null;

                var sensorReading = _inclinometer.GetCurrentReading();
                _attributes[AttributePitchDegrees] = sensorReading.PitchDegrees.ToString();
                _attributes[AttributeYawDegrees] = sensorReading.YawDegrees.ToString();
                _attributes[AttributeRollDegrees] = sensorReading.RollDegrees.ToString();

                _attributes[AttributeYawAccuracy] = sensorReading.YawAccuracy.ToString();

                return string.Empty;
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public InclinometerSensor(Inclinometer inclinometer)
        {
            _inclinometer = inclinometer;
        }
    }
}
