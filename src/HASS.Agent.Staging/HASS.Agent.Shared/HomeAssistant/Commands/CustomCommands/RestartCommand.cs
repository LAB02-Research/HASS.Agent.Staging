using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to restart the machine
    /// </summary>
    public class RestartCommand : CustomCommand
    {
        private const string DefaultName = "restart";

        public RestartCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /r", false, name ?? DefaultName, friendlyName ?? null, entityType, id) => State = "OFF";
    }
}
