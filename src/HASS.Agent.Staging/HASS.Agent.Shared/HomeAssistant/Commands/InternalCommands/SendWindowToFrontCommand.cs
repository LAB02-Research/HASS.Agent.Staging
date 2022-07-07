using System;
using System.Diagnostics;
using System.IO;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to perform an action through the configured custom executor
    /// </summary>
    public class SendWindowToFrontCommand : InternalCommand
    {
        public SendWindowToFrontCommand(string name = "SendWindowToFront", string process = "", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? "SendWindowToFront", process, entityType, id)
        {
            CommandConfig = process;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Warning("[SENDWINDOWTOFRONT] Unable to launch command '{name}', it's configured as action-only", Name);

                State = "OFF";
                return;
            }

            ProcessManager.BringMainWindowToFront(CommandConfig);

            State = "OFF";
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(action))
            {
                Log.Warning("[SENDWINDOWTOFRONT] Unable to launch command '{name}', empty action provided", Name);

                State = "OFF";
                return;
            }

            if (!string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Warning("[SENDWINDOWTOFRONT] Command '{name}' launched by action, command-provided process will be ignored", Name);
            }

            ProcessManager.BringMainWindowToFront(action);

            State = "OFF";
        }
    }
}
