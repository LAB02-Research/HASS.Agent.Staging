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
        private static readonly Dictionary<int, string> ApplicationNames = new Dictionary<int, string>();
        private bool _errorPrinted = false;

        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public AudioSensors(int? updateInterval = null, string name = "audio", string id = default) : base(name ?? "audio", updateInterval ?? 20, id)
        {
            _updateInterval = updateInterval ?? 20;

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            try
            {
                // lowercase and safe name of the multivalue sensor
                var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

                // get the default audio device
                using (var audioDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eRender, Role.Multimedia))
                {
                    // default device name
                    var defaultDeviceId = $"{parentSensorSafeName}_default_device";
                    var defaultDeviceSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Device", defaultDeviceId, string.Empty, "mdi:speaker", string.Empty, Name);
                    defaultDeviceSensor.SetState(audioDevice.DeviceFriendlyName);

                    if (!Sensors.ContainsKey(defaultDeviceId)) Sensors.Add(defaultDeviceId, defaultDeviceSensor);
                    else Sensors[defaultDeviceId] = defaultDeviceSensor;

                    // default device state
                    var defaultDeviceStateId = $"{parentSensorSafeName}_default_device_state";
                    var defaultDeviceStateSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Device State", defaultDeviceStateId, string.Empty, "mdi:speaker", string.Empty, Name);
                    defaultDeviceStateSensor.SetState(GetReadableState(audioDevice.State));

                    if (!Sensors.ContainsKey(defaultDeviceStateId)) Sensors.Add(defaultDeviceStateId, defaultDeviceStateSensor);
                    else Sensors[defaultDeviceStateId] = defaultDeviceStateSensor;

                    // default device volume
                    var masterVolume = (int)(audioDevice.AudioEndpointVolume?.MasterVolumeLevelScalar * 100 ?? 0);
                    var defaultDeviceVolumeId = $"{parentSensorSafeName}_default_device_volume";
                    var defaultDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Default Device Volume", defaultDeviceVolumeId, string.Empty, "mdi:speaker", string.Empty, Name);
                    defaultDeviceVolumeSensor.SetState(masterVolume);

                    if (!Sensors.ContainsKey(defaultDeviceVolumeId)) Sensors.Add(defaultDeviceVolumeId, defaultDeviceVolumeSensor);
                    else Sensors[defaultDeviceVolumeId] = defaultDeviceVolumeSensor;

                    // default device muted
                    var defaultDeviceIsMuted = audioDevice.AudioEndpointVolume?.Mute ?? false;
                    var defaultDeviceIsMutedId = $"{parentSensorSafeName}_default_device_muted";
                    var defaultDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, $"{Name} Default Device Muted", defaultDeviceIsMutedId, string.Empty, "mdi:speaker", Name);
                    defaultDeviceIsMutedSensor.SetState(defaultDeviceIsMuted);

                    if (!Sensors.ContainsKey(defaultDeviceIsMutedId)) Sensors.Add(defaultDeviceIsMutedId, defaultDeviceIsMutedSensor);
                    else Sensors[defaultDeviceIsMutedId] = defaultDeviceIsMutedSensor;

                    // get session and volume info
                    var sessionInfos = GetSessions(out var peakVolume);
                    
                    // peak value sensor
                    var peakVolumeId = $"{parentSensorSafeName}_peak_volume";
                    var peakVolumeSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Peak Volume", peakVolumeId, string.Empty, "mdi:volume-high", string.Empty, Name);
                    peakVolumeSensor.SetState(peakVolume.ToString(CultureInfo.CurrentCulture));

                    if (!Sensors.ContainsKey(peakVolumeId)) Sensors.Add(peakVolumeId, peakVolumeSensor);
                    else Sensors[peakVolumeId] = peakVolumeSensor;

                    // sessions sensor
                    var sessions = JsonConvert.SerializeObject(new AudioSessionInfoCollection(sessionInfos), Formatting.Indented);
                    var sessionsId = $"{parentSensorSafeName}_audio_sessions";
                    var sessionsSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Audio Sessions", sessionsId, string.Empty, "mdi:music-box-multiple-outline", string.Empty, Name, true);
                    
                    sessionsSensor.SetState(sessionInfos.Count);
                    sessionsSensor.SetAttributes(sessions);

                    if (!Sensors.ContainsKey(sessionsId)) Sensors.Add(sessionsId, sessionsSensor);
                    else Sensors[sessionsId] = sessionsSensor;
                }

                // get the default input audio device
                using (var inputDevice = Variables.AudioDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.eCapture, Role.Communications))
                {
                    // default input device name
                    var defaultInputDeviceId = $"{parentSensorSafeName}_default_input_device";
                    var defaultInputDeviceSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Input Device", defaultInputDeviceId, string.Empty, "mdi:microphone", string.Empty, Name);
                    defaultInputDeviceSensor.SetState(inputDevice.DeviceFriendlyName);

                    if (!Sensors.ContainsKey(defaultInputDeviceId)) Sensors.Add(defaultInputDeviceId, defaultInputDeviceSensor);
                    else Sensors[defaultInputDeviceId] = defaultInputDeviceSensor;

                    // default input device state
                    var defaultInputDeviceStateId = $"{parentSensorSafeName}_default_input_device_state";
                    var defaultInputDeviceStateSensor = new DataTypeStringSensor(_updateInterval, $"{Name} Default Input Device State", defaultInputDeviceStateId, string.Empty, "mdi:microphone", string.Empty, Name);
                    defaultInputDeviceStateSensor.SetState(GetReadableState(inputDevice.State));

                    if (!Sensors.ContainsKey(defaultInputDeviceStateId)) Sensors.Add(defaultInputDeviceStateId, defaultInputDeviceStateSensor);
                    else Sensors[defaultInputDeviceStateId] = defaultInputDeviceStateSensor;

                    // default input device muted
                    var defaultInputDeviceIsMuted = inputDevice.AudioEndpointVolume?.Mute ?? false;
                    var defaultInputDeviceIsMutedId = $"{parentSensorSafeName}_default_input_device_muted";
                    var defaultInputDeviceIsMutedSensor = new DataTypeBoolSensor(_updateInterval, $"{Name} Default Input Device Muted", defaultInputDeviceIsMutedId, string.Empty, "mdi:microphone", Name);
                    defaultInputDeviceIsMutedSensor.SetState(defaultInputDeviceIsMuted);

                    if (!Sensors.ContainsKey(defaultInputDeviceIsMutedId)) Sensors.Add(defaultInputDeviceIsMutedId, defaultInputDeviceIsMutedSensor);
                    else Sensors[defaultInputDeviceIsMutedId] = defaultInputDeviceIsMutedSensor;

                    // default input device volume
                    var inputVolume = (int)GetDefaultInputDevicePeakVolume(inputDevice);
                    var defaultInputDeviceVolumeId = $"{parentSensorSafeName}_default_input_device_volume";
                    var defaultInputDeviceVolumeSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Default Input Device Volume", defaultInputDeviceVolumeId, string.Empty, "mdi:microphone", string.Empty, Name);
                    defaultInputDeviceVolumeSensor.SetState(inputVolume);

                    if (!Sensors.ContainsKey(defaultInputDeviceVolumeId)) Sensors.Add(defaultInputDeviceVolumeId, defaultInputDeviceVolumeSensor);
                    else Sensors[defaultInputDeviceVolumeId] = defaultInputDeviceVolumeSensor;
                }

                // optionally reset error flag
                if (_errorPrinted) _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted) return;
                _errorPrinted = true;

                Log.Fatal(ex, "[AUDIO] [{name}] Error while fetching audio info: {err}", Name, ex.Message);
            }
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
                    // process sessions (and get peak volume)
                    foreach (var session in device.AudioSessionManager2?.Sessions.Where(x => x != null))
                    {
                        try
                        {
                            // filter inactive sessions
                            if (session.State != AudioSessionState.AudioSessionStateActive) continue;

                            // prepare sessioninfo
                            var sessionInfo = new AudioSessionInfo();

                            // get displayname
                            string displayName;
                            var procId = (int)session.ProcessID;
                            if (procId <= 0)
                            {
                                // faulty process id, use the provided displayname
                                displayName = session.DisplayName;
                            }
                            else
                            {
                                if (ApplicationNames.ContainsKey(procId)) displayName = ApplicationNames[procId];
                                else
                                {
                                    // we don't know this app yet, get process info
                                    using var p = Process.GetProcessById(procId);
                                    displayName = p.ProcessName;
                                    ApplicationNames.Add(procId, displayName);
                                }
                            }

                            // set displayname
                            if (displayName.Length > 30) displayName = $"{displayName[..30]}..";
                            sessionInfo.Application = displayName;

                            // get muted state
                            sessionInfo.Muted = session.SimpleAudioVolume?.Mute ?? false;

                            // set master volume
                            sessionInfo.MasterVolume = session.SimpleAudioVolume?.MasterVolume * 100 ?? 0f;

                            // set peak volume
                            sessionInfo.PeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f;

                            // new max?
                            if (sessionInfo.PeakVolume > peakVolume) peakVolume = sessionInfo.PeakVolume;

                            // store the session info
                            sessionInfos.Add(sessionInfo);
                        }
                        catch (Exception ex)
                        {
                            if (!_errorPrinted) Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving info: {err}", Name, session.DisplayName, ex.Message);
                            errors = true;
                        }
                        finally
                        {
                            session?.Dispose();
                            device?.Dispose();
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
                if (_errorPrinted) _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted) return sessionInfos;
                _errorPrinted = true;

                Log.Fatal(ex, "[AUDIO] [{name}] Fatal exception while getting sessions: {err}", Name, ex.Message);
            }

            return sessionInfos;
        }

        private float GetDefaultInputDevicePeakVolume(MMDevice inputDevice)
        {
            if (inputDevice == null) return 0f;
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
                        if (session.State != AudioSessionState.AudioSessionStateActive) continue;
                        
                        // set peak volume
                        var sessionPeakVolume = session.AudioMeterInformation?.MasterPeakValue * 100 ?? 0f;

                        // new max?
                        if (sessionPeakVolume > peakVolume) peakVolume = sessionPeakVolume;
                    }
                    catch (Exception ex)
                    {
                        if (!_errorPrinted) Log.Fatal(ex, "[AUDIO] [{name}] [{app}] Exception while retrieving input info: {err}", Name, session.DisplayName, ex.Message);
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
                if (_errorPrinted) _errorPrinted = false;
            }
            catch (Exception ex)
            {
                // something went wrong, only print once
                if (_errorPrinted) return peakVolume;
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
