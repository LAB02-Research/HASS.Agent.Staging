using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal class AccelerometerSensor : IDeviceSensor
    {
        public const string AttributeLastShaken = "LastShaken";

        private readonly Accelerometer _accelerometer;

        public bool Available => _accelerometer != null;
        public DeviceSensorType Type => DeviceSensorType.Accelerometer;
        public string Measurement => _accelerometer?.GetCurrentReading().ToString();

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
