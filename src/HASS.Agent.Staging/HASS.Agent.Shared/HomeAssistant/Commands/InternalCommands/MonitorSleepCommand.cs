using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to put all monitors to sleep
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MonitorSleepCommand : InternalCommand
    {
        public MonitorSleepCommand(string name = "MonitorSleep", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(name ?? "MonitorSleep", string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_MONITORPOWER, (IntPtr)2);

            State = "OFF";
        }
    }
}
