using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using ByteSizeLib;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;
#pragma warning disable CS1591

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing local storage info
    /// </summary>
    public class StorageSensors : AbstractMultiValueSensor
    {
        private readonly int _updateInterval;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public StorageSensors(int? updateInterval = null, string name = "storage", string id = default) : base(name ?? "storage", updateInterval ?? 30, id)
        {
            _updateInterval = updateInterval ?? 30;

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            var driveCount = 0;

            // lowercase and safe name of the multivalue sensor
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (!(drive is { IsReady: true, DriveType: DriveType.Fixed })) continue;
                    if (string.IsNullOrWhiteSpace(drive.Name)) continue;

                    // name (letter)
                    var driveName = $"{drive.Name[..1].ToUpper()}";
                    var driveNameLower = driveName.ToLower();

                    // label
                    var driveLabel = string.IsNullOrEmpty(drive.VolumeLabel) ? "-" : drive.VolumeLabel;

                    // sensor value
                    var sensorValue = string.IsNullOrEmpty(drive.VolumeLabel) ? driveName : drive.VolumeLabel;

                    // prepare the info
                    var storageInfo = new StorageInfo();
                    storageInfo.Name = driveName;
                    storageInfo.Label = driveLabel;
                    storageInfo.FileSystem = drive.DriveFormat;

                    // total size
                    var totalSizeMb = Math.Round(ByteSize.FromBytes(drive.TotalSize).MegaBytes);
                    storageInfo.TotalSizeMB = totalSizeMb;

                    // available space
                    var availableSpaceMb = Math.Round(ByteSize.FromBytes(drive.AvailableFreeSpace).MegaBytes);
                    storageInfo.AvailableSpaceMB = availableSpaceMb;

                    // used space
                    var usedSpaceMb = totalSizeMb - availableSpaceMb;
                    storageInfo.UsedSpaceMB = usedSpaceMb;

                    // available space percentage
                    var availableSpacePercentage = (int)Math.Round((availableSpaceMb / totalSizeMb) * 100);
                    storageInfo.AvailableSpacePercentage = availableSpacePercentage;

                    // used space percentage
                    var usedSpacePercentage = (int)Math.Round((usedSpaceMb / totalSizeMb) * 100);
                    storageInfo.UsedSpacePercentage = usedSpacePercentage;

                    // process the sensor
                    var info = JsonConvert.SerializeObject(storageInfo, Formatting.Indented);
                    var driveInfoId = $"{parentSensorSafeName}_{driveNameLower}";
                    var driveInfoSensor = new DataTypeStringSensor(_updateInterval, $"{Name} {driveName}", driveInfoId, string.Empty, "mdi:harddisk", string.Empty, Name, true);

                    driveInfoSensor.SetState(sensorValue);
                    driveInfoSensor.SetAttributes(info);

                    if (!Sensors.ContainsKey(driveInfoId)) Sensors.Add(driveInfoId, driveInfoSensor);
                    else Sensors[driveInfoId] = driveInfoSensor;

                    // increment drive count
                    driveCount++;
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case UnauthorizedAccessException _:
                        case SecurityException _:
                            Log.Fatal(ex, "[STORAGE] [{name}] Disk access denied: {msg}", Name, ex.Message);
                            continue;
                        case DriveNotFoundException _:
                            Log.Fatal(ex, "[STORAGE] [{name}] Disk not found: {msg}", Name, ex.Message);
                            continue;
                        case IOException _:
                            Log.Fatal(ex, "[STORAGE] [{name}] Disk IO error: {msg}", Name, ex.Message);
                            continue;
                    }

                    Log.Fatal(ex, "[STORAGE] [{name}] Error querying disk: {msg}", Name, ex.Message);
                }
            }

            // drive count
            var driveCountId = $"{parentSensorSafeName}_total_disk_count";
            var driveCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Total Disk Count", driveCountId, string.Empty, "mdi:harddisk", string.Empty, Name);
            driveCountSensor.SetState(driveCount);

            if (!Sensors.ContainsKey(driveCountId)) Sensors.Add(driveCountId, driveCountSensor);
            else Sensors[driveCountId] = driveCountSensor;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
