using HASS.Agent.Satellite.Service.Managers;
using HASS.Agent.Satellite.Service.Settings;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using Serilog.Events;

namespace HASS.Agent.Satellite.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // initialize serilog
            LoggingManager.PrepareLogging(args);

            Log.Information("[MAIN] Version: {v}", Variables.Version);

            // get extended logging settings
            Variables.ExtendedLogging = SettingsManager.GetExtendedLoggingSetting();

            if (Variables.ExtendedLogging)
            {
                Log.Information("[MAIN] Extended logging enabled");

                // make sure we catch 'm all
                AppDomain.CurrentDomain.FirstChanceException += LoggingManager.CurrentDomainOnFirstChanceException;
            }

#if DEBUG
            Variables.LevelSwitch.MinimumLevel = LogEventLevel.Debug;
            Log.Debug("[MAIN] DEBUGGING BUILD, NOT FOR PRODUCTION");
#endif

            Log.Information("[MAIN] Service started, initializing ..");

            // build and run the worker
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // configure windows service
                .UseWindowsService(options =>
                {
                    options.ServiceName = "hass.agent.satellite.service";
                })
                // configure serilog
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // bind our worker
                    services.AddHostedService<Worker>();

                    if (WindowsServiceHelpers.IsWindowsService())
                    {
                        Log.Information("[MAIN] Running as a service, initializing lifetime manager");

                        // bind our lifetime manager
                        services.AddSingleton<IHostLifetime, ServiceLifetimeManager>();
                    }
                    else Log.Information("[MAIN] Not running as a service, skipping lifetime manager");
                    
                }).UseSerilog();
    }
}
