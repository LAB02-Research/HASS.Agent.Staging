using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to restart the machine
    /// </summary>
    public class RestartCommand : CustomCommand
    {
        public RestartCommand(string name = "Restart", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /r", false, name ?? "Restart", entityType, id) => State = "OFF";
    }
}
