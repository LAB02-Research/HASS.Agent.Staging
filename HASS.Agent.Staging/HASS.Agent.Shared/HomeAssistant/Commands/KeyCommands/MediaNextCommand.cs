using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'next' mediakey press
    /// </summary>
    public class MediaNextCommand : KeyCommand
    {
        public MediaNextCommand(string name = "Next", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_NEXT_TRACK, name ?? "Next", entityType, id) { }
    }
}
