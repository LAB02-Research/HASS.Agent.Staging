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


    internal static class DeviceSensorsManager
    {
        /*        private static Accelerometer s_accelerometer;
                private static ActivitySensor s_activitySensor;
                private static Altimeter s_altimeter;
                private static Barometer s_barometer;
                private static Compass s_compass;
                private static Gyrometer s_gyrometer;
                private static HingeAngleSensor s_hingeAngleSensor;
                //private static HumanPresenceSensor s_humanPresenceSensor;
                private static Inclinometer s_inclinometer;
                private static LightSensor s_lightSensor;
                private static Magnetometer s_magnetometer;
                private static OrientationSensor s_orientationSensor;
                private static Pedometer s_pedometer;

                private static DeviceWatcher deviceWatcher;
                private static ProximitySensor s_proximitySensor;

                private static SimpleOrientationSensor s_simpleOrientationSensor;*/

        /*        private static readonly List<string> s_sensorNames = new()
                {
                    nameof(Accelerometer),
                    nameof(ActivitySensor),
                    nameof(Altimeter),
                    nameof(Barometer),
                    nameof(Compass),
                    nameof(Gyrometer),
                    nameof(HingeAngleSensor),
                    nameof(Inclinometer),
                    nameof(LightSensor),
                    nameof(Magnetometer),
                    nameof(OrientationSensor),
                    nameof(Pedometer),
                    nameof(ProximitySensor),
                    nameof(SimpleOrientationSensor)
                };*/

        private static List<IDeviceSensor> deviceSensors = new List<IDeviceSensor>();

        public static async Task Initialize()
        {
            deviceSensors.Add(new AccelerometerSensor(Accelerometer.GetDefault()));


            /*            s_accelerometer = Accelerometer.GetDefault();
                        s_activitySensor = await ActivitySensor.GetDefaultAsync();
                        s_altimeter = Altimeter.GetDefault();
                        s_barometer = Barometer.GetDefault();
                        s_compass = Compass.GetDefault();
                        s_gyrometer = Gyrometer.GetDefault();
                        s_hingeAngleSensor = await HingeAngleSensor.GetDefaultAsync();
                        s_inclinometer = Inclinometer.GetDefault();
                        s_lightSensor = LightSensor.GetDefault();
                        s_magnetometer = Magnetometer.GetDefault();
                        s_orientationSensor = OrientationSensor.GetDefault();
                        s_pedometer = await Pedometer.GetDefaultAsync();

                        deviceWatcher = DeviceInformation.CreateWatcher(ProximitySensor.GetDeviceSelector());
                        deviceWatcher.Added += OnProximitySensorAdded;

                        s_simpleOrientationSensor = SimpleOrientationSensor.GetDefault();*/
        }

        private static void OnProximitySensorAdded(DeviceWatcher sender, DeviceInformation args)
        {
            var proximitySensor = ProximitySensor.FromId(args.Id);
            if (proximitySensor != null && s_proximitySensor == null)
                s_proximitySensor = proximitySensor;
        }
    }
}
