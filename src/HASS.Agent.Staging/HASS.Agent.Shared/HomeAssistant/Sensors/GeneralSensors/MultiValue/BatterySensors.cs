using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing power and battery info
    /// </summary>
    public class BatterySensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "battery";
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public BatterySensors(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            _updateInterval = updateInterval ?? 30;

            UpdateSensorValues();
        }

        private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
        {
            if (!Sensors.ContainsKey(sensorId))
                Sensors.Add(sensorId, sensor);
            else
                Sensors[sensorId] = sensor;
        }

        public sealed override void UpdateSensorValues()
        {
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            var powerStatus = SystemInformation.PowerStatus;
            var chargeStatus = powerStatus.BatteryChargeStatus.ToString();

            var chargeStatusId = $"{parentSensorSafeName}_charge_status";
            var chargeStatusSensor = new DataTypeStringSensor(_updateInterval, "Charge Status", chargeStatusId, string.Empty, "mdi:battery-charging", string.Empty, Name);
            chargeStatusSensor.SetState(chargeStatus);
            AddUpdateSensor(chargeStatusId, chargeStatusSensor);

            var fullChargeLifetimeMinutes = powerStatus.BatteryFullLifetime;
            if (fullChargeLifetimeMinutes != -1)
                fullChargeLifetimeMinutes = Convert.ToInt32(Math.Round(TimeSpan.FromSeconds(fullChargeLifetimeMinutes).TotalMinutes));

            var fullChargeLifetimeId = $"{parentSensorSafeName}_full_charge_lifetime";
            var fullChargeLifetimeSensor = new DataTypeIntSensor(_updateInterval, "Full Charge Lifetime", fullChargeLifetimeId, string.Empty, "mdi:battery-high", string.Empty, Name);
            fullChargeLifetimeSensor.SetState(fullChargeLifetimeMinutes);
            AddUpdateSensor(fullChargeLifetimeId, fullChargeLifetimeSensor);

            var chargeRemainingPercentage = Convert.ToInt32(powerStatus.BatteryLifePercent * 100);
            var chargeRemainingPercentageId = $"{parentSensorSafeName}_charge_remaining_percentage";
            var chargeRemainingPercentageSensor = new DataTypeIntSensor(_updateInterval, "Charge Remaining Percentage", chargeRemainingPercentageId, string.Empty, "mdi:battery-high", "%", Name);
            chargeRemainingPercentageSensor.SetState(chargeRemainingPercentage);
            AddUpdateSensor(chargeRemainingPercentageId, chargeRemainingPercentageSensor);

            var chargeRemainingMinutes = powerStatus.BatteryLifeRemaining;
            if (chargeRemainingMinutes != -1)
                chargeRemainingMinutes = Convert.ToInt32(Math.Round(TimeSpan.FromSeconds(chargeRemainingMinutes).TotalMinutes));

            var chargeRemainingMinutesId = $"{parentSensorSafeName}_charge_remaining";
            var chargeRemainingMinutesSensor = new DataTypeIntSensor(_updateInterval, "Charge Remaining", chargeRemainingMinutesId, string.Empty, "mdi:battery-high", string.Empty, Name);
            chargeRemainingMinutesSensor.SetState(chargeRemainingMinutes);
            AddUpdateSensor(chargeRemainingMinutesId, chargeRemainingMinutesSensor);

            var powerlineStatus = powerStatus.PowerLineStatus.ToString();
            var powerlineStatusId = $"{parentSensorSafeName}_powerline_status";
            var powerlineStatusSensor = new DataTypeStringSensor(_updateInterval, "Powerline Status", powerlineStatusId, string.Empty, "mdi:power-plug", string.Empty, Name);
            powerlineStatusSensor.SetState(powerlineStatus);
            AddUpdateSensor(powerlineStatusId, powerlineStatusSensor);
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
