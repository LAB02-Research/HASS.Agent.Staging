using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'previous' mediakey press
    /// </summary>
    public class MediaPreviousCommand : KeyCommand
    {
        private const string DefaultName = "previous";

        public MediaPreviousCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.MEDIA_PREV_TRACK, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
