﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class BarometerSensor : IInternalDeviceSensor
    {
        private readonly Barometer _barometer;

        public bool Available => _barometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Barometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                return _barometer.GetCurrentReading().StationPressureInHectopascals.ToString();
            }
        }

        public Dictionary<string, string> Attributes => InternalDeviceSensor.NoAttributes;

        public BarometerSensor(Barometer barometer)
        {
            _barometer = barometer;
        }
    }
}
