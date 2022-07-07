using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to shutdown the machine
    /// </summary>
    public class ShutdownCommand : CustomCommand
    {
        public ShutdownCommand(string name = "Shutdown", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /s", false, name ?? "Shutdown", entityType, id) => State = "OFF";
    }
}
