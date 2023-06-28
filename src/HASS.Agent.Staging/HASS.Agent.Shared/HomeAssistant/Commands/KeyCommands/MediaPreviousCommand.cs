using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'previous' mediakey press
    /// </summary>
    public class MediaPreviousCommand : KeyCommand
    {
        private const string DefaultName = "previous";

        public MediaPreviousCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_PREV_TRACK, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
