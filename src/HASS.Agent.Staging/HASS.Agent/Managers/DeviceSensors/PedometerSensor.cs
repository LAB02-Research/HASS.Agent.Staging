using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class PedometerSensor : IInternalDeviceSensor
    {
        private readonly Pedometer _pedometer;

        public bool Available => _pedometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Pedometer;
        public string Measurement
        {
            get
            {
                if(!Available)
                    return null;

                var totalStepCount = 0;

                var sensorReadings = _pedometer.GetCurrentReadings();
                foreach(var sensorReading in sensorReadings)
                {
                    var attributeCumulativeSteps = $"{sensorReading.Key}CumulativeSteps";
                    _attributes[attributeCumulativeSteps] = sensorReading.Value.CumulativeSteps.ToString();
                    totalStepCount += sensorReading.Value.CumulativeSteps;

                    var attributeCumulativeStepsDuration = $"{sensorReading.Key}CumulativeStepsDuration";
                    _attributes[attributeCumulativeStepsDuration] = sensorReading.Value.CumulativeStepsDuration.ToString();
                }

                return totalStepCount.ToString();
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public PedometerSensor(Pedometer pedometer)
        {
            _pedometer = pedometer;
        }
    }
}
