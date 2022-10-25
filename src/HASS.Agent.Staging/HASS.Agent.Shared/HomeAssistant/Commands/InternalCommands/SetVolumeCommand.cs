using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using CoreAudio;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Functions;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to set the system's audio volume level
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class SetVolumeCommand : InternalCommand
    {
        private static float _volume = -1f;

        public SetVolumeCommand(string name = "SetVolume", string volume = "", CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(name ?? "SetVolume", volume, entityType, id)
        {
            if (!string.IsNullOrWhiteSpace(volume))
            {
                var parsed = int.TryParse(volume, out var volumeInt);
                if (!parsed)
                {
                    Log.Error("[SETVOLUME] [{name}] Unable to parse configured volume level, not an int: {val}", Name, volume);
                    _volume = -1f;
                }

                _volume = volumeInt / 100.0f;
            }

            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            try
            {
                // do we have a value to work with?
                if (_volume == -1f)
                {
                    Log.Warning("[SETVOLUME] [{name}] Unable to trigger command, it's configured as action-only", Name);
                    return;
                }

                // get the current default endpoint
                using var audioDevice = Variables.AudioDeviceEnumerator?.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia);
                if (audioDevice?.AudioEndpointVolume == null)
                {
                    Log.Warning("[SETVOLUME] [{name}] Unable to trigger command, no default audio endpoint found", Name);
                    return;
                }

                // all good, set the volume
                audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = _volume;
            }
            catch (Exception ex)
            {
                Log.Error("[SETVOLUME] [{name}] Error while processing: {err}", Name, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            try
            {
                // check if we got a valid number
                var parsed = int.TryParse(action, out var volumeInt);
                if (!parsed)
                {
                    Log.Error("[SETVOLUME] [{name}] Unable to trigger command, the provided action value can't be parsed: {val}", Name, action);
                    return;
                }

                // get the current default endpoint
                using var audioDevice = Variables.AudioDeviceEnumerator?.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia);
                if (audioDevice?.AudioEndpointVolume == null)
                {
                    Log.Warning("[SETVOLUME] [{name}] Unable to trigger action for command, no default audio endpoint found", Name);
                    return;
                }

                // allg ood, set the volume
                audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumeInt / 100.0f; ;
            }
            catch (Exception ex)
            {
                Log.Error("[SETVOLUME] [{name}] Error while processing action: {err}", Name, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }
    }
}
