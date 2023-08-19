using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Managers.DeviceSensors
{
    internal enum DeviceSensorType
    {
        Accelerometer = 0,
        ActivitySensor,
        Altimeter,
        Barometer,
        Compass,
        Gyrometer,
        HingeAngleSensor,
        HumanPresenceSensor,
        Inclinometer,
        LightSensor,
        Magnetometer,
        OrientationSensor,
        Pedometer,
        ProximitySensor,
        SimpleOrientationSensor
    }

    internal class DeviceSensor
    {
        internal static readonly Dictionary<string, string> NoAttributes = new();
    }

    internal interface IDeviceSensor
    {
        public bool Available { get; }
        public DeviceSensorType Type { get; }
        public string Measurement { get; }
        public Dictionary<string, string> Attributes { get; }
    }
}
