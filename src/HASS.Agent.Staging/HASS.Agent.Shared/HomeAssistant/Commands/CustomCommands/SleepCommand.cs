using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in sleep
    /// <para>Note: this only works when hibernation is disabled</para>
    /// </summary>
    public class SleepCommand : CustomCommand
    {
        public SleepCommand(string name = "Sleep", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("Rundll32.exe powrprof.dll,SetSuspendState 0,1,0", false, name ?? "Sleep", entityType, id) => State = "OFF";
    }
}
