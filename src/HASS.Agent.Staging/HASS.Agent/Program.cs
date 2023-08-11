using System.Diagnostics;
using System.Globalization;
using System.Text;
using HASS.Agent.Compatibility;
using HASS.Agent.Enums;
using HASS.Agent.Forms;
using HASS.Agent.Forms.ChildApplications;
using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Settings;
using HASS.Agent.Shared.Extensions;
using Serilog;
using Serilog.Events;

namespace HASS.Agent
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(Variables.SyncfusionLicense);

                LoggingManager.PrepareLogging(args);

                Variables.ExtendedLogging = SettingsManager.GetExtendedLoggingSetting();

#if DEBUG
                Variables.ExtendedLogging = true;
                Variables.LevelSwitch.MinimumLevel = LogEventLevel.Debug;

                Log.Debug("[MAIN] DEBUG BUILD - TESTING PURPOSES ONLY");

                // make sure we catch 'm all
                AppDomain.CurrentDomain.FirstChanceException += LoggingManager.CurrentDomainOnFirstChanceException;
#else
                if (Variables.ExtendedLogging)
                {
                    Variables.LevelSwitch.MinimumLevel = LogEventLevel.Debug;
                    Log.Information("[MAIN] Extended logging enabled");

                    // make sure we catch 'm all
                    AppDomain.CurrentDomain.FirstChanceException += LoggingManager.CurrentDomainOnFirstChanceException;
                }
#endif

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var childApp = LaunchAsChildApplication(args);

                var settingsLoaded = SettingsManager.LoadAsync(!childApp).GetAwaiter().GetResult();
                if (!settingsLoaded)
                {
                    Log.Error(
                        "[PROGRAM] Something went wrong while loading the settings. Check appsettings.json, or delete the file to start fresh.");
                    Log.CloseAndFlush();
                    return;
                }

                LocalizationManager.Initialize();

                Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
                Application.SetDefaultFont(Variables.DefaultFont);

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                HelperFunctions.SetMsgBoxStyle(Variables.DefaultFont);

                if (LaunchedAsChildApplication(args))
                    return;

                Variables.MainForm = new Main();
                Application.Run(new CustomApplicationContext(Variables.MainForm));
            }
            catch (AccessViolationException ex)
            {
                Log.Fatal(ex, "[PROGRAM] AccessViolationException: {err}", ex.Message);
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[PROGRAM] {err}", ex.Message);
                Log.CloseAndFlush();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Checks whether we're asked to launch as a child application
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static bool LaunchAsChildApplication(string[] args)
        {
            return args.Any(x => x == "update")
                   || args.Any(x => x == "portreservation")
                   || args.Any(x => x == "restart")
                   || args.Any(x => x == "service_disable")
                   || args.Any(x => x == "service_enabled")
                   || args.Any(x => x == "service_start")
                   || args.Any(x => x == "service_stop")
                   || args.Any(x => x == "service_reinstall")
                   || args.Any(x => x == "compat_names");
        }

        /// <summary>
        /// Launches as a child application according to the provided arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool LaunchedAsChildApplication(string[] args)
        {
            try
            {
                if (args.Any(x => x == "update"))
                {
                    Log.Information("[SYSTEM] Post-update mode activated");
                    Variables.ChildApplicationMode = true;

                    var postUpdate = new PostUpdate();
                    Application.Run(postUpdate);

                    return true;
                }

                if (args.Any(x => x == "portreservation"))
                {
                    Log.Information("[SYSTEM] Port reservation mode activated");
                    Variables.ChildApplicationMode = true;


                    var portReservation = new PortReservation();
                    Application.Run(portReservation);

                    return true;
                }

                if (args.Any(x => x == "restart"))
                {
                    Log.Information("[SYSTEM] Restart mode activated");
                    Variables.ChildApplicationMode = true;

                    var restart = new Restart();
                    Application.Run(restart);

                    return true;
                }

                if (args.Any(x => x == "service_disable"))
                {
                    Log.Information("[SYSTEM] Set service disabled mode activated");
                    Variables.ChildApplicationMode = true;

                    var serviceState = new ServiceSetState(ServiceDesiredState.Disabled);
                    Application.Run(serviceState);

                    return true;
                }

                if (args.Any(x => x == "service_enabled"))
                {
                    Log.Information("[SYSTEM] Set service enabled mode activated");
                    Variables.ChildApplicationMode = true;

                    var serviceState = new ServiceSetState(ServiceDesiredState.Automatic);
                    Application.Run(serviceState);

                    return true;
                }

                if (args.Any(x => x == "service_start"))
                {
                    Log.Information("[SYSTEM] Start service mode activated");
                    Variables.ChildApplicationMode = true;

                    var serviceState = new ServiceSetState(ServiceDesiredState.Started);
                    Application.Run(serviceState);

                    return true;
                }

                if (args.Any(x => x == "service_stop"))
                {
                    Log.Information("[SYSTEM] Stop service mode activated");
                    Variables.ChildApplicationMode = true;


                    var serviceState = new ServiceSetState(ServiceDesiredState.Stopped);
                    Application.Run(serviceState);

                    return true;
                }

                if (args.Any(x => x == "service_reinstall"))
                {
                    Log.Information("[SYSTEM] Reinstall service mode activated");
                    Variables.ChildApplicationMode = true;

                    var serviceReinstall = new ServiceReinstall();
                    Application.Run(serviceReinstall);

                    return true;
                }

                if(args.Any(x => x == "compat_names"))
                {
                    Log.Information("[SYSTEM] Rename entity names mode activated [HA 2023.8]");
                    Variables.ChildApplicationMode = true;

                    var compatibilityTask = new CompatibilityTask(new NameCompatibilityTask());
                    Application.Run(compatibilityTask);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[PROGRAM] Error while trying to determine child-application mode: {err}", ex.Message);
                return false;
            }
        }
    }
}