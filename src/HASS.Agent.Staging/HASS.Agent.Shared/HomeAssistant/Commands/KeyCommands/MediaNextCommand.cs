using HASS.Agent.Shared.Enums;
using static HASS.Agent.Shared.Functions.Inputs;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'next' mediakey press
    /// </summary>
    public class MediaNextCommand : KeyCommand
    {
        private const string DefaultName = "next";

        public MediaNextCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VirtualKeyShort.MEDIA_NEXT_TRACK, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
