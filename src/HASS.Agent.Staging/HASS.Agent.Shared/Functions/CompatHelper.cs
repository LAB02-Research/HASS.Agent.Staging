
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Functions
{
    internal static class CompatHelper
    {
        internal static (int, int, int) SplitHAVerion(string haVersion)
        {
            var splitVersion = haVersion.Split('.');

            _ = int.TryParse(splitVersion[0], out int major);
            _ = int.TryParse(splitVersion[1], out int minor);

            int patch = 0;
            if (splitVersion.Length > 2)
                _ = int.TryParse(splitVersion[2], out patch);

            return (major, minor, patch);
        }

        internal static bool HassVersionEqualOrOver(string haVersion)
        {
            if (haVersion == null)
                return false;

            var (targetMajor, targetMinor, targetPatch) = SplitHAVerion(haVersion);
            var (major, minor, patch) = SplitHAVerion(HassApiManager.HaVersion);

            return major > targetMajor
                || major == targetMajor && minor > targetMinor
                || major == targetMajor && minor == targetMinor && patch > targetPatch;
        }
    }
}
