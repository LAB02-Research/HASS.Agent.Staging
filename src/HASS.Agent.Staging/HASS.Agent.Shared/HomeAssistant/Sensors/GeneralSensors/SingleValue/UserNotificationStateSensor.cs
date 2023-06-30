using System.Runtime.InteropServices;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the current Windows notifications state
    /// </summary>
    public class UserNotificationStateSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "notificationstate";

        public UserNotificationStateSensor(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 10, id) { }

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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{Name}/state",
                Icon = "mdi:laptop",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState() => GetStateEnum().ToString();

        [DllImport("shell32.dll")]
        private static extern int SHQueryUserNotificationState(out UserNotificationState state);

        public UserNotificationState GetStateEnum()
        {
            SHQueryUserNotificationState(out var state);
            return state;
        }

        public override string GetAttributes() => string.Empty;
    }
}
