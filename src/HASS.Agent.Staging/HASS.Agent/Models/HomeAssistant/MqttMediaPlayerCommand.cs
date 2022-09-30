using System.Text.Json;
using HASS.Agent.Enums;

namespace HASS.Agent.Media;

public class MqttMediaPlayerCommand
{
    public MediaPlayerCommand Command { get; set; }
    public JsonElement Data { get; set; }
}