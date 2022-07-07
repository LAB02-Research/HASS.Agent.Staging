using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume down' mediakey press
    /// </summary>
    public class MediaVolumeDownCommand : KeyCommand
    {
        public MediaVolumeDownCommand(string name = "VolumeDown", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_VOLUME_DOWN, name ?? "VolumeDown", entityType, id) { }
    }
}
