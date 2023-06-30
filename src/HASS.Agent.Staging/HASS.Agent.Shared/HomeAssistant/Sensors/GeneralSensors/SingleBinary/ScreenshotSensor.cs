using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using HASS.Agent.Shared.Models.HomeAssistant;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    //https://stevessmarthomeguide.com/adding-an-mqtt-device-to-home-assistant/
    /// <summary>
    /// Sensor containing the coördinates of the device
    /// </summary>
    public class ScreenshotSensor : AbstractSingleBinarySensor
    {
        public int ScreenNumber { get; protected set; }

        public ScreenshotSensor(string screenNumber, int? updateInterval = 10, string name = "screenshot", string id = default) : base(name ?? "screenshot", updateInterval ?? 30, id)
        {
            ScreenNumber = int.TryParse(screenNumber, out _) ? Convert.ToInt32(screenNumber) : 0;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new PictureSensorDiscoveryConfigModel()
            {
                Name = Name,
                FriendlyName = FriendlyName,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                Icon = "mdi:camera",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/sensor/{deviceConfig.Name}/availability",
                Device_class = "motion"
            });
        }

        public override byte[] GetState()
        {
            int screenCount = Screen.AllScreens.Length;
            int requestedScreen = ScreenNumber;
            if (screenCount > requestedScreen) return CaptureScreen(requestedScreen);
            
            Log.Warning("[SCREENSHOT] Error capturing screen {index}- revert to capturing screen 0", requestedScreen);
            requestedScreen = 0;
            return CaptureScreen(requestedScreen);
        }

        public override string GetAttributes() => string.Empty;

        private byte[] CaptureScreen(int screenIndex)
        {
            try
            {
                return CapturePngFile(screenIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private static byte[] CapturePngFile(int screenIndex)
        {
            Rectangle captureRectangle = Screen.AllScreens[screenIndex].Bounds;

            Bitmap captureBitmap = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[screenIndex].Bounds.Height, PixelFormat.Format32bppArgb);
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);
            captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top,
                0, 0, captureRectangle.Size);

            byte[] captureByteArray = null;
            using var ms = new MemoryStream();
            captureBitmap.Save(ms, ImageFormat.Png);
            captureByteArray = ms.ToArray();
            return captureByteArray;
        }
    }
}

