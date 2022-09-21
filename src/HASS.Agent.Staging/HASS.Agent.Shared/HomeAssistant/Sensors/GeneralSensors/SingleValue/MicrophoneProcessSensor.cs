using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating whether the mic is in use
    /// </summary>
    public class MicrophoneProcessSensor : AbstractSingleValueSensor
    {
        public MicrophoneProcessSensor(int? updateInterval = null, string name = "microphoneprocess", string id = default) : base(name ?? "microphoneprocess", updateInterval ?? 10, id)
        {
            //
        }

        public override string GetState() => MicrophoneProcess();

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Icon = "mdi:microphone"
            });
        }

        public override string GetAttributes() => string.Empty;

        private static string MicrophoneProcess()
        {
            const string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone";
            (bool, string) inUse;

            // first local machine
            using (var key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                inUse = CheckRegForMicrophoneInUse(key);
                if (inUse.Item1) return inUse.Item2;
            }

            // then current user
            using (var key = Registry.CurrentUser.OpenSubKey(regKey))
            {
                inUse = CheckRegForMicrophoneInUse(key);
                if (inUse.Item1) return inUse.Item2;
            }

            // nope
            return string.Empty;
        }

        private static (bool active, string application) CheckRegForMicrophoneInUse(RegistryKey key)
        {
            if (key == null) return (false, string.Empty);

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                // NonPackaged has multiple subkeys
                if (subKeyName == "NonPackaged")
                {
                    using var nonpackagedkey = key.OpenSubKey(subKeyName);
                    if (nonpackagedkey == null) continue;

                    foreach (var nonpackagedSubKeyName in nonpackagedkey.GetSubKeyNames())
                    {
                        using var subKey = nonpackagedkey.OpenSubKey(nonpackagedSubKeyName);
                        if (subKey == null || !subKey.GetValueNames().Contains("LastUsedTimeStop")) continue;

                        var endTime = subKey.GetValue("LastUsedTimeStop") is long
                            ? (long)(subKey.GetValue("LastUsedTimeStop") ?? -1)
                            : -1;

                        if (endTime <= 0) return (true, SharedHelperFunctions.ParseRegWebcamMicApplicationName(subKey.Name));
                    }
                }
                else
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey == null || !subKey.GetValueNames().Contains("LastUsedTimeStop")) continue;

                    var endTime = subKey.GetValue("LastUsedTimeStop") is long ? (long)(subKey.GetValue("LastUsedTimeStop") ?? -1) : -1;
                    if (endTime <= 0) return (true, SharedHelperFunctions.ParseRegWebcamMicApplicationName(subKey.Name));
                }
            }

            return (false, string.Empty);
        }
    }
}
