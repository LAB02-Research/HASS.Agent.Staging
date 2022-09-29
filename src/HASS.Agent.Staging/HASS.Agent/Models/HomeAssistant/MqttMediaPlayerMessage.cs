using HASS.Agent.Enums;

namespace HASS.Agent.Media;

public class MqttMediaPlayerMessage
{
    public string Title { get; set; }
    public MediaPlayerState State { get; set; }
    public int Volume { get; set; }
    public bool Muted { get; set; }
}