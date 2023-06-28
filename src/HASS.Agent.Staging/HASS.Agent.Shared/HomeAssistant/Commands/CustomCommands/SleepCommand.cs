using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in sleep
    /// <para>Note: this only works when hibernation is disabled</para>
    /// </summary>
    public class SleepCommand : CustomCommand
    {
        private const string DefaultName = "sleep";

        public SleepCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("Rundll32.exe powrprof.dll,SetSuspendState 0,1,0", false, name ?? DefaultName, friendlyName ?? null, entityType, id) => State = "OFF";
    }
}
