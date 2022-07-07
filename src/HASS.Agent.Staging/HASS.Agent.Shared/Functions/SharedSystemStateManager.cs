using System;
using System.Threading.Tasks;
using HASS.Agent.Shared.Enums;
using Microsoft.Win32;
using Serilog;

namespace HASS.Agent.Shared.Functions
{
    /// <summary>
    /// Monitors and logs the system's state
    /// </summary>
    public static class SharedSystemStateManager
    {
        static SharedSystemStateManager() => Initialize();

        /// <summary>
        /// Notes the last time something happened to the system's state, ie. user logged on, session locked, etc
        /// </summary>
        public static DateTime LastSystemStateChange { get; private set; } = DateTime.Now;

        /// <summary>
        /// Contains the last event that happened to the system, ie. user logged on, session locked, etc
        /// </summary>
        public static SystemStateEvent LastSystemStateEvent { get; private set; } = SystemStateEvent.ApplicationStarted;

        /// <summary>
        /// Sets the provided system state event
        /// </summary>
        /// <param name="systemStateEvent"></param>
        public static void SetSystemStateEvent(SystemStateEvent systemStateEvent)
        {
            LastSystemStateEvent = systemStateEvent;
            LastSystemStateChange = DateTime.Now;
        }

        /// <summary>
        /// Initializes the systemstate manager and binds to Windows' event announcements
        /// </summary>
        private static async void Initialize()
        {
            await Task.Run(Monitor);
        }

        /// <summary>
        /// Hooks onto system events and acts accordingly
        /// </summary>
        private static void Monitor()
        {
            try
            {
                LastSystemStateChange = DateTime.Now;

                SystemEvents.SessionEnded += delegate (object sender, SessionEndedEventArgs e)
                {
                    LastSystemStateEvent = e.Reason switch
                    {
                        SessionEndReasons.Logoff => SystemStateEvent.Logoff,
                        SessionEndReasons.SystemShutdown => SystemStateEvent.SystemShutdown,
                        _ => LastSystemStateEvent
                    };
                };

                SystemEvents.SessionEnding += delegate (object sender, SessionEndingEventArgs e)
                {
                    LastSystemStateEvent = e.Reason switch
                    {
                        SessionEndReasons.Logoff => SystemStateEvent.Logoff,
                        SessionEndReasons.SystemShutdown => SystemStateEvent.SystemShutdown,
                        _ => LastSystemStateEvent
                    };
                };

                SystemEvents.PowerModeChanged += delegate (object sender, PowerModeChangedEventArgs e)
                {
                    // we're either going to sleep/hibernation or coming out of it
                    // if we're going under, process as fast as possible because we don't know how/when we're coming out of it
                    LastSystemStateEvent = e.Mode switch
                    {
                        PowerModes.Resume => SystemStateEvent.Resume,
                        PowerModes.Suspend => SystemStateEvent.Suspend,
                        _ => LastSystemStateEvent
                    };
                };

                SystemEvents.SessionSwitch += delegate (object sender, SessionSwitchEventArgs e)
                {
                    // something changed to the session (ie. local or remote login, logout, etc)
                    LastSystemStateEvent = e.Reason switch
                    {
                        SessionSwitchReason.ConsoleConnect => SystemStateEvent.ConsoleConnect,
                        SessionSwitchReason.ConsoleDisconnect => SystemStateEvent.ConsoleDisconnect,
                        SessionSwitchReason.RemoteConnect => SystemStateEvent.RemoteConnect,
                        SessionSwitchReason.RemoteDisconnect => SystemStateEvent.RemoteDisconnect,
                        SessionSwitchReason.SessionLock => SystemStateEvent.SessionLock,
                        SessionSwitchReason.SessionLogoff => SystemStateEvent.SessionLogoff,
                        SessionSwitchReason.SessionLogon => SystemStateEvent.SessionLogon,
                        SessionSwitchReason.SessionRemoteControl => SystemStateEvent.SessionRemoteControl,
                        SessionSwitchReason.SessionUnlock => SystemStateEvent.SessionUnlock,
                        _ => LastSystemStateEvent
                    };
                };
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[SHAREDSYSTEMSTATE] Error while processing change: {err}", ex.Message);
            }
        }
    }
}
