﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MultipleKeysCommand : AbstractCommand
    {
        private const string DefaultName = "multiplekeys";

        public string State { get; protected set; }
        public List<string> Keys { get; set; }

        public MultipleKeysCommand(List<string> keys, string name = DefaultName, string friendlyName = DefaultName, CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? DefaultName, friendlyName ?? null, entityType, id)
        {
            Keys = keys;
            State = "OFF";
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Command_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/set",
                Action_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/action",
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Device = deviceConfig
            };
        }

        public override string GetState() => State;

        public override void TurnOff()
        {
            //
        }

        public override async void TurnOn()
        {
            try
            {
                State = "ON";

                foreach (var key in Keys)
                {
                    SendKeys.SendWait(key);
                    SendKeys.Flush();
                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[MULTIPLEKEYS] [{name}] Executing command failed: {ex}", Name, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        public override void TurnOnWithAction(string action)
        {
            //
        }
    }
}
