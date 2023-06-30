using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to log off the current Windows session
    /// </summary>
    public class LogOffCommand : CustomCommand
    {
        private const string DefaultName = "logoff";

        public LogOffCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /l", false, name ?? DefaultName, friendlyName ?? null, entityType, id) => State = "OFF";
    }
}
