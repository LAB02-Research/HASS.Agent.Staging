using CoreAudio;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    public class SetApplicationVolumeCommand : InternalCommand
    {
        private const string DefaultName = "setappvolume";
        private static readonly Dictionary<int, string> ApplicationNames = new Dictionary<int, string>();

        public SetApplicationVolumeCommand(string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Button, string id = default) : base(name ?? DefaultName, friendlyName ?? null, string.Empty, entityType, id)
        {
            State = "OFF";
        }

        public override void TurnOn()
        {
            Log.Error("[SETAPPVOLUME] [{name}] Error, this command can be run only with action");
        }

        private MMDevice GetAudioDeviceOrDefault(string playbackDeviceName)
        {
            var devices = Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.eRender, DeviceState.Active);
            var playbackDevice = devices.Where(d => d.DeviceFriendlyName == playbackDeviceName).FirstOrDefault();

            return playbackDevice ?? Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia);
        }

        private string GetSessionDisplayName(AudioSessionControl2 session)
        {
            var procId = (int)session.ProcessID;

            if (procId <= 0)
                return session.DisplayName;

            if (ApplicationNames.ContainsKey(procId))
                return ApplicationNames[procId];

            using var p = Process.GetProcessById(procId);
            ApplicationNames.Add(procId, p.ProcessName);

            return p.ProcessName;
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            try
            {
                var actionData = JsonConvert.DeserializeObject<ApplicationVolumeAction>(action);

                if (string.IsNullOrWhiteSpace(actionData.ApplicationName))
                {
                    Log.Error("[SETAPPVOLUME] Error, this command can be run only with action");

                    return;
                }

                var audioDevice = GetAudioDeviceOrDefault(actionData.PlaybackDevice);

                var session = audioDevice.AudioSessionManager2?.Sessions?.Where(s =>
                    s != null &&
                    actionData.ApplicationName == GetSessionDisplayName(s)
                ).FirstOrDefault();

                if (session == null)
                {
                    Log.Error("[SETAPPVOLUME] Error, no session of application {app} can be found", actionData.ApplicationName);

                    return;
                }

                session.SimpleAudioVolume.Mute = actionData.Mute;
                if (actionData.Volume == -1)
                {
                    Log.Debug("[SETAPPVOLUME] No volume value provided, only mute has been set for {app}", actionData.ApplicationName);

                    return;
                }

                var volume = Math.Clamp(actionData.Volume, 0, 100) / 100.0f;
                session.SimpleAudioVolume.MasterVolume = volume;
            }
            catch (Exception ex)
            {
                Log.Error("[SETAPPVOLUME] Error while processing action: {err}", ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        private class ApplicationVolumeAction
        {
            public int Volume { get; set; } = -1;
            public bool Mute { get; set; } = false;
            public string ApplicationName { get; set; } = string.Empty;
            public string PlaybackDevice { get; set; } = string.Empty;
        }
    }
}
