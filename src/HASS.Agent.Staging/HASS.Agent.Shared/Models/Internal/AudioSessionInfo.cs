namespace HASS.Agent.Shared.Models.Internal
{
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
