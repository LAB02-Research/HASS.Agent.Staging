﻿using System.Collections.Generic;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing Windows Update info
    /// </summary>
    public class WindowsUpdatesSensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "windowsupdates";
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public WindowsUpdatesSensors(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 900, id)
        {
            _updateInterval = updateInterval ?? 900;

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            // lowercase and safe name of the multivalue sensor
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            // fetch the latest updates
            var (driverUpdates, softwareUpdates) = WindowsUpdatesManager.GetAvailableUpdates();

            // driver update count
            var driverUpdateCount = driverUpdates.Count;

            var driverUpdateCountId = $"{parentSensorSafeName}_driver_updates_pending";
            var driverUpdateCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Driver Updates Pending", driverUpdateCountId, string.Empty, "mdi:microsoft-windows", string.Empty, Name);
            driverUpdateCountSensor.SetState(driverUpdateCount);

            if (!Sensors.ContainsKey(driverUpdateCountId)) Sensors.Add(driverUpdateCountId, driverUpdateCountSensor);
            else Sensors[driverUpdateCountId] = driverUpdateCountSensor;

            // software update count
            var softwareUpdateCount = softwareUpdates.Count;

            var softwareUpdateCountId = $"{parentSensorSafeName}_software_updates_pending";
            var softwareUpdateCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Software Updates Pending", softwareUpdateCountId, string.Empty, "mdi:microsoft-windows", string.Empty, Name);
            softwareUpdateCountSensor.SetState(softwareUpdateCount);

            if (!Sensors.ContainsKey(softwareUpdateCountId)) Sensors.Add(softwareUpdateCountId, softwareUpdateCountSensor);
            else Sensors[softwareUpdateCountId] = softwareUpdateCountSensor;

            // driver updates array
            var driverUpdatesList = new WindowsUpdateInfoCollection(driverUpdates);
            var driverUpdatesStr = JsonConvert.SerializeObject(driverUpdatesList, Formatting.Indented);

            var driverUpdatesId = $"{parentSensorSafeName}_driver_updates";

            var driverUpdatesSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Available Driver Updates", driverUpdatesId, string.Empty, "mdi:microsoft-windows", string.Empty, Name, true);
            driverUpdatesSensor.SetState(driverUpdates.Count);
            driverUpdatesSensor.SetAttributes(driverUpdatesStr);

            if (!Sensors.ContainsKey(driverUpdatesId)) Sensors.Add(driverUpdatesId, driverUpdatesSensor);
            else Sensors[driverUpdatesId] = driverUpdatesSensor;

            // software updates array
            var softwareUpdatesStr = JsonConvert.SerializeObject(new WindowsUpdateInfoCollection(softwareUpdates), Formatting.Indented);
            var softwareUpdatesId = $"{parentSensorSafeName}_software_updates";
            var softwareUpdatesSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Available Software Updates", softwareUpdatesId, string.Empty, "mdi:microsoft-windows", string.Empty, Name, true);
            
            softwareUpdatesSensor.SetState(softwareUpdates.Count);
            softwareUpdatesSensor.SetAttributes(softwareUpdatesStr);

            if (!Sensors.ContainsKey(softwareUpdatesId)) Sensors.Add(softwareUpdatesId, softwareUpdatesSensor);
            else Sensors[softwareUpdatesId] = softwareUpdatesSensor;

            // all done!
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
