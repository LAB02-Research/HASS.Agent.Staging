using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume up' mediakey press
    /// </summary>
    public class MediaVolumeUpCommand : KeyCommand
    {
        private const string DefaultName = "volumeup";

        public MediaVolumeUpCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_VOLUME_UP, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
