using System.Collections.Generic;

namespace HASS.Agent.Shared.Models.Internal
{
    /// <summary>
    /// Contains Windows update information
    /// </summary>
    public class WindowsUpdateInfo
    {
        public WindowsUpdateInfo()
        {
            //
        }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> KbArticleIDs { get; set; } = new List<string>();
        public bool Hidden { get; set; }
        public string SupportUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
    }
}
