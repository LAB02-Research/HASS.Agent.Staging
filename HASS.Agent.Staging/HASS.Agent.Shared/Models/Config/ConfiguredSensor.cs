using System;
using HASS.Agent.Shared.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HASS.Agent.Shared.Models.Config
{
    /// <summary>
    /// Storable version of sensor objects
    /// </summary>
    public class ConfiguredSensor
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType Type { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        public int? UpdateInterval { get; set; }
        public string Query { get; set; } = string.Empty;
        public string Scope { get; set; }
        public string WindowName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Counter { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
