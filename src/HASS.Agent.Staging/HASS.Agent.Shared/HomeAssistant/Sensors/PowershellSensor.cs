﻿using System;
using System.Globalization;
using System.Management;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors
{
    /// <summary>
    /// Sensor containing the result of the provided Powershell command or script
    /// </summary>
    public class PowershellSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "powershellsensor";

        public string Command { get; private set; }
        public bool ApplyRounding { get; private set; }
        public int? Round { get; private set; }

        public PowershellSensor(string command, bool applyRounding = false, int? round = null, int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 10, id)
        {
            Command = command;
            ApplyRounding = applyRounding;
            Round = round;
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
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{Name}/state",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }
        
        public override string GetState()
        {
            var executed = PowershellManager.ExecuteWithOutput(Command, TimeSpan.FromMinutes(5), out var output, out var errors);
            if (!executed) return "error_during_execution";

            if (string.IsNullOrWhiteSpace(output) && string.IsNullOrWhiteSpace(errors)) return string.Empty;
            if (string.IsNullOrWhiteSpace(output)) return errors.Trim();

            // optionally apply rounding
            if (ApplyRounding && Round != null && double.TryParse(output, out var dblValue)) { output = Math.Round(dblValue, (int)Round).ToString(CultureInfo.CurrentCulture); }

            // done
            return string.IsNullOrWhiteSpace(errors) ? output : $"{output} | {errors}";
        }

        public override string GetAttributes() => string.Empty;
    }
}