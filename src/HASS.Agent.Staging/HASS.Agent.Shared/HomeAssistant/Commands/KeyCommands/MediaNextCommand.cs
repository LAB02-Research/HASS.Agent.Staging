using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.KeyCommands
{
    /// <summary>
    /// Simulates a 'next' mediakey press
    /// </summary>
    public class MediaNextCommand : KeyCommand
    {
        private const string DefaultName = "next";

        public MediaNextCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(VK_MEDIA_NEXT_TRACK, name ?? DefaultName, friendlyName ?? null, entityType, id) { }
    }
}
