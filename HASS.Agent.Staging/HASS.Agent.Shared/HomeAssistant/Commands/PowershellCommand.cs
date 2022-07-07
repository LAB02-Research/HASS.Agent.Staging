using System.Diagnostics;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to perform an action or script through Powershell
    /// </summary>
    public class PowershellCommand : AbstractCommand
    {
        public string Command { get; protected set; }
        public string State { get; protected set; }
        public Process Process { get; set; }

        private readonly bool _isScript = false;
        private readonly string _descriptor = "command";

        public PowershellCommand(string command, string name = "Powershell", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? "Powershell", entityType, id)
        {
            Command = command;
            if (Command.ToLower().EndsWith(".ps1"))
            {
                _isScript = true;
                _descriptor = "script";
            }

            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(Command))
            {
                Log.Warning("[COMMAND] Unable to launch PS '{name}', it's configured as action-only", Name);

                State = "OFF";
                return;
            }

            var executed = _isScript
                ? PowershellManager.ExecuteScriptHeadless(Command)
                : PowershellManager.ExecuteCommandHeadless(Command);

            if (!executed)
            {
                Log.Error("[COMMAND] Launching PS {descriptor} '{name}' failed", _descriptor, Name);
                State = "FAILED";
            }
            else State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            // prepare command
            var command = string.IsNullOrWhiteSpace(Command) ? action : $"{Command} {action}";

            var executed = _isScript
                ? PowershellManager.ExecuteScriptHeadless(command)
                : PowershellManager.ExecuteCommandHeadless(command);

            if (!executed)
            {
                Log.Error("[COMMAND] Launching PS {descriptor} '{name}' with action '{action}' failed", _descriptor, Name, action);
                State = "FAILED";
            }
            else State = "OFF";
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel()
            {
                Name = Name,
                Unique_id = Id,
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Command_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/set",
                Action_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/action",
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Device = deviceConfig,
            };
        }

        public override string GetState() => State;

        public override void TurnOff() => Process?.Kill();
    }
}
