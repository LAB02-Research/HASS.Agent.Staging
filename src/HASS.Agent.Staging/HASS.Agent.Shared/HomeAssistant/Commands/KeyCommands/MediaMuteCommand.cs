using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'mute' mediakey press
    /// </summary>
    public class MediaMuteCommand : KeyCommand
    {
        private const string DefaultName = "mute";

        public MediaMuteCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.VOLUME_MUTE, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
