using System.Management;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors
{
    /// <summary>
    /// Sensor containing the result of the provided WMI query
    /// </summary>
    public class WmiQuerySensor : AbstractSingleValueSensor
    {
        public string Query { get; private set; }
        public string Scope { get; private set; }

        protected readonly ObjectQuery ObjectQuery;
        protected readonly ManagementObjectSearcher Searcher;

        public WmiQuerySensor(string query, string scope = "", int? updateInterval = null, string name = "wmiquerysensor", string id = default) : base(name ?? "wmiquerysensor", updateInterval ?? 10, id)
        {
            Query = query;
            Scope = scope;

            // prepare query
            ObjectQuery = new ObjectQuery(Query);

            // use either default or provided scope
            var managementscope = !string.IsNullOrWhiteSpace(scope) 
                ? new ManagementScope(scope) 
                : new ManagementScope(@"\\localhost\");

            // prepare searcher
            Searcher = new ManagementObjectSearcher(managementscope, ObjectQuery);
        }
        
        public void Dispose() => Searcher?.Dispose();

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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{Name}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }
        
        public override string GetState()
        {
            using var collection = Searcher.Get();
            var retValue = string.Empty;
            foreach (var managementBaseObject in collection)
            {
                try
                {
                    if (!string.IsNullOrEmpty(retValue)) continue;

                    using var managementObject = (ManagementObject)managementBaseObject;
                    foreach (var property in managementObject.Properties)
                    {
                        retValue = property.Value.ToString();
                        break;
                    }
                }
                finally
                {
                    managementBaseObject?.Dispose();
                }
            }
            return retValue;
        }
    }
}
