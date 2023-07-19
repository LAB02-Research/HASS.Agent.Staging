using System.Collections.Generic;

namespace HASS.Agent.Shared.Models.Internal
{
    public class WindowsInfoCollection
    {
        public WindowsInfoCollection()
        {
            //
        }

        public WindowsInfoCollection(List<WindowsWindowInfo> windowsWindows)
        {
            foreach (var windowsUpdate in windowsWindows) WindowsOpenWindows.Add(windowsUpdate);
        }

        public List<WindowsWindowInfo> WindowsOpenWindows { get; set; } = new List<WindowsWindowInfo>();
    }

    /// <summary>
    /// Contains Windows window information
    /// </summary>
    public class WindowsWindowInfo
    {
        public WindowsWindowInfo()
        {
            //
        }

        public string Title { get; set; } = string.Empty;
        public string Handle { get; set; } = string.Empty;
    }
}
