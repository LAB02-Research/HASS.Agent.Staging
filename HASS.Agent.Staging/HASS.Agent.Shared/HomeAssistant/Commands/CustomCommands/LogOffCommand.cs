using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to log off the current Windows session
    /// </summary>
    public class LogOffCommand : CustomCommand
    {
        public LogOffCommand(string name = "LogOff", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /l", false, name ?? "LogOff", entityType, id) => State = "OFF";
    }
}
