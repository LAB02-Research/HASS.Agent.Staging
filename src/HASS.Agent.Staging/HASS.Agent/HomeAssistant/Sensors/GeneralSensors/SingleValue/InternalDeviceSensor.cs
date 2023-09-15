﻿using HASS.Agent.Managers;
using HASS.Agent.Managers.DeviceSensors;
using HASS.Agent.Shared.Extensions;
using HASS.Agent.Shared.Models.HomeAssistant;
using Newtonsoft.Json;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the device's internal sensor data
    /// </summary>
    public class InternalDeviceSensor : AbstractSingleValueSensor
    {
        private const string DefaultName = "internaldevicesensor";

        public InternalDeviceSensorType SensorType { get; set; }

        private readonly IInternalDeviceSensor _internalDeviceSensor;

        public InternalDeviceSensor(string sensorType, int? updateInterval = 10, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            SensorType = Enum.Parse<InternalDeviceSensorType>(sensorType);
            _internalDeviceSensor = InternalDeviceSensorsManager.AvailableSensors.First(s => s.Type == SensorType);

            UseAttributes = _internalDeviceSensor.Attributes != Managers.DeviceSensors.InternalDeviceSensor.NoAttributes;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null)
                return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null)
                return null;

            var sensorDiscoveryConfigModel = new SensorDiscoveryConfigModel()
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:information-box-outline",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            };

            if (UseAttributes)
                sensorDiscoveryConfigModel.Json_attributes_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/attributes";

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(sensorDiscoveryConfigModel);
        }

        public override string GetState() => _internalDeviceSensor.Measurement;

        public override string GetAttributes() => JsonConvert.SerializeObject(_internalDeviceSensor.Attributes);
    }
}
