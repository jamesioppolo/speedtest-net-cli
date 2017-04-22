using SpeedtestNetCli.Query;
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
        private Func<IHttpQueryExecutor> _httpExecutor;

        public Speedtester(Func<IHttpQueryExecutor> httpExecutor)
        {
            _httpExecutor = httpExecutor;
        }

        public void Execute()
        {
            var client = _httpExecutor().Execute(new SpeedtestConfigQuery()).Result;
            var servers = _httpExecutor().Execute(new SpeedtestServerQuery()).Result;

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

            var imageUrl = bestServer.Attribute("url").Value.Replace("upload.php", "random2000x2000.jpg");
            var downSpeed = _httpExecutor().Execute(new SpeedtestQuery(imageUrl)).Result;
        }

        private XElement GetBestServerFrom(IList<XElement> closestServers)
        {
            foreach (var server in closestServers)
            {
                var averageLatency = 0.0;
                for (var latencyIteration = 0; latencyIteration < 5; latencyIteration++)
                {
                    averageLatency += DetermineLatencyTo(server.Attribute("host").Value)/5.0;
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
