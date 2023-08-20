﻿using Windows.UI.Notifications;
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
using Microsoft.Windows.AppLifecycle;
using System.Windows.Markup;
using System.Text;
using System.Net;

namespace HASS.Agent.Managers
{
    internal static class NotificationManager
    {
        public const string NotificationLaunchArgument = "----AppNotificationActivated:";

        public static bool Ready { get; private set; } = false;

        private static readonly string s_actionPrefix = "action=";
        private static readonly string s_uriPrefix = "uri=";

        private static readonly AppNotificationManager _notificationManager = AppNotificationManager.Default;


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

                if (_notificationManager.Setting != AppNotificationSetting.Enabled)
                    Log.Warning("[NOTIFIER] Showing notifications might fail, reason: {r}", _notificationManager.Setting.ToString());


                _notificationManager.NotificationInvoked += OnNotificationInvoked;

                _notificationManager.Register();
                Ready = true;

                Log.Information("[NOTIFIER] Ready");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "[NOTIFIER] Error while initializing: {err}", ex.Message);
            }
        }

        private static string EncodeNotificationParameter(string parameter)
        {
            var encodedParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(parameter));
            // for some reason, Windows App SDK URL encodes the arguments even if they are already encoded
            // this is the reason the WebUtility.UrlEncode is missing from here
            return encodedParameter;
        }

        private static string DecodeNotificationParameter(string encodedParameter)
        {
            var urlDecodedParameter = WebUtility.UrlDecode(encodedParameter);
            return Encoding.UTF8.GetString(Convert.FromBase64String(urlDecodedParameter));
        }

        /// <summary>
        /// Show a notification object as a toast message
        /// </summary>
        /// <param name="notification"></param>
        internal static async void ShowNotification(Notification notification)
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            try
            {
                if (!Variables.AppSettings.NotificationsEnabled || _notificationManager == null)
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

                        var button = new AppNotificationButton(action.Title)
                            .AddArgument("action", EncodeNotificationParameter(action.Action));

                        if (action.Uri != null)
                            button.AddArgument("uri", EncodeNotificationParameter(action.Uri));

                        toastBuilder.AddButton(button);
                    }
                }

                if (notification.Data?.Inputs.Count > 0)
                {
                    foreach (var input in notification.Data.Inputs)
                    {
                        if (string.IsNullOrEmpty(input.Id))
                            continue;

                        toastBuilder.AddTextBox(input.Id, input.Text, input.Title);
                    }
                }

                var toast = toastBuilder.BuildNotification();

                if (notification.Data?.Duration > 0)
                {
                    //TODO: unreliable
                    toast.Expiration = DateTime.Now.AddSeconds(notification.Data.Duration);
                }

                _notificationManager.Show(toast);

                if (toast.Id == 0)
                {
                    Log.Error("[NOTIFIER] Notification '{err}' failed to show", notification.Title);
                }

            }
            catch (Exception ex)
            {
                if (Variables.ExtendedLogging)
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}\r\n{json}", ex.Message, JsonConvert.SerializeObject(notification, Formatting.Indented));
                else
                    Log.Fatal(ex, "[NOTIFIER] Error while showing notification: {err}", ex.Message);
            }
        }

        private static string GetValueFromEventArgs(AppNotificationActivatedEventArgs e, string startText)
        {
            var start = e.Argument.IndexOf(startText) + startText.Length;
            if (start < startText.Length)
                return null;

            var separatorIndex = e.Argument.IndexOf(";", start);
            var end = separatorIndex < 0 ? e.Argument.Length : separatorIndex;
            return DecodeNotificationParameter(e.Argument[start..end]);
        }

        private static IDictionary<string, string> GetInputFromEventArgs(AppNotificationActivatedEventArgs e) => e.UserInput.Count > 0 ? e.UserInput : null;

        private static async void OnNotificationInvoked(AppNotificationManager _, AppNotificationActivatedEventArgs e) => await HandleAppNotificationActivation(e);

        private static async Task HandleAppNotificationActivation(AppNotificationActivatedEventArgs e)
        {
            try
            {
                var action = GetValueFromEventArgs(e, s_actionPrefix);
                var input = GetInputFromEventArgs(e);
                var uri = GetValueFromEventArgs(e, s_uriPrefix);

                if (uri != null && Variables.AppSettings.NotificationsOpenActionUri)
                    HelperFunctions.LaunchUrl(uri);

                var haEventTask = HassApiManager.FireEvent("hass_agent_notifications", new
                {
                    device_name = HelperFunctions.GetConfiguredDeviceName(),
                    action,
                    input,
                    uri
                });

                if (Variables.AppSettings.MqttEnabled)
                {
                    var haMessageBuilder = new MqttApplicationMessageBuilder()
                        .WithTopic($"hass.agent/notifications/{Variables.DeviceConfig.Name}/actions")
                        .WithPayload(JsonConvert.SerializeObject(new
                        {
                            action,
                            input,
                            uri
                        }));

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

        internal static async void HandleNotificationLaunch()
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            Log.Information("[NOTIFIER] Launched with notification action");

            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (args.Kind != ExtendedActivationKind.AppNotification)
                return;

            var appNotificationArgs = args.Data as AppNotificationActivatedEventArgs;
            if (appNotificationArgs.Argument == null)
                return;

            await HandleAppNotificationActivation(appNotificationArgs);

            Log.Information("[NOTIFIER] Finished handling notification action");
        }

        internal static void Exit()
        {
            if (!Ready)
                throw new Exception("NotificationManager is not initialized");

            _notificationManager.Unregister();
        }
    }
}