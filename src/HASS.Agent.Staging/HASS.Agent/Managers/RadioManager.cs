using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Radios;

namespace HASS.Agent.Managers
{
    internal static class RadioManager
    {
        public static List<Radio> AvailableRadio { get; private set; } = new();
        public static List<string> AvailableRadioNames { get => AvailableRadio.Select(r => r.Name).ToList(); }

        public static async Task Initialize()
        {
            var accessStatus = await Radio.RequestAccessAsync();
            if (accessStatus == RadioAccessStatus.Allowed)
            {
                foreach (var radio in await Radio.GetRadiosAsync())
                {
                    AvailableRadio.Add(radio);
                }

                Log.Information("[RADIOMGR] Ready");
            }
            else
            {
                Log.Fatal("[RADIOMGR] No permission granted for Bluetooth radio management");
            }
        }
    }
}
