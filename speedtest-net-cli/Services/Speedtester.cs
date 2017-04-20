using SpeedtestNetCli.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                    averageLatency += DetermineLatencyTo(server.Attribute("url").Value)/3.0;
                }
                server.Add(new XAttribute("latency", averageLatency));
            }

            return closestServers.OrderBy(server => Convert.ToDouble(server.Attribute("latency").Value)).FirstOrDefault();
        }

        private double DetermineLatencyTo(string url)
        {
            var latencyAddress = url.Replace("upload.php", "latency.txt");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                client.DefaultRequestHeaders.CacheControl.NoCache = true;

                try
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var response = client.GetAsync(latencyAddress).Result;
                    return response.IsSuccessStatusCode 
                            ? sw.ElapsedMilliseconds 
                            : 3600;
                }
                catch (HttpRequestException)
                {
                    return 3600;
                }
                
            }
        }
    }
}
