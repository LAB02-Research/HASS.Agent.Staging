using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'mute' mediakey press
    /// </summary>
    public class MediaMuteCommand : KeyCommand
    {
        public MediaMuteCommand(string name = "Mute", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_VOLUME_MUTE, name ?? "Mute", entityType, id) { }
    }
}
