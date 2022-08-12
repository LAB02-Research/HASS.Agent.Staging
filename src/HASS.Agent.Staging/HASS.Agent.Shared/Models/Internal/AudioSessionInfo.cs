using System.Collections.Generic;

namespace HASS.Agent.Shared.Models.Internal
{
    public class AudioSessionInfoCollection
    {
        public AudioSessionInfoCollection()
        {
            //
        }

        public AudioSessionInfoCollection(List<AudioSessionInfo> audioSessions)
        {
            foreach (var audioSession in audioSessions) AudioSessions.Add(audioSession);
        }

        public List<AudioSessionInfo> AudioSessions { get; set; } = new List<AudioSessionInfo>();
    }

    public class AudioSessionInfo
    {
        public AudioSessionInfo()
        {
            //
        }

        public string Application { get; set; } = string.Empty;
        public bool Muted { get; set; }
        public float MasterVolume { get; set; } = 0f;
        public float PeakVolume { get; set; } = 0f;
    }
}
