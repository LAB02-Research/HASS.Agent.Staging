using System;
using System.Runtime.InteropServices;
using System.Text;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the ID of the currently active virtual desktop
    /// Optionally the virtual desktop name is returned in sensor attributes
    /// </summary>
    public class ActiveDesktopSensor : AbstractSingleValueSensor
    {
        private const string _defaultName = "activedesktop";

        private string _desktopName = default;

        public ActiveDesktopSensor(int? updateInterval = null, string name = _defaultName, string friendlyName = _defaultName, string id = default) : base(name ?? _defaultName, friendlyName ?? null, updateInterval ?? 15, id) { }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:monitor",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            return "";
        }

        public override string GetAttributes() => string.Empty;


    }
}
