using Windows.UI.Notifications;
using HASS.Agent.API;
using HASS.Agent.Functions;
using HASS.Agent.HomeAssistant;
using MQTTnet;
using Newtonsoft.Json;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Notification = HASS.Agent.Models.HomeAssistant.Notification;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace HASS.Agent.Managers
{
    internal static class NotificationManager
    {
        private static readonly string s_actionPrefix = "action=";

        private static AppNotificationManager _toastNotifier = AppNotificationManager.Default;

        /// <summary>
        /// Initializes the notification manager
        /// </summary>
        internal static void Initialize()
        {
            try
            {
                if (!Variables.AppSettings.NotificationsEnabled)
                {
                    Log.Information("[NOTIFIER] Disabled");

                    return;
                }

                if (!Variables.AppSettings.LocalApiEnabled && !Variables.AppSettings.MqttEnabled)
                {
                    Log.Warning("[NOTIFIER] Both local API and MQTT are disabled, unable to receive notifications");

                    return;
                }

                if (!Variables.AppSettings.MqttEnabled)
                    Log.Warning("[NOTIFIER] MQTT is disabled, not all aspects of actions might work as expected");
                else
                    _ = Task.Run(Variables.MqttManager.SubscribeNotificationsAsync);

                if (_toastNotifier.Setting != AppNotificationSetting.Enabled)
                    Log.Warning("[NOTIFIER] Showing notifications might fail, reason: {r}", _toastNotifier.Setting.ToString());


                _toastNotifier.NotificationInvoked += OnNotificationInvoked;

                _toastNotifier.Register();

                Log.Information("[NOTIFIER] Ready");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFIER] Error while initializing: {err}", ex.Message);
            }
        }

        /// <summary>
        /// Show a notification object as a toast message
        /// </summary>
        /// <param name="notification"></param>
        internal static async void ShowNotification(Notification notification)
        {
            try
            {
                if (!Variables.AppSettings.NotificationsEnabled || _toastNotifier == null)
                    return;

                var toastBuilder = new AppNotificationBuilder()
                    .AddText(notification.Title)
                    .AddText(notification.Message);

                if (!string.IsNullOrWhiteSpace(notification.Data?.Image))
                {
                    var (success, localFile) = await StorageManager.DownloadImageAsync(notification.Data.Image);
                    if (success)
                        toastBuilder.SetInlineImage(new Uri(localFile));
                    else
                        Log.Error("[NOTIFIER] Image download failed, dropping: {img}", notification.Data.Image);
                }

                if (notification.Data?.Actions.Count > 0)
                {
                    foreach (var action in notification.Data.Actions)
                    {
                        if (string.IsNullOrEmpty(action.Action))
                            continue;

                        toastBuilder.AddButton(new AppNotificationButton(action.Title)
                            .AddArgument("action", action.Action));
                    }
                }

                var toast = toastBuilder.BuildNotification();

                if (notification.Data?.Duration > 0)
                {
                    //TODO: unreliable
                    toast.Expiration = DateTime.Now.AddSeconds(notification.Data.Duration);
                }

                // show indefinitely
                _toastNotifier.Show(toast);
            }
            catch (Exception ex)
            {
                if (Variables.ExtendedLogging)
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}\r\n{json}", ex.Message, JsonConvert.SerializeObject(notification, Formatting.Indented));
                else
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}", ex.Message);
            }
        }

        private static string GetActionFromEventArgs(AppNotificationActivatedEventArgs e)
        {
            var startIndex = e.Argument.IndexOf(s_actionPrefix, StringComparison.Ordinal);
            return startIndex == -1 ? e.Argument : e.Argument.Remove(startIndex, s_actionPrefix.Length);
        }

        private static string GetInputFromEventArgs(AppNotificationActivatedEventArgs e) => e.UserInput.ContainsKey("input") ? e.UserInput["input"] : null;

        private static async void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs e)
        {
            try
            {
                var action = GetActionFromEventArgs(e);

                var haEventTask = HassApiManager.FireEvent("hass_agent_notifications", new
                {
                    device_name = HelperFunctions.GetConfiguredDeviceName(),
                    action
                });

                if (Variables.AppSettings.MqttEnabled)
                {
                    var haMessageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/notifications/{Variables.DeviceConfig.Name}/actions")
                        .WithPayload(JsonSerializer.Serialize(new
                        {
                            action,
                            input = GetInputFromEventArgs(e),
                        }, ApiDeserialization.SerializerOptions));

                    var mqttTask = Variables.MqttManager.PublishAsync(haMessageBuilder.Build());

                    await Task.WhenAny(haEventTask, mqttTask);
                }
                else
                {
                    await haEventTask;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFIER] Unable to process button: {err}", ex.Message);
            }
        }

        public static void Exit()
        {
            _toastNotifier.Unregister();
        }
    }
}