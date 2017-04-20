using SpeedtestNetCli.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace SpeedtestNetCli.Services
{
    public interface ISpeedtester
    {
        void Execute();
    }

    public class Speedtester : ISpeedtester
    {
        private ISpeedtestConfigurationRetriever _speedtestConfigurationRetriever;
        private ISpeedtestServerRetriever _speedtestServerRetriever;

        public Speedtester(ISpeedtestConfigurationRetriever speedtestConfigurationRetriever,
            ISpeedtestServerRetriever speedtestServerRetriever)
        {
            _speedtestConfigurationRetriever = speedtestConfigurationRetriever;
            _speedtestServerRetriever = speedtestServerRetriever;
        }

        public void Execute()
        {
            var client =  _speedtestConfigurationRetriever.GetConfig().Result;
            var servers = _speedtestServerRetriever.GetServers().Result;

            var clientLocation = new Location(client.Descendants("client").First());
            foreach (var server in servers.Descendants("server"))
            {
                server.Add(new XAttribute("clientDistance", clientLocation.DistanceTo(new Location(server))));
            }

            var closestServers = servers.Descendants("server")
                                            .OrderBy(server => Convert.ToDouble(server.Attribute("clientDistance").Value))
                                            .Take(5)
                                            .ToList();

            var bestServer = GetBestServerFrom(closestServers);
        }

        private XElement GetBestServerFrom(IList<XElement> closestServers)
        {
            foreach (var server in closestServers)
            {
                var averageLatency = 0.0;
                for (var latencyIteration = 0; latencyIteration < 3; latencyIteration++)
                {
                    averageLatency += DetermineLatencyTo(server.Attribute("host").Value)/3.0;
                }
                server.Add(new XAttribute("latency", averageLatency));
            }

            return closestServers.OrderBy(server => Convert.ToDouble(server.Attribute("latency").Value)).FirstOrDefault();
        }

        private long DetermineLatencyTo(string url)
        {
            using (var pingTest = new Ping())
            { 
                try
                {
                    var result = pingTest.SendPingAsync(new Uri($"http://{url}").Host, 3600).Result;
                    return result.RoundtripTime;
                }
                catch (PingException)
                {
                    return 3600;
                }
            }
        }
    }
}
