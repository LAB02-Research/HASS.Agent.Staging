using System;
using System.Management;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors
{
    /// <summary>
    /// Sensor containing the result of the provided Powershell command or script
    /// </summary>
    public class PowershellSensor : AbstractSingleValueSensor
    {
        public string Command { get; private set; }

        public PowershellSensor(string command, int? updateInterval = null, string name = "powershellsensor", string id = default) : base(name ?? "powershellsensor", updateInterval ?? 10, id)
        {
            Command = command;
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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{Name}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }
        
        public override string GetState()
        {
            var executed = PowershellManager.ExecuteWithOutput(Command, TimeSpan.FromSeconds(30), out var output, out var errors);
            if (!executed) return "error_during_execution";

            if (string.IsNullOrWhiteSpace(output) && string.IsNullOrWhiteSpace(errors)) return string.Empty;
            if (string.IsNullOrWhiteSpace(output)) return errors.Trim();

            return string.IsNullOrWhiteSpace(errors) ? output : $"{output} | {errors}";
        }

        public override string GetAttributes() => string.Empty;
    }
}