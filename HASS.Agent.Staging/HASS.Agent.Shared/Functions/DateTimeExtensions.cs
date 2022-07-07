using System;
using System.Collections.Generic;
using System.Text;

namespace HASS.Agent.Shared.Functions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts the DateTime object to a timezone-containing string
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToTimeZoneString(this DateTime datetime) => $"{datetime.ToUniversalTime():u}";
    }
}
