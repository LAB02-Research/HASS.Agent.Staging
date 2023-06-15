using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.SingleValue
{
    //https://stevessmarthomeguide.com/adding-an-mqtt-device-to-home-assistant/
    /// <summary>
    /// Sensor containing the coördinates of the device
    /// </summary>
    public class ScreenshotSensor : AbstractSingleBinarySensor
    {
        public ScreenshotSensor(int? updateInterval = 10, string name = "screenshot", string id = default) : base(name ?? "screenshot", updateInterval ?? 30, id) { }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new PictureSensorDiscoveryConfigModel()
            {
                Name = Name,
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
        return CaptureMyScreen();
    }

    public override string GetAttributes() => string.Empty;

    private byte[] CaptureMyScreen()
    {
        try
        {
            return CaptureJpgFile();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return null;
        }
    }

    private static byte[] CaptureJpgFile()
    {
        Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

        Bitmap captureBitmap = new Bitmap(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height, PixelFormat.Format32bppArgb);
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

