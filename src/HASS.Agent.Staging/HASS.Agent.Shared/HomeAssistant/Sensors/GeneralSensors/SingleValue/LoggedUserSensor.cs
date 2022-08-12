using System;
using System.Linq;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    public class LoggedUserSensor : AbstractSingleValueSensor
    {
        public LoggedUserSensor(int? updateInterval = null, string name = "loggeduser", string id = default) : base(name ?? "loggeduser", updateInterval ?? 10, id) { }

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
                Icon = "mdi:account-group",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            // get the active users
            var loggedUsers = SessionsManager.GetLoggedUsers(true);
            var loggedUsersList = loggedUsers as string[] ?? loggedUsers.ToArray();

            // if there is none, our username, otherwise the first
            return loggedUsersList.Any() ? Environment.UserName : loggedUsersList.First();
        }

        public override string GetAttributes() => string.Empty;
    }
}
