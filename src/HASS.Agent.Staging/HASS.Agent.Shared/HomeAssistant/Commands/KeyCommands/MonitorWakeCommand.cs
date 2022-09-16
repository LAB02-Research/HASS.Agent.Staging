using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'arrow up' key press to wake the monitors
    /// https://stackoverflow.com/a/42393472 ?
    /// </summary>
    public class MonitorWakeCommand : KeyCommand
    {
        public MonitorWakeCommand(string name = "MonitorWake", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(KEY_UP, name ?? "MonitorWake", entityType, id) { }
    }
}
