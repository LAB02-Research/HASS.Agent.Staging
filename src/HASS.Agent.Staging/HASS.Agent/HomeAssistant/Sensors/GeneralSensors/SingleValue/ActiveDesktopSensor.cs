using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Shared.Models.HomeAssistant;
using Microsoft.Win32;
using Newtonsoft.Json;
using Windows.Foundation.Metadata;
using WindowsDesktop;
using static HASS.Agent.Functions.NativeMethods;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the ID of the currently active virtual desktop
    /// Additionally returns all available virtual desktops and their names (if named)
    /// </summary>
    public class ActiveDesktopSensor : AbstractSingleValueSensor
    {
        private const string _defaultName = "activedesktop";
 
        private string _desktopId = string.Empty;
        private string _attributes = string.Empty;

        public ActiveDesktopSensor(int? updateInterval = null, string name = _defaultName, string friendlyName = _defaultName, string id = default) : base(name ?? _defaultName, friendlyName ?? null, updateInterval ?? 15, id)
        {
            UseAttributes = true;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel()
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:monitor",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability",
                Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes"
            });
        }

        public override string GetState()
        {
            var currentDesktop = VirtualDesktop.Current;
            _desktopId = currentDesktop.Id.ToString();

            var desktops = new Dictionary<string, string>();
            foreach (var desktop in VirtualDesktop.GetDesktops())
            {
                var id = desktop.Id.ToString();
                desktops[id] = string.IsNullOrWhiteSpace(desktop.Name) ? GetDesktopNameFromRegistry(id) : desktop.Name;
            }

            _attributes = JsonConvert.SerializeObject(new
            {
                desktopName = currentDesktop.Name,
                availableDesktops = desktops
            }, Formatting.Indented);

            return _desktopId;
        }

        public override string GetAttributes() => _attributes;

        private string GetDesktopNameFromRegistry(string id)
        {
            var registryPath = $"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VirtualDesktops\\Desktops\\{{{id}}}";
            return (Registry.GetValue(registryPath, "Name", string.Empty) as string) ?? string.Empty;
        }
    }
}
