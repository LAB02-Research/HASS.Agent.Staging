using System.Text.RegularExpressions;

namespace HASS.Agent.Shared.Models.HomeAssistant
{
    /// <summary>
    /// Abstract discoverable object from which all commands and sensors are derived
    /// </summary>
    public abstract class AbstractDiscoverable
    {
        public string Domain { get; set; } = "switch";
        public string Name { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;

        private string _objectId = string.Empty;

        public string ObjectId
        {
            get
            {
                if (!string.IsNullOrEmpty(_objectId)) return _objectId;

                _objectId = Regex.Replace(Name, "[^a-zA-Z0-9_-]", "_");
                return _objectId;
            }

            set => _objectId = Regex.Replace(value, "[^a-zA-Z0-9_-]", "_");
        }


        public string Id { get; set; } = string.Empty;

        public abstract DiscoveryConfigModel GetAutoDiscoveryConfig();
        public abstract void ClearAutoDiscoveryConfig();
    }
}
