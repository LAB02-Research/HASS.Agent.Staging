using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.PerfCounterSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the current CPU load
    /// </summary>
    public class CpuLoadSensor : PerformanceCounterSensor
    {
        private const string DefaultName = "cpuload";

        public CpuLoadSensor(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default, bool applyRounding = false, int? round = null) : base("Processor", "% Processor Time", "_Total", applyRounding, round, updateInterval ?? 30, name ?? DefaultName, friendlyName ?? null, id) { }

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
                Icon = "mdi:chart-areaspline",
                Unit_of_measurement = "%",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }
    }
}
