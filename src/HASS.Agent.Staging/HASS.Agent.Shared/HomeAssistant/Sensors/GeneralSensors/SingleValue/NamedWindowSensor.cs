using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating whether the provided windowname is currently open (doesn't need focus)
    /// </summary>
    public class NamedWindowSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "namedwindow";
        public string WindowName { get; protected set; }

        public NamedWindowSensor(string windowName, string name = DefaultName, string friendlyName = DefaultName, int? updateInterval = 10, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            Domain = "binary_sensor";
            WindowName = windowName;
        }

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
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            var windowNames = SharedHelperFunctions.GetOpenWindows().Values;
            return windowNames.Any(v => v.ToUpper().Contains(WindowName.ToUpper())) ? "ON" : "OFF";
        }

        public override string GetAttributes() => string.Empty;
    }
}
