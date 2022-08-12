using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ByteSizeLib;
using HASS.Agent.Shared.Functions;
using HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue.DataTypes;
using HASS.Agent.Shared.Models.HomeAssistant;
using HASS.Agent.Shared.Models.Internal;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.GeneralSensors.MultiValue
{
    /// <summary>
    /// Multivalue sensor containing network and NIC info
    /// </summary>
    public class NetworkSensors : AbstractMultiValueSensor
    {
        private readonly int _updateInterval;

        public string NetworkCard { get; protected set; }
        private readonly bool _useSpecificCard = false;

        public sealed override Dictionary<string, AbstractSingleValueSensor> Sensors { get; protected set; } = new Dictionary<string, AbstractSingleValueSensor>();

        public NetworkSensors(int? updateInterval = null, string name = "network", string networkCard = "*", string id = default) : base(name ?? "network", updateInterval ?? 30, id)
        {
            _updateInterval = updateInterval ?? 30;

            NetworkCard = networkCard;
            _useSpecificCard = networkCard != "*" && !string.IsNullOrEmpty(networkCard);

            UpdateSensorValues();
        }

        public sealed override void UpdateSensorValues()
        {
            // lowercase and safe name of the multivalue sensor
            var parentSensorSafeName = SharedHelperFunctions.GetSafeValue(Name);

            // get nic info
            var nicCount = 0;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var nic in networkInterfaces)
            {
                try
                {
                    if (nic == null) continue;

                    // test if we need to show this card
                    if (_useSpecificCard && nic.Id != NetworkCard) continue;

                    // id
                    var id = nic.Id.Replace("{", "").Replace("}", "").Replace("-", "").ToLower();
                    if (string.IsNullOrWhiteSpace(id)) continue;

                    // prepare the info
                    var networkInfo = new NetworkInfo();
                    networkInfo.Name = nic.Name;
                    networkInfo.NetworkInterfaceType = nic.NetworkInterfaceType.ToString();
                    networkInfo.SpeedBitsPerSecond = nic.Speed;
                    networkInfo.OperationalStatus = nic.OperationalStatus.ToString();

                    // get interface stats
                    var interfaceStats = nic.GetIPv4Statistics();

                    // process the stats
                    networkInfo.DataReceivedMB = Math.Round(ByteSize.FromBytes(interfaceStats.BytesReceived).MegaBytes);
                    networkInfo.DataSentMB = Math.Round(ByteSize.FromBytes(interfaceStats.BytesSent).MegaBytes);
                    networkInfo.IncomingPacketsDiscarded = interfaceStats.IncomingPacketsDiscarded;
                    networkInfo.IncomingPacketsWithErrors = interfaceStats.IncomingPacketsWithErrors;
                    networkInfo.IncomingPacketsWithUnknownProtocol = interfaceStats.IncomingUnknownProtocolPackets;
                    networkInfo.OutgoingPacketsDiscarded = interfaceStats.OutgoingPacketsDiscarded;
                    networkInfo.OutgoingPacketsWithErrors = interfaceStats.OutgoingPacketsWithErrors;

                    // get nic properties
                    var nicProperties = nic.GetIPProperties();

                    // process the properties
                    foreach (var unicast in nicProperties.UnicastAddresses)
                    {
                        var ip = unicast.Address.ToString();
                        if (!string.IsNullOrEmpty(ip) && !networkInfo.IpAddresses.Contains(ip)) networkInfo.IpAddresses.Add(ip);

                        var mac = nic.GetPhysicalAddress().ToString();
                        if (!string.IsNullOrEmpty(mac) && !networkInfo.MacAddresses.Contains(mac)) networkInfo.MacAddresses.Add(mac);
                    }

                    foreach (var gateway in nicProperties.GatewayAddresses)
                    {
                        var gatewayAddress = gateway.Address.ToString();
                        if (!string.IsNullOrEmpty(gatewayAddress) && !networkInfo.Gateways.Contains(gatewayAddress)) networkInfo.Gateways.Add(gatewayAddress);
                    }

                    networkInfo.DhcpEnabled = nicProperties.GetIPv4Properties().IsDhcpEnabled;

                    foreach (var dhcp in nicProperties.DhcpServerAddresses)
                    {
                        var dhcpAddress = dhcp.ToString();
                        if (!string.IsNullOrEmpty(dhcpAddress) && !networkInfo.DhcpAddresses.Contains(dhcpAddress)) networkInfo.DhcpAddresses.Add(dhcpAddress);
                    }

                    networkInfo.DnsEnabled = nicProperties.IsDnsEnabled;
                    networkInfo.DnsSuffix = nicProperties.DnsSuffix;

                    foreach (var dns in nicProperties.DnsAddresses)
                    {
                        var dnsAddress = dns.ToString();
                        if (!string.IsNullOrEmpty(dnsAddress) && !networkInfo.DnsAddresses.Contains(dnsAddress)) networkInfo.DnsAddresses.Add(dnsAddress);
                    }

                    // process the sensor
                    var info = JsonConvert.SerializeObject(networkInfo, Formatting.Indented);
                    var networkInfoId = $"{parentSensorSafeName}_{id}";
                    var networkInfoSensor = new DataTypeStringSensor(_updateInterval, $"{Name} {nic.Name}", networkInfoId, string.Empty, "mdi:lan", string.Empty, Name, true);

                    networkInfoSensor.SetState(nic.OperationalStatus.ToString());
                    networkInfoSensor.SetAttributes(info);

                    if (!Sensors.ContainsKey(networkInfoId)) Sensors.Add(networkInfoId, networkInfoSensor);
                    else Sensors[networkInfoId] = networkInfoSensor;

                    // nic count
                    nicCount++;
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "[NETWORK] [{name}] Error querying NIC: {msg}", Name, ex.Message);
                }
            }

            // nic count
            var nicCountId = $"{parentSensorSafeName}_total_network_card_count";
            var nicCountSensor = new DataTypeIntSensor(_updateInterval, $"{Name} Network Card Count", nicCountId, string.Empty, "mdi:lan", string.Empty, Name);
            nicCountSensor.SetState(nicCount);

            if (!Sensors.ContainsKey(nicCountId)) Sensors.Add(nicCountId, nicCountSensor);
            else Sensors[nicCountId] = nicCountSensor;
        }

        public override DiscoveryConfigModel GetAutoDiscoveryConfig() => null;
    }
}
