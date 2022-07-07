using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume up' mediakey press
    /// </summary>
    public class MediaVolumeUpCommand : KeyCommand
    {
        public MediaVolumeUpCommand(string name = "VolumeUp", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_VOLUME_UP, name ?? "VolumeUp", entityType, id) { }
    }
}
