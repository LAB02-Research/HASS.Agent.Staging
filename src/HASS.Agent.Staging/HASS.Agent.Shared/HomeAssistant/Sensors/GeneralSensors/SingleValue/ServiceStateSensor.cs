using System;
using System.ServiceProcess;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the current state of the provided service
    /// </summary>
    public class ServiceStateSensor : AbstractSingleValueSensor
    {
        public string ServiceName { get; protected set; }

        public ServiceStateSensor(string serviceName, int? updateInterval = null, string name = "servicestate", string id = default) : base(name ?? "servicestate", updateInterval ?? 10, id)
        {
            ServiceName = serviceName;
        }

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
                Icon = "mdi:file-eye-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            try
            {
                using var svc = new ServiceController(ServiceName);
                return svc.Status.ToString();
            }
            catch (InvalidOperationException)
            {
                // service wasn't found
                return "NotFound";
            }
        }
    }
}
