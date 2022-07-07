using System.Globalization;
using System.Linq;
using HASS.Agent.Shared.Models.HomeAssistant;
using LibreHardwareMonitor.Hardware;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the current GPU temp
    /// </summary>
    public class GpuTemperatureSensor : AbstractSingleValueSensor
    {
        private readonly IHardware _gpu;

        public GpuTemperatureSensor(int? updateInterval = null, string name = "gputemperature", string id = default) : base(name ?? "gputemperature", updateInterval ?? 30, id)
        {
            var computer = new Computer
            {
                IsCpuEnabled = false,
                IsGpuEnabled = true,
                IsMemoryEnabled = false,
                IsMotherboardEnabled = false,
                IsControllerEnabled = false,
                IsNetworkEnabled = false,
                IsStorageEnabled = false,
            };

            computer.Open();
            _gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia);

            computer.Close();
        }

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
                Device_class = "temperature",
                Unit_of_measurement = "°C",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            if (_gpu == null) return "NotSupported";

            _gpu.Update();
            var sensor = _gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            if (sensor?.Value == null) return "NotSupported";

            return sensor.Value.HasValue ? sensor.Value.Value.ToString("#.##", CultureInfo.InvariantCulture) : "Unknown";
        }
    }
}
