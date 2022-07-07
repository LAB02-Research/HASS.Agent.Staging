using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'playpause' mediakey press
    /// </summary>
    public class MediaPlayPauseCommand : KeyCommand
    {
        public MediaPlayPauseCommand(string name = "PlayPause", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_PLAY_PAUSE, name ?? "PlayPause", entityType, id) { }
    }
}
