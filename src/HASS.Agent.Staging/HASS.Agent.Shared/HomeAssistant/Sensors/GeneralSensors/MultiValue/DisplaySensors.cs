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

        private void AddUpdateSensor(string sensorId, AbstractSingleValueSensor sensor)
        {
            if (!Sensors.ContainsKey(sensorId))
                Sensors.Add(sensorId, sensor);
            else
                Sensors[sensorId] = sensor;
        }

        public sealed override void UpdateSensorValues()
        {
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            var displays = Screen.AllScreens;

            var primaryDisplayStr = string.Empty;
            var primaryDisplay = displays.FirstOrDefault(x => x.Primary);
            if (primaryDisplay != null)
                primaryDisplayStr = primaryDisplay.DeviceName.Split('\\').Last();

            var displayCount = displays.Length;
            var displayCountId = $"{parentSensorSafeName}_display_count";
            var displayCountSensor = new DataTypeIntSensor(_updateInterval, "Display Count", displayCountId, string.Empty, "mdi:monitor", string.Empty, Name);
            displayCountSensor.SetState(displayCount);
            AddUpdateSensor(displayCountId, displayCountSensor);

            if (displayCount == 0)
                return;

            var primaryDisplayId = $"{parentSensorSafeName}_primary_display";
            var primaryDisplaySensor = new DataTypeStringSensor(_updateInterval, "Primary Display", primaryDisplayId, string.Empty, "mdi:monitor", string.Empty, Name);
            primaryDisplaySensor.SetState(primaryDisplayStr);
            AddUpdateSensor(primaryDisplayId, primaryDisplaySensor);

            var monitors = Monitors.All?.ToList() ?? new List<Monitors>();

            foreach (var display in displays)
            {
                var id = SharedHelperFunctions.GetSafeValue(display.DeviceName);
                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var name = display.DeviceName.Split('\\').Last();

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

                var displayInfo = new DisplayInfo
                {
                    Name = name,
                    Resolution = resolution,
                    VirtualResolution = virtualResolution,
                    Width = width,
                    VirtualWidth = virtualWidth,
                    Height = height,
                    VirtualHeight = virtualHeight,
                    BitsPerPixel = display.BitsPerPixel,
                    PrimaryDisplay = display.Primary,
                    WorkingArea = $"{display.WorkingArea.Width}x{display.WorkingArea.Height}",
                    WorkingAreaWidth = display.WorkingArea.Width,
                    WorkingAreaHeight = display.WorkingArea.Height,
                    RotatedDegrees = rotated
                };

                var info = JsonConvert.SerializeObject(displayInfo, Formatting.Indented);
                var displayInfoId = $"{parentSensorSafeName}_{id}";
                var displayInfoSensor = new DataTypeStringSensor(_updateInterval, name, displayInfoId, string.Empty, "mdi:monitor", string.Empty, Name, true);
                displayInfoSensor.SetState(name);
                displayInfoSensor.SetAttributes(info);
                AddUpdateSensor(displayInfoId, displayInfoSensor);
            }
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
