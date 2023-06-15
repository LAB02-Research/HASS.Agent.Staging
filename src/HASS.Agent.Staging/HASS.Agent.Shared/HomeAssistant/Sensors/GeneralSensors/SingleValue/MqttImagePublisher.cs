using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace HASS.Agent.HomeAssistant.Sensors.GeneralSensors.SingleValue
{

    public class MqttImagePublisher
    {
        private readonly IMqttClient _client;
        private readonly string _topic;

        public MqttImagePublisher(string topic)
        {
            _topic = topic;
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
        }

        public async void PublishImage(byte[] image)
        {
            // Set up MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("homeassistant", 1883) // Replace with your MQTT broker address
                .WithClientId("homeassistant")
                .WithCredentials("homeassistant", "taemeef6LoWao1thahk3Ui3woz9ai8quaexooxohphie0ro5shahChee5gahs9Ya")
                .Build();

            // Connect to the MQTT broker
            await _client.ConnectAsync(options);

            // Create an MQTT message with the image bytes as the payload
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_topic)
                .WithPayload((byte[]) image)
                .Build();

            // Publish the MQTT message
            await _client.PublishAsync(message);

            // Disconnect from the MQTT broker
            await _client.DisconnectAsync();
        }

        public async void PublishImage(string pathToImage)
        {
            object screenCapture = null;

            using (Bitmap bmp = new Bitmap(1000, 800))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(1000, 800, 0, 0, new Size(100, 100), CopyPixelOperation.SourceCopy);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        screenCapture = ms.ToArray();
                    }
                }
            }

            // Set up MQTT client options
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("homeassistant", 1883) // Replace with your MQTT broker address
                .WithClientId("homeassistant")
                .WithCredentials("homeassistant", "taemeef6LoWao1thahk3Ui3woz9ai8quaexooxohphie0ro5shahChee5gahs9Ya")
                .Build();

            // Connect to the MQTT broker
            await _client.ConnectAsync(options);

            // Read the image file into a byte array
            byte[] imageBytes = File.ReadAllBytes(pathToImage); // Replace with the actual image file path

            // Create an MQTT message with the image bytes as the payload
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_topic)
                .WithPayload((byte[]) screenCapture)
                .Build();

            // Publish the MQTT message
            await _client.PublishAsync(message);

            // Disconnect from the MQTT broker
            await _client.DisconnectAsync();
        }
    }
}