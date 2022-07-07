using System;
using System.Threading.Tasks;
using MQTTnet;
using Serilog;

namespace HASS.Agent.Shared.Models.HomeAssistant
{
    /// <summary>
    /// Abstract singlevalue-sensor from which all singlevalue-sensors are derived
    /// </summary>
    public abstract class AbstractSingleValueSensor : AbstractDiscoverable
    {
        public int UpdateIntervalSeconds { get; protected set; }
        public DateTime? LastUpdated { get; protected set; }
        public string PreviousPublishedState { get; protected set; } = string.Empty;

        protected AbstractSingleValueSensor(string name, int updateIntervalSeconds = 10, string id = default)
        {
            Id = id == null || id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : id;
            Name = name;
            UpdateIntervalSeconds = updateIntervalSeconds;
            Domain = "sensor";
        }

        protected SensorDiscoveryConfigModel AutoDiscoveryConfigModel;
        protected SensorDiscoveryConfigModel SetAutoDiscoveryConfigModel(SensorDiscoveryConfigModel config)
        {
            AutoDiscoveryConfigModel = config;
            return config;
        }

        public override void ClearAutoDiscoveryConfig() => AutoDiscoveryConfigModel = null;

        public abstract string GetState();

        public void ResetChecks()
        {
            LastUpdated = DateTime.MinValue;
            PreviousPublishedState = string.Empty;
        }

        public async Task PublishStateAsync(bool respectChecks = true)
        {
            try
            {
                if (Variables.MqttManager == null) return;

                if (respectChecks)
                {
                    if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateIntervalSeconds) > DateTime.Now) return;
                }
            
                var state = GetState();

                if (respectChecks)
                {
                    if (PreviousPublishedState == state) return;
                }

                var autoDiscoConfig = GetAutoDiscoveryConfig();
                if (autoDiscoConfig == null) return;

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(autoDiscoConfig.State_topic)
                    .WithPayload(state)
                    .Build();

                await Variables.MqttManager.PublishAsync(message);

                // only store the state if the checks are respected
                // otherwise, we might stay in 'unknown' state untill the value changes
                if (!respectChecks) return;

                PreviousPublishedState = state;
                LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                Log.Fatal("[SENSOR] [{name}] Error publishing state: {err}", Name, ex.Message);
            }
        }

        public async Task PublishAutoDiscoveryConfigAsync()
        {
            if (Variables.MqttManager == null) return;
            await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain);
        }

        public async Task UnPublishAutoDiscoveryConfigAsync()
        {
            if (Variables.MqttManager == null) return;
            await Variables.MqttManager.AnnounceAutoDiscoveryConfigAsync(this, Domain, true);
        }
    }
}
