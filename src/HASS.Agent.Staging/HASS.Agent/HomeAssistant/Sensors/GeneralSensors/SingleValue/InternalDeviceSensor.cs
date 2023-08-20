using HASS.Agent.Managers;
using HASS.Agent.Managers.DeviceSensors;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the device's internal sensor data
    /// </summary>
    public class InternalDeviceSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "internaldevicesensor";

        public InternalDeviceSensorType SensorType { get; set; }

        public InternalDeviceSensor(string sensorType, int? updateInterval = 10, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            SensorType = Enum.Parse<InternalDeviceSensorType>(sensorType);
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
                Icon = "mdi:information-box-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState() => SystemStateManager.LastMonitorPowerEvent.ToString();

        public override string GetAttributes() => string.Empty;
    }
}
