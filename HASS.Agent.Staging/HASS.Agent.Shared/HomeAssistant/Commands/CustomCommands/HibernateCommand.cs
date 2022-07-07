using HASS.Agent.Shared.Enums;

namespace HASS.Agent.Shared.HomeAssistant.Commands.CustomCommands
{
    /// <summary>
    /// Command to put Windows in hibernation
    /// </summary>
    public class HibernateCommand : CustomCommand
    {
        public HibernateCommand(string name = "Hibernate", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base("shutdown /h", false, name ?? "Hibernate", entityType, id) => State = "OFF";
    }
}
