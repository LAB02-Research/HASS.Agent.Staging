using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'previous' mediakey press
    /// </summary>
    public class MediaPreviousCommand : KeyCommand
    {
        public MediaPreviousCommand(string name = "Previous", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_PREV_TRACK, name ?? "Previous", entityType, id) { }
    }
}
