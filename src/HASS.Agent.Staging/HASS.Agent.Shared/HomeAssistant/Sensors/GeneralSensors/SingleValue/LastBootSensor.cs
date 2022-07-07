using System;
using System.Runtime.InteropServices;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the moment the system last booted
    /// <para>Note: value can be incorrect when fastboot is enabled</para>
    /// </summary>
    public class LastBootSensor : AbstractSingleValueSensor
    {
        public LastBootSensor(int? updateInterval = 10, string name = "lastboot", string id = default) : base(name ?? "lastboot", updateInterval ?? 10, id) { }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:clock-time-three-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Device_class = "timestamp"
            });
        }

        public override string GetState() => (DateTime.Now - TimeSpan.FromMilliseconds(GetTickCount64())).ToTimeZoneString();

        [DllImport("kernel32")]
        private static extern ulong GetTickCount64();
    }
}
