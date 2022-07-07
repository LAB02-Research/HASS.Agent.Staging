using System.Diagnostics;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to perform an action through a console, either normal or with low integrity
    /// </summary>
    public class CustomCommand : AbstractCommand
    {
        public string Command { get; protected set; }
        public string State { get; protected set; }
        public bool RunAsLowIntegrity { get; protected set; }
        public Process Process { get; set; } = null;

        public CustomCommand(string command, bool runAsLowIntegrity, string name = "Custom", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? "Custom", entityType, id)
        {
            Command = command;
            RunAsLowIntegrity = runAsLowIntegrity;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(Command))
            {
                Log.Warning("[COMMAND] Unable to launch command '{name}', it's configured as action-only", Name);

                State = "OFF";
                return;
            }

            if (RunAsLowIntegrity) CommandLineManager.LaunchAsLowIntegrity(Command);
            else
            {
                var executed = CommandLineManager.ExecuteHeadless(Command);

                if (!executed)
                {
                    Log.Error("[COMMAND] Launching command '{name}' failed", Name);
                    State = "FAILED";
                    return;
                }
            }

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            // prepare command
            var command = string.IsNullOrWhiteSpace(Command) ? action : $"{Command} {action}";

            if (RunAsLowIntegrity) CommandLineManager.LaunchAsLowIntegrity(command);
            else
            {
                var executed = !string.IsNullOrWhiteSpace(Command)
                    ? CommandLineManager.Execute(Command, action)
                    : CommandLineManager.ExecuteHeadless(action);

                if (!executed)
                {
                    Log.Error("[COMMAND] Launching command '{name}' with action '{action}' failed", Name, action);
                    State = "FAILED";
                    return;
                }
            }

            State = "OFF";
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
