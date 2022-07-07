using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Commands
{
    /// <summary>
    /// Command to simulate a keypress
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class KeyCommand : AbstractCommand
    {
        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public const int VK_VOLUME_MUTE = 0xAD;
        public const int VK_VOLUME_UP = 0xAF;
        public const int VK_VOLUME_DOWN = 0xAE;

        public byte KeyCode { get; set; }

        public KeyCommand(byte keyCode, string name = "Key", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? "Key", entityType, id) => KeyCode = keyCode;

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return new CommandDiscoveryConfigModel
            {
                Name = Name,
                Unique_id = Id,
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Command_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/set",
                Action_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/action",
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Device = deviceConfig
            };
        }
        
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        public override string GetState() => "OFF";

        public override void TurnOff()
        {
            //
        }

        public override void TurnOn() => keybd_event(KeyCode, 0, KEYEVENTF_EXTENTEDKEY, IntPtr.Zero);

        public override void TurnOnWithAction(string action)
        {
            //
        }
    }
}
