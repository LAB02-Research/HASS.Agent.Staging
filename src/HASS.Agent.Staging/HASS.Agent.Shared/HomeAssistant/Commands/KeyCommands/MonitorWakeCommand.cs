using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'arrow up' key press to wake the monitors
    /// https://stackoverflow.com/a/42393472 ?
    /// </summary>
    public class MonitorWakeCommand : KeyCommand
    {
        private const string DefaultName = "monitorwake";

        public MonitorWakeCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(KEY_UP, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
