﻿using System;
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
        public string PreviousPublishedAttributes { get; protected set; } = string.Empty;

        protected AbstractSingleValueSensor(string name, string friendlyName, int updateIntervalSeconds = 10, string id = default, bool useAttributes = false)
        {
            Id = id == null || id == Guid.Empty.ToString() ? Guid.NewGuid().ToString() : id;
            Name = name;
            FriendlyName = friendlyName;
            UpdateIntervalSeconds = updateIntervalSeconds;
            Domain = "sensor";
            UseAttributes = useAttributes;
        }

        protected SensorDiscoveryConfigModel AutoDiscoveryConfigModel;
        protected SensorDiscoveryConfigModel SetAutoDiscoveryConfigModel(SensorDiscoveryConfigModel config)
        {
            AutoDiscoveryConfigModel = config;
            return config;
        }

        public override void ClearAutoDiscoveryConfig() => AutoDiscoveryConfigModel = null;

        public abstract string GetState();
        public abstract string GetAttributes();

        public void ResetChecks()
        {
            LastUpdated = DateTime.MinValue;

            PreviousPublishedState = string.Empty;
            PreviousPublishedAttributes = string.Empty;
        }

        public async Task PublishStateAsync(bool respectChecks = true)
        {
            try
            {
                if (Variables.MqttManager == null) return;

                // are we asked to check elapsed time?
                if (respectChecks)
                {
                    if (LastUpdated.HasValue && LastUpdated.Value.AddSeconds(UpdateIntervalSeconds) > DateTime.Now) return;
                }

                // get the current state/attributes
                var state = GetState();
                var attributes = GetAttributes();

                // are we asked to check state changes?
                if (respectChecks)
                {
                    if (PreviousPublishedState == state && PreviousPublishedAttributes == attributes) return;
                }

                // fetch the autodiscovery config
                var autoDiscoConfig = (SensorDiscoveryConfigModel)GetAutoDiscoveryConfig();
                if (autoDiscoConfig == null) return;

                // prepare the state message
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(autoDiscoConfig.State_topic)
                    .WithPayload(state)
                    .WithRetainFlag(Variables.MqttManager.UseRetainFlag())
                    .Build();

                // send it
                var published = await Variables.MqttManager.PublishAsync(message);
                if (!published)
                {
                    // failed, don't store the state
                    return;
                }

                // optionally prepare and send attributes
                if (UseAttributes)
                {
                    message = new MqttApplicationMessageBuilder()
                        .WithTopic(autoDiscoConfig.Json_attributes_topic)
                        .WithPayload(attributes)
                        .WithRetainFlag(Variables.MqttManager.UseRetainFlag())
                        .Build();

                    published = await Variables.MqttManager.PublishAsync(message);
                    if (!published)
                    {
                        // failed, don't store the state
                        return;
                    }
                }

                // only store the values if the checks are respected
                // otherwise, we might stay in 'unknown' state untill the value changes
                if (!respectChecks) return;

                PreviousPublishedState = state;
                PreviousPublishedAttributes = attributes;
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
