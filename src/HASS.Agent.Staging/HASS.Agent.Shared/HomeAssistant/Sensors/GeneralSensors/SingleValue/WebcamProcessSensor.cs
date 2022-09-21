using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating whether the mic is in use
    /// </summary>
    public class WebcamProcessSensor : AbstractSingleValueSensor
    {
        public WebcamProcessSensor(int? updateInterval = null, string name = "webcamprocess", string id = default) : base(name ?? "webcamprocess", updateInterval ?? 10, id)
        {
            //
        }

        public override string GetState() => WebcamProcess();

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
                Icon = "mdi:webcam"
            });
        }

        public override string GetAttributes() => string.Empty;

        private static string WebcamProcess()
        {
            const string regKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";
            (bool, string) inUse;

            // first local machine
            using (var key = Registry.LocalMachine.OpenSubKey(regKey))
            {
                inUse = CheckRegForWebcamInUse(key);
                if (inUse.Item1) return inUse.Item2;
            }

            // then current user
            using (var key = Registry.CurrentUser.OpenSubKey(regKey))
            {
                inUse = CheckRegForWebcamInUse(key);
                if (inUse.Item1) return inUse.Item2;
            }

            // nope
            return string.Empty;
        }

        private static (bool active, string application) CheckRegForWebcamInUse(RegistryKey key)
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
