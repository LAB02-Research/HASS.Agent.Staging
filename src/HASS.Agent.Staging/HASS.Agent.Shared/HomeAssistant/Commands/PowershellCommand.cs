﻿using System.Diagnostics;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to perform an action or script through Powershell
    /// </summary>
    public class PowershellCommand : AbstractCommand
    {
        private const string DefaultName = "powershell";

        public string Command { get; protected set; }
        public string State { get; protected set; }
        public Process Process { get; set; }

        private readonly bool _isScript = false;
        private readonly string _descriptor = "command";

        public PowershellCommand(string command, string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? DefaultName, friendlyName ?? null, entityType, id)
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
                Log.Warning("[POWERSHELL] [{name}] Unable to execute, it's configured as action-only", Name);

                State = "OFF";
                return;
            }

            var executed = _isScript
                ? PowershellManager.ExecuteScriptHeadless(Command, string.Empty)
                : PowershellManager.ExecuteCommandHeadless(Command);

            if (!executed) Log.Error("[POWERSHELL] [{name}] Executing {descriptor} failed", Name, _descriptor, Name);
            
            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            var executed = _isScript
                ? PowershellManager.ExecuteScriptHeadless(Command, action)
                : PowershellManager.ExecuteCommandHeadless(Command);

            if (!executed) Log.Error("[POWERSHELL] [{name}] Launching PS {descriptor} with action '{action}' failed", Name, _descriptor, action);
            
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
                FriendlyName = FriendlyName,
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
