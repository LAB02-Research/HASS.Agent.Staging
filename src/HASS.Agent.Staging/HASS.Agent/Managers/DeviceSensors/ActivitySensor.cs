using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class ActivitySensor : IDeviceSensor
    {
        public const string AttributeConfidence = "Confidence";
        public const string AttributeTimestamp = "Timestamp";

        private readonly Windows.Devices.Sensors.ActivitySensor _activitySensor;

        public bool Available => _activitySensor != null;
        public DeviceSensorType Type => DeviceSensorType.ActivitySensor;
        public string Measurement
        {
            get
            {
                var sensorReading = _activitySensor?.GetCurrentReadingAsync().AsTask().Result;
                
            }
        }

        public Dictionary<string, string> Attributes => throw new NotImplementedException();

        public ActivitySensor(Windows.Devices.Sensors.ActivitySensor activitySensor)
        {
            _activitySensor = activitySensor;
            new ActivitySensorReading().
        }
    }
}
