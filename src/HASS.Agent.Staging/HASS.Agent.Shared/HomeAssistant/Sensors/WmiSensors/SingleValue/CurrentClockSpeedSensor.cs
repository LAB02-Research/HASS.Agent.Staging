﻿using System;
using System.Management;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.WmiSensors.SingleValue
{
    /// <summary>
    /// Sensor indicating the CPU's (advertised) clockspeed
    /// </summary>
    public class CurrentClockSpeedSensor : WmiQuerySensor
    {
        private const string DefaultName = "currentclockspeed";

        private readonly ManagementObject _managementObject;

        private protected DateTime LastFetched = DateTime.MinValue;
        private protected string LastValue = string.Empty;

        public CurrentClockSpeedSensor(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default, bool applyRounding = false, int? round = null) : base(string.Empty, string.Empty, applyRounding, round, updateInterval ?? 300, name ?? DefaultName, friendlyName ?? null, id) 
            => _managementObject = new ManagementObject("Win32_Processor.DeviceID='CPU0'");

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
                Icon = "mdi:speedometer",
                Unit_of_measurement = "MHz",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            try
            {
                // we're caching this, too heavy
                if ((DateTime.Now - LastFetched).TotalHours < 1 && !string.IsNullOrEmpty(LastValue)) return LastValue;
                LastFetched = DateTime.Now;

                var speed = (uint)(_managementObject["CurrentClockSpeed"]);
                LastValue = speed.ToString();

                return LastValue;
            }
            catch (Exception ex)
            {
                Log.Error("[CURRENTCLOCKSPEED] [{name}] Error getting current clockspeed: {msg}", Name, ex.Message);
                return "0";
            }
        }
    }
}
