using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class AccelerometerSensor : IInternalDeviceSensor
    { 
        public const string AttributeAccelerationX = "AccelerationX";
        public const string AttributeAccelerationY = "AccelerationY";
        public const string AttributeAccelerationZ = "AccelerationZ";
        public const string AttributeLastShaken = "LastShaken";

        private readonly Accelerometer _accelerometer;

        public bool Available => _accelerometer != null;
        public InternalDeviceSensorType Type => InternalDeviceSensorType.Accelerometer;
        public string Measurement
        {
            get
            {
                if (!Available)
                    return null;

                var sensorReading = _accelerometer.GetCurrentReading();
                _attributes[AttributeAccelerationX] = sensorReading.AccelerationX.ToString();
                _attributes[AttributeAccelerationY] = sensorReading.AccelerationY.ToString();
                _attributes[AttributeAccelerationZ] = sensorReading.AccelerationZ.ToString();

                return (
                    Math.Abs(sensorReading.AccelerationX) +
                    Math.Abs(sensorReading.AccelerationY) +
                    Math.Abs(sensorReading.AccelerationZ)
                    ).ToString();
            }
        }

        private readonly Dictionary<string, string> _attributes = new();
        public Dictionary<string, string> Attributes => _attributes;

        public AccelerometerSensor(Accelerometer accelerometer)
        {
            _accelerometer = accelerometer;
            if (_accelerometer != null)
                _accelerometer.Shaken += OnAccelerometerShake;
        }

        private void OnAccelerometerShake(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            _attributes[AttributeLastShaken] = args.Timestamp.ToLocalTime().ToString();
        }
    }
}
