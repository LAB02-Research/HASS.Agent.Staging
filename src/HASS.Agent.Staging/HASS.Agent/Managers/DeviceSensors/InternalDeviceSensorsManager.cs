using Syncfusion.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;

namespace HASS.Agent.Managers.DeviceSensors
{


    internal static class InternalDeviceSensorsManager
    {
        private static readonly List<IInternalDeviceSensor> deviceSensors = new();
        private static DeviceWatcher deviceWatcher;

        public static List<IInternalDeviceSensor> AvailableSensors => deviceSensors.FindAll(s => s.Available);

        public static async Task Initialize()
        {
            deviceSensors.Add(new AccelerometerSensor(Accelerometer.GetDefault()));
            deviceSensors.Add(new ActivitySensor(await Windows.Devices.Sensors.ActivitySensor.GetDefaultAsync()));
            deviceSensors.Add(new AltimeterSensor(Altimeter.GetDefault()));
            deviceSensors.Add(new BarometerSensor(Barometer.GetDefault()));
            deviceSensors.Add(new CompassSensor(Compass.GetDefault()));
            deviceSensors.Add(new GyrometerSensor(Gyrometer.GetDefault()));
            deviceSensors.Add(new HingeAngleSensor(await Windows.Devices.Sensors.HingeAngleSensor.GetDefaultAsync()));
            deviceSensors.Add(new InclinometerSensor(Inclinometer.GetDefault()));
            deviceSensors.Add(new MagnetometerSensor(Magnetometer.GetDefault()));
            deviceSensors.Add(new OrientationSensor(Windows.Devices.Sensors.OrientationSensor.GetDefault()));
            deviceSensors.Add(new PedometerSensor(await Pedometer.GetDefaultAsync()));
            deviceSensors.Add(new ProximitySensor(null));
            deviceSensors.Add(new SimpleOrientationSensor(Windows.Devices.Sensors.SimpleOrientationSensor.GetDefault()));

            deviceWatcher = DeviceInformation.CreateWatcher(Windows.Devices.Sensors.ProximitySensor.GetDeviceSelector());
            deviceWatcher.Added += OnProximitySensorAdded;
        }

        private static void OnProximitySensorAdded(DeviceWatcher sender, DeviceInformation args)
        {
            var proximitySensor = Windows.Devices.Sensors.ProximitySensor.FromId(args.Id);
            var storedProximitySensor = deviceSensors.FirstOrDefault(s => s.Type == InternalDeviceSensorType.ProximitySensor);

            if (proximitySensor != null && storedProximitySensor.Available == false)
            {
                deviceSensors.Remove(storedProximitySensor);
                deviceSensors.Add(new ProximitySensor(proximitySensor));
            }
        }
    }
}
