using CoreAudio;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    public class SetAudioOutputCommand : InternalCommand
    {
        private const string DefaultName = "setaudiooutput";

        private string OutputDevice { get => CommandConfig; }

        public SetAudioOutputCommand(string name = DefaultName, string friendlyName = DefaultName, string audioDevice = "", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(name ?? DefaultName, friendlyName ?? null, audioDevice, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            if (string.IsNullOrWhiteSpace(OutputDevice))
            {
                Log.Error("[SETAUDIOOUT] Error, output device name cannot be null/blank");

                return;
            }

            TurnOnWithAction(OutputDevice);
        }

        private MMDevice GetAudioDeviceOrDefault(string playbackDeviceName)
        {
            var devices = Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.eRender, DeviceState.Active);
            var playbackDevice = devices.Where(d => d.DeviceFriendlyName == playbackDeviceName).FirstOrDefault();

            return playbackDevice ?? Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia);
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";
 
            try
            {
                var outputDevice = GetAudioDeviceOrDefault(action);
                if (outputDevice == Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia))
                    return;

                outputDevice.Selected = true;
            }
            catch (Exception ex)
            {
                Log.Error("[SETAUDIOOUT] Error while processing action '{action}': {err}", action, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }
    }
}
