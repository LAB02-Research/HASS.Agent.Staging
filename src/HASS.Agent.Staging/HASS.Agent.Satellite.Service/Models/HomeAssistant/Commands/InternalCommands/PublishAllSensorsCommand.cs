using HASS.Agent.Satellite.Service.Sensors;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;

namespace HASS.Agent.Satellite.Service.Models.HomeAssistant.Commands.InternalCommands
{
    internal class PublishAllSensorsCommand : InternalCommand
    {
        private const string DefaultName = "publishallsensors";

        internal PublishAllSensorsCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string? id = default) : base(name ?? DefaultName, friendlyName ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            SensorsManager.ResetAllSensorChecks();

            State = "OFF";
        }
    }
}