using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using CoreAudio;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using HidSharp;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing audio-related info
    /// </summary>
    public class AudioSensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "audio";
        private static readonly Dictionary<int, string> ApplicationNames = new Dictionary<int, string>();
        private bool _errorPrinted = false;

        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public AudioSensors(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 20, id)
        {
            _updateInterval = updateInterval ?? 20;

            UpdateSensorValues();
        }

        private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
        {
            if (!Sensors.ContainsKey(sensorId))
                Sensors.Add(sensorId, sensor);
            else
                Sensors[sensorId] = sensor;
        }

        private List<string> GetAudioOutputDevices()
        {
            var audioOutputDevices = new List<string>();
            foreach (var device in Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.eRender, DeviceState.Active))
            {
                audioOutputDevices.Add(device.DeviceFriendlyName);
                device.Dispose();
            }

            return audioOutputDevices;
        }

        private List<string> GetAudioInputDevices()
        {
            var audioOutputDevices = new List<string>();
            foreach (var device in Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.eCapture, DeviceState.Active))
            {
                audioOutputDevices.Add(device.DeviceFriendlyName);
                device.Dispose();
            }

            return audioOutputDevices;
        }

        private void HandleAudioOutputSensors(string parentSensorSafeName)
        {
            using (var audioDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia))
            {
                var defaultDeviceId = $"{parentSensorSafeName}_default_device";
                var defaultDeviceSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Device", defaultDeviceId, string.Empty, "mdi:speaker", string.Empty, Name);
                defaultDeviceSensor.SetState(audioDevice.DeviceFriendlyName);
                AddUpdateSensor(defaultDeviceId, defaultDeviceSensor);

                var defaultDeviceStateId = $"{parentSensorSafeName}_default_device_state";
                var defaultDeviceStateSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Device State", defaultDeviceStateId, string.Empty, "mdi:speaker", string.Empty, Name);
                defaultDeviceStateSensor.SetState(GetReadableState(audioDevice.State));
                AddUpdateSensor(defaultDeviceStateId, defaultDeviceStateSensor);

                var masterVolume = Convert.ToInt32(Math.Round(audioDevice.AudioEndpointVolume?.MasterVolumeLevelScalar * 100 ?? 0, 0));
                var defaultDeviceVolumeId = $"{parentSensorSafeName}_default_device_volume";
                var defaultDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Default Device Volume", defaultDeviceVolumeId, string.Empty, "mdi:speaker", string.Empty, Name);
                defaultDeviceVolumeSensor.SetState(masterVolume);
                AddUpdateSensor(defaultDeviceVolumeId, defaultDeviceVolumeSensor);

                var defaultDeviceIsMuted = audioDevice.AudioEndpointVolume?.Mute ?? false;
                var defaultDeviceIsMutedId = $"{parentSensorSafeName}_default_device_muted";
                var defaultDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, $"{Name} Default Device Muted", defaultDeviceIsMutedId, string.Empty, "mdi:speaker", Name);
                defaultDeviceIsMutedSensor.SetState(defaultDeviceIsMuted);
                AddUpdateSensor(defaultDeviceIsMutedId, defaultDeviceIsMutedSensor);

                // get session and volume info
                var sessionInfos = GetSessions(out var peakVolume);

                var peakVolumeId = $"{parentSensorSafeName}_peak_volume";
                var peakVolumeSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Peak Volume", peakVolumeId, string.Empty, "mdi:volume-high", string.Empty, Name);
                peakVolumeSensor.SetState(peakVolume.ToString(CultureInfo.CurrentCulture));
                AddUpdateSensor(peakVolumeId, peakVolumeSensor);

                var sessions = JsonConvert.SerializeObject(new AudioSessionInfoCollection(sessionInfos), Formatting.Indented);
                var sessionsId = $"{parentSensorSafeName}_audio_sessions";
                var sessionsSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Audio Sessions", sessionsId, string.Empty, "mdi:music-box-multiple-outline", string.Empty, Name, true);
                sessionsSensor.SetState(sessionInfos.Count);
                sessionsSensor.SetAttributes(sessions);
                AddUpdateSensor(sessionsId, sessionsSensor);
            }

            var audioOutputDevices = GetAudioOutputDevices();
            var audioOutputDevicesId = $"{parentSensorSafeName}_audio_output_devices";
            var audioOutputDevicesSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Audio Output Devices", audioOutputDevicesId, string.Empty, "mdi:music-box-multiple-outline", string.Empty, Name, true);
            audioOutputDevicesSensor.SetState(audioOutputDevices.Count);
            audioOutputDevicesSensor.SetAttributes(
                JsonConvert.SerializeObject(new
                {
                    OutputDevices = audioOutputDevices
                }, Formatting.Indented)
            );
            AddUpdateSensor(audioOutputDevicesId, audioOutputDevicesSensor);
        }

        private void HandleAudioInputSensors(string parentSensorSafeName)
        {
            using var inputDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eCapture, Role.Communications);

            var defaultInputDeviceId = $"{parentSensorSafeName}_default_input_device";
            var defaultInputDeviceSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Input Device", defaultInputDeviceId, string.Empty, "mdi:microphone", string.Empty, Name);
            defaultInputDeviceSensor.SetState(inputDevice.DeviceFriendlyName);
            AddUpdateSensor(defaultInputDeviceId, defaultInputDeviceSensor);

            var defaultInputDeviceStateId = $"{parentSensorSafeName}_default_input_device_state";
            var defaultInputDeviceStateSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Input Device State", defaultInputDeviceStateId, string.Empty, "mdi:microphone", string.Empty, Name);
            defaultInputDeviceStateSensor.SetState(GetReadableState(inputDevice.State));
            AddUpdateSensor(defaultInputDeviceStateId, defaultInputDeviceStateSensor);

            var defaultInputDeviceIsMuted = inputDevice.AudioEndpointVolume?.Mute ?? false;
            var defaultInputDeviceIsMutedId = $"{parentSensorSafeName}_default_input_device_muted";
            var defaultInputDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, $"{Name} Default Input Device Muted", defaultInputDeviceIsMutedId, string.Empty, "mdi:microphone", Name);
            defaultInputDeviceIsMutedSensor.SetState(defaultInputDeviceIsMuted);
            AddUpdateSensor(defaultInputDeviceIsMutedId, defaultInputDeviceIsMutedSensor);

            var inputVolume = (int)GetDefaultInputDevicePeakVolume(inputDevice);
            var defaultInputDeviceVolumeId = $"{parentSensorSafeName}_default_input_device_volume";
            var defaultInputDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Default Input Device Volume", defaultInputDeviceVolumeId, string.Empty, "mdi:microphone", string.Empty, Name);
            defaultInputDeviceVolumeSensor.SetState(inputVolume);
            AddUpdateSensor(defaultInputDeviceVolumeId, defaultInputDeviceVolumeSensor);

            var audioInputDevices = GetAudioInputDevices();
            var audioInputDevicesId = $"{parentSensorSafeName}_audio_output_devices";
            var audioInputDevicesSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Audio Input Devices", audioInputDevicesId, string.Empty, "mdi:microphone", string.Empty, Name, true);
            audioInputDevicesSensor.SetState(audioInputDevices.Count);
            audioInputDevicesSensor.SetAttributes(
                JsonConvert.SerializeObject(new
                {
                    InputDevices = audioInputDevices
                }, Formatting.Indented)
            );
            AddUpdateSensor(audioInputDevicesId, audioInputDevicesSensor);
        }

        public sealed override void UpdateSensorValues()
        {
            try
            {
                var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

                HandleAudioOutputSensors(parentSensorSafeName);
                HandleAudioInputSensors(parentSensorSafeName);

                if (_errorPrinted)
                    _errorPrinted = false;
            }
            catch (Exception ex)
            {
                if (_errorPrinted)
                    return;

                _errorPrinted = true;

                Log.Fatal(ex, "[AUDIO] [{name}] Error while fetching audio info: {err}", Name, ex.Message);
            }
        }

        private string GetSessionDisplayName(AudioSessionControl2 session)
        {
            var procId = (int)session.ProcessID;

            if (procId <= 0)
                return session.DisplayName;

            if (ApplicationNames.ContainsKey(procId))
                return ApplicationNames[procId];

            // we don't know this app yet, get process info
            using var p = Process.GetProcessById(procId);
            ApplicationNames.Add(procId, p.ProcessName);

            return p.ProcessName;
        }

        private List<AudioSessionInfo> GetSessions(out float peakVolume)
        {
            var sessionInfos = new List<AudioSessionInfo>();
            peakVolume = 0f;

            try
            {
                var errors = false;

                foreach (var device in Variables.AudioDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.eRender, DeviceState.Active))
                {
                    using (device)
                    {
                        foreach (var session in device.AudioSessionManager2?.Sessions.Where(x => x != null))
                        {
                            if (session.ProcessID == 0)
                                continue;

                            try
                            {
                                var displayName = GetSessionDisplayName(session);

                                if (displayName.Length > 30)
                                    displayName = $"{displayName[..30]}..";

                                Debug.WriteLine($" {displayName} /// {session.ProcessID}"); //TODO: remove

                                var sessionInfo = new AudioSessionInfo
                                {
                                    Application = displayName,
                                    PlaybackDevice = device.DeviceFriendlyName,
                                    Muted = session.SimpleAudioVolume?.Mute ?? false,
                                    Active = session.State == AudioSessionState.AudioSessionStateActive,
                                    MasterVolume = session.SimpleAudioVolume?.MasterVolume * 100 ?? 0f,
                                    PeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f
                                };

                                // new max?
                                if (sessionInfo.PeakVolume > peakVolume)
                                    peakVolume = sessionInfo.PeakVolume;

                                sessionInfos.Add(sessionInfo);
                            }
                            catch (Exception ex)
                            {
                                if (!_errorPrinted)
                                    Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving info: {err}", Name, session.DisplayName, ex.Message);

                                errors = true;
                            }
                            finally
                            {
                                session?.Dispose();
                            }
                        }
                    }
                }

                // only print errors once
                if (errors && !_errorPrinted)
                {
                    _errorPrinted = true;

                    return sessionInfos;
                }

                // optionally reset error flag
                if (_errorPrinted)
                    _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted)
                    return sessionInfos;

                _errorPrinted = true;

                Log.Fatal(ex, "[AUDIO] [{name}] Fatal exception while getting sessions: {err}", Name, ex.Message);
            }

            return sessionInfos;
        }

        private float GetDefaultInputDevicePeakVolume(MMDevice inputDevice)
        {
            if (inputDevice == null)
                return 0f;

            var peakVolume = 0f;

            try
            {
                var errors = false;

                // process sessions (and get peak volume)
                foreach (var session in inputDevice.AudioSessionManager2?.Sessions?.Where(x => x != null)!)
                {
                    try
                    {
                        // filter inactive sessions
                        if (session.State != AudioSessionState.AudioSessionStateActive)
                            continue;

                        // set peak volume
                        var sessionPeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f;

                        // new max?
                        if (sessionPeakVolume > peakVolume)
                            peakVolume = sessionPeakVolume;
                    }
                    catch (Exception ex)
                    {
                        if (!_errorPrinted)
                            Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving input info: {err}", Name, session.DisplayName, ex.Message);

                        errors = true;
                    }
                    finally
                    {
                        session?.Dispose();
                    }
                }

                // only print errors once
                if (errors && !_errorPrinted)
                {
                    _errorPrinted = true;

                    return peakVolume;
                }

                // optionally reset error flag
                if (_errorPrinted)
                    _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted)
                    return peakVolume;

                _errorPrinted = true;

                Log.Fatal(ex, "[AUDIO] [{name}] Fatal exception while getting input info: {err}", Name, ex.Message);
            }

            return peakVolume;
        }

        /// <summary>
        /// Converts the audio device's state to a better readable form
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static string GetReadableState(DeviceState state)
        {
            return state switch
            {
                DeviceState.Active => "ACTIVE",
                DeviceState.Disabled => "DISABLED",
                DeviceState.NotPresent => "NOT PRESENT",
                DeviceState.Unplugged => "UNPLUGGED",
                DeviceState.MaskAll => "STATEMASK_ALL",
                _ => "UNKNOWN"
            };
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
