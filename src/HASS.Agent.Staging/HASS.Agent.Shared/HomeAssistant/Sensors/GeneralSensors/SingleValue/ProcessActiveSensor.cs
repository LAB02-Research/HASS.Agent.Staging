using System.Diagnostics;
using System.IO;
using System.Linq;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating how many (if any) instances of the provided process are active
    /// </summary>
    public class ProcessActiveSensor : AbstractSingleValueSensor
    {
        public string ProcessName { get; protected set; }

        public ProcessActiveSensor(string processName, int? updateInterval = null, string name = "processactive", string id = default) : base(name ?? "processactive", updateInterval ?? 10, id) => ProcessName = processName;

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
                Icon = "mdi:file-eye-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            // we don't need the extension
            if (ProcessName.Contains(".")) ProcessName = Path.GetFileNameWithoutExtension(ProcessName);

            // search for our process
            var procs = Process.GetProcessesByName(ProcessName);
            var instanceCount = procs.Any() ? procs.Length : 0;

            // dispose all objects
            foreach (var proc in procs) proc?.Dispose();

            // done
            return instanceCount.ToString();
        }
    }
}
