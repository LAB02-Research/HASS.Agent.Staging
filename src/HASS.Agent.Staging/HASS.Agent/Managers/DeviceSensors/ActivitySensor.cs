﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class ActivitySensor : IInternalDeviceSensor
    {
        public const string AttributeConfidence = "Confidence";
        public const string AttributeTimestamp = "Timestamp";

        private readonly Windows.Devices.Sensors.ActivitySensor _activitySensor;

        public bool Available => _activitySensor != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.ActivitySensor;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _activitySensor.GetCurrentReadingAsync().AsTask().Result;
                _attributes[AttributeConfidence] = sensorReading.Confidence.ToString();
                _attributes[AttributeTimestamp] = sensorReading.Timestamp.ToLocalTime().ToString();

                return sensorReading.Activity.ToString();
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public ActivitySensor(Windows.Devices.Sensors.ActivitySensor activitySensor)
        {
            _activitySensor = activitySensor;
        }
    }
}
