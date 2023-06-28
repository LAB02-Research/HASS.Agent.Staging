using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'playpause' mediakey press
    /// </summary>
    public class MediaPlayPauseCommand : KeyCommand
    {
        private const string DefaultName = "playpause";

        public MediaPlayPauseCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_PLAY_PAUSE, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
