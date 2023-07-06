using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'volume down' mediakey press
    /// </summary>
    public class MediaVolumeDownCommand : KeyCommand
    {
        private const string DefaultName = "volumedown";

        public MediaVolumeDownCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.VOLUME_DOWN, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
