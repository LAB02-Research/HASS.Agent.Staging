using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to lock the current Windows session
    /// </summary>
    public class LockCommand : CustomCommand
    {
        public LockCommand(string name = "Lock", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("Rundll32.exe user32.dll,LockWorkStation", false, name ?? "Lock", entityType, id) => State = "OFF";
    }
}
