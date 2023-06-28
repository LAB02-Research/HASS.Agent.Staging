using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume down' mediakey press
    /// </summary>
    public class MediaVolumeDownCommand : KeyCommand
    {
        private const string DefaultName = "volumedown";

        public MediaVolumeDownCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_VOLUME_DOWN, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
