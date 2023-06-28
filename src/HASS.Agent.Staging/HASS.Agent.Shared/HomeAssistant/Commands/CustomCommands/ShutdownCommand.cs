using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to shutdown the machine
    /// </summary>
    public class ShutdownCommand : CustomCommand
    {
        private const string DefaultName = "shutdown";

        public ShutdownCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /s", false, name ?? DefaultName, friendlyName ?? null, entityType, id) => State = "OFF";
    }
}
