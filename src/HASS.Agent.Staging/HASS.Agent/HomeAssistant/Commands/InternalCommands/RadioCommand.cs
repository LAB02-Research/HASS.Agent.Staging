using HASS.Agent.Managers;
using HASS.Agent.Sensors;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;
using HASS.Agent.Shared.Models.HomeAssistant;
using Windows.Devices.Radios;

namespace HASS.Agent.HomeAssistant.Commands.InternalCommands
{
    internal class RadioCommand : InternalCommand
    {
        private const string DefaultName = "radiocommand";

        private readonly Radio _radio;

        public string RadioName { get; set; }

        internal RadioCommand(string radioName, string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? DefaultName, friendlyName ?? null, string.Empty, entityType, id)
        {
            RadioName = radioName;
            _radio = RadioManager.AvailableRadio.First(r => r.Name == radioName);
            _radio.StateChanged += RadioStateChanged;
            State = _radio.State == RadioState.On ? "ON" : "OFF";
        }

        private void RadioStateChanged(Radio sender, object args)
        {
            State = sender.State == RadioState.On ? "ON" : "OFF";
        }

        public override void TurnOn()
        {
            var success = _radio.SetStateAsync(RadioState.On).AsTask().Result;
            if (success == RadioAccessStatus.Allowed)
                State = "ON";
        }

        public override void TurnOff()
        {
            var success = _radio.SetStateAsync(RadioState.Off).AsTask().Result;
            if (success == RadioAccessStatus.Allowed)
                State = "OFF";
        }
    }
}
