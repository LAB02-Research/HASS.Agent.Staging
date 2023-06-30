using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in hibernation
    /// </summary>
    public class HibernateCommand : CustomCommand
    {
        private const string DefaultName = "hibernate";

        public HibernateCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /h", false, name ?? DefaultName, friendlyName ?? null, entityType, id) => State = "OFF";
    }
}
