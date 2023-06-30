﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing display-related info
    /// </summary>
    public class DisplaySensors : AbstractMultiValueSensor
    {
        private const string DefaultName = "display";
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public DisplaySensors(int? updateInterval = null, string name = DefaultName, string friendlyName = DefaultName, string id = default) : base(name ?? DefaultName, friendlyName ?? null, updateInterval ?? 30, id)
        {
            _updateInterval = updateInterval ?? 30;

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            // lowercase and safe name of the multivalue sensor
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            // fetch the latest display infos
            var displays = Screen.AllScreens;

            // prepare the data
            var primaryDisplayStr = string.Empty;
            var primaryDisplay = displays.FirstOrDefault(x => x.Primary);
            if (primaryDisplay != null) primaryDisplayStr = primaryDisplay.DeviceName.Split('\\').Last();

            // display count sensor
            var displayCount = displays.Length;

            var displayCountId = $"{parentSensorSafeName}_display_count";
            var displayCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Display Count", displayCountId, string.Empty, "mdi:monitor", string.Empty, Name);
            displayCountSensor.SetState(displayCount);

            if (!Sensors.ContainsKey(displayCountId)) Sensors.Add(displayCountId, displayCountSensor);
            else Sensors[displayCountId] = displayCountSensor;

            // nothing to do if there aren't any displays
            if (displayCount == 0) return;

            // primary display sensor
            var primaryDisplayId = $"{parentSensorSafeName}_primary_display";
            var primaryDisplaySensor = new DataTypeStringSensor(_updateInterval, $"{Name} Primary Display", primaryDisplayId, string.Empty, "mdi:monitor", string.Empty, Name);
            primaryDisplaySensor.SetState(primaryDisplayStr);

            if (!Sensors.ContainsKey(primaryDisplayId)) Sensors.Add(primaryDisplayId, primaryDisplaySensor);
            else Sensors[primaryDisplayId] = primaryDisplaySensor;

            // get non-virtual monitor info
            var monitors = Monitors.All?.ToList() ?? new List<Monitors>();

            // process all monitors
            foreach (var display in displays)
            {
                // id
                var id = SharedHelperFunctions.GetSafeValue(display.DeviceName);
                if (string.IsNullOrWhiteSpace(id)) continue;

                // name, remove the backslashes
                var name = display.DeviceName.Split('\\').Last();

                // prepare values
                var resolution = $"{display.Bounds.Width}x{display.Bounds.Height}";
                var virtualResolution = $"{display.Bounds.Width}x{display.Bounds.Height}";
                var width = display.Bounds.Width;
                var virtualWidth = display.Bounds.Width;
                var height = display.Bounds.Height;
                var virtualHeight = display.Bounds.Height;
                var rotated = 0;

                if (monitors.Any(x => x.Name == name))
                {
                    var monitor = monitors.Find(x => x.Name == name);
                    resolution = $"{monitor.PhysicalBounds.Width}x{monitor.PhysicalBounds.Height}";
                    width = monitor.PhysicalBounds.Width;
                    height = monitor.PhysicalBounds.Height;
                    rotated = monitor.RotatedDegrees;
                }

                // prepare the info
                var displayInfo = new DisplayInfo();
                displayInfo.Name = name;
                displayInfo.Resolution = resolution;
                displayInfo.VirtualResolution = virtualResolution;
                displayInfo.Width = width;
                displayInfo.VirtualWidth = virtualWidth;
                displayInfo.Height = height;
                displayInfo.VirtualHeight = virtualHeight;
                displayInfo.BitsPerPixel = display.BitsPerPixel;
                displayInfo.PrimaryDisplay = displayInfo.PrimaryDisplay;
                displayInfo.WorkingArea = $"{display.WorkingArea.Width}x{display.WorkingArea.Height}";
                displayInfo.WorkingAreaWidth = display.WorkingArea.Width;
                displayInfo.WorkingAreaHeight = display.WorkingArea.Height;
                displayInfo.RotatedDegrees = rotated;

                // process the sensor
                var info = JsonConvert.SerializeObject(displayInfo, Formatting.Indented);
                var displayInfoId = $"{parentSensorSafeName}_{id}";
                var displayInfoSensor = new DataTypeStringSensor(_updateInterval, $"{Name} {name}", displayInfoId, string.Empty, "mdi:monitor", string.Empty, Name, true);

                displayInfoSensor.SetState(name);
                displayInfoSensor.SetAttributes(info);

                if (!Sensors.ContainsKey(displayInfoId)) Sensors.Add(displayInfoId, displayInfoSensor);
                else Sensors[displayInfoId] = displayInfoSensor;
            }
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
