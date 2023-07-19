using System.Collections.Generic;
using System.Linq;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Managers;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;
using HWND = System.IntPtr;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing Open Windows info
    /// </summary>
    public class OpenWindowsSensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "openwindows";
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public OpenWindowsSensors(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 900, id)
        {
            _updateInterval = updateInterval ?? 900;

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            // lowercase and safe name of the multivalue sensor
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            // fetch the latest updates
            var openWindows = OpenWindowGetter.GetOpenWindows();

            // driver update count
            var openWindowsCount = openWindows.Count;

            var openWindowCountId = $"{parentSensorSafeName}_open_window_count";
            var openWindowCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Number of open windows", openWindowCountId, string.Empty, "mdi:microsoft-windows", string.Empty, Name);
            openWindowCountSensor.SetState(openWindowsCount);

            if (!Sensors.ContainsKey(openWindowCountId)) Sensors.Add(openWindowCountId, openWindowCountSensor);
            else Sensors[openWindowCountId] = openWindowCountSensor;

            // open window list
            var openWindowsList = new List<string>();
            var openWindowsInfo = new WindowsInfoCollection();

            foreach (var openWindow in openWindows)
            {
                openWindowsInfo.WindowsOpenWindows.Add(new WindowsWindowInfo()
                {
                    Handle = openWindow.Key.ToString(),
                    Title= openWindow.Value
                });
            }

            var driverUpdatesStr = JsonConvert.SerializeObject(openWindowsInfo, Formatting.Indented);

            var openWindowsId = $"{parentSensorSafeName}_open_windows";

            var openWindowsSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Open Windows", openWindowsId, string.Empty, "mdi:microsoft-windows", string.Empty, Name, true);
            openWindowsSensor.SetState(openWindowsList.Count);
            openWindowsSensor.SetAttributes(driverUpdatesStr);

            if (!Sensors.ContainsKey(openWindowsId)) Sensors.Add(openWindowsId, openWindowsSensor);
            else Sensors[openWindowsId] = openWindowsSensor;

            // all done!
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }


    /// Move to Manager class (like WindowsWindowManager.cs ?)
    /// <summary>Contains functionality to get all the open windows.</summary>
    /// Credit goes to Tommy Carlier https://www.tcx.be/about/
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<HWND, string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern HWND GetShellWindow();
    }
}
