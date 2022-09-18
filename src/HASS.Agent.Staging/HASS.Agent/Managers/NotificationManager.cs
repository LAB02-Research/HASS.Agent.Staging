using HASS.Agent.Functions;
using HASS.Agent.HomeAssistant;
using HASS.Agent.Models.HomeAssistant;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Managers
{
    internal static class NotificationManager
    {
        /// <summary>
        /// Initializes the notification manager
        /// </summary>
        internal static void Initialize()
        {
            if (!Variables.AppSettings.NotificationsEnabled)
            {
                Log.Information("[NOTIFIER] Disabled");
                return;
            }

            if (!Variables.AppSettings.LocalApiEnabled)
            {
                Log.Warning("[NOTIFIER] Local API is disabled, unable to receive notifications");
                return;
            }
            
            ToastNotificationManagerCompat.OnActivated += OnNotificationButtonPressed;

            // no task other than logging
            Log.Information("[NOTIFIER] Ready");
        }

        private static async void OnNotificationButtonPressed(ToastNotificationActivatedEventArgsCompat e)
        {
            await HassApiManager.FireEvent("hass_agent_notifications", new
            {
                device_name = HelperFunctions.GetConfiguredDeviceName(),
                action = e.Argument
            });
        }

        /// <summary>
        /// Show a notification object as a toast message
        /// </summary>
        /// <param name="notification"></param>
        internal static async void ShowNotification(Notification notification)
        {
            try
            {
                // are notifications enabled?
                if (!Variables.AppSettings.NotificationsEnabled) return;

                // prepare new toast
                var toastBuilder = new ToastContentBuilder();

                // prepare title
                if (string.IsNullOrWhiteSpace(notification.Title)) notification.Title = "Home Assistant";
                toastBuilder.AddHeader("HASS.Agent", notification.Title, string.Empty);

                // prepare image
                if (!string.IsNullOrWhiteSpace(notification.Data?.Image))
                {
                    var (success, localFile) = await StorageManager.DownloadImageAsync(notification.Data.Image);
                    if (success) toastBuilder.AddInlineImage(new Uri(localFile));
                    else Log.Error("[NOTIFIER] Image download failed, dropping: {img}", notification.Data.Image);
                }

                // prepare message
                toastBuilder.AddText(notification.Message);

                if (notification.Data?.Actions.Count > 0)
                {
                    foreach (var action in notification.Data.Actions)
                    {
                        if (!string.IsNullOrEmpty(action.Action))
                        {
                            toastBuilder.AddButton(action.Title, ToastActivationType.Background, action.Action);
                        }
                    }
                }

                // check for duration limit
                if (notification.Data?.Duration > 0)
                {
                    // there's a duration added, so show for x seconds
                    // todo: unreliable
                    toastBuilder.Show(toast =>
                    {
                        toast.ExpirationTime = DateTime.Now.AddSeconds(notification.Data.Duration);
                    });

                    return;
                }

                // show indefinitely, but clear on reboot
                toastBuilder.Show();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFICATIONS] Error while showing notification:\r\n{json}", JsonConvert.SerializeObject(notification, Formatting.Indented));
            }
        }
    }
}
