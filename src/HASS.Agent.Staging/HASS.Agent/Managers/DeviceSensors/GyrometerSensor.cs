﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class GyrometerSensor : IInternalDeviceSensor
    {
        public const string AttributeAngularVelocityX = "AngularVelocityX";
        public const string AttributeAngularVelocityY = "AngularVelocityY";
        public const string AttributeAngularVelocityZ = "AngularVelocityZ";

        private readonly Gyrometer _gyrometer;

        public bool Available => _gyrometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Gyrometer;
        public string Measurement
        {
            get
            {
                if(!Available)
                    return null;

                var sensorReading = _gyrometer.GetCurrentReading();
                _attributes[AttributeAngularVelocityX] = sensorReading.AngularVelocityX.ToString();
                _attributes[AttributeAngularVelocityY] = sensorReading.AngularVelocityY.ToString();
                _attributes[AttributeAngularVelocityZ] = sensorReading.AngularVelocityZ.ToString();

                return (
                    Math.Abs(sensorReading.AngularVelocityX) +
                    Math.Abs(sensorReading.AngularVelocityY) +
                    Math.Abs(sensorReading.AngularVelocityZ)
                    ).ToString();
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public GyrometerSensor(Gyrometer gyrometer)
        {
            _gyrometer = gyrometer;
        }
    }
}