using SpeedtestNetCli.Utilities;
using System;
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

            var closestFiveServers = servers.Descendants("server")
                                            .OrderBy(server => Convert.ToDouble(server.Attribute("clientDistance").Value))
                                            .Take(5);

            for (var latencyIteration = 0; latencyIteration < 3; latencyIteration++)
            {
                var latencyValue = PerformLatencyTest();
            }
        }

        private double PerformLatencyTest()
        {
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
                    var response = client.GetAsync("http://rockingham.wa.speedtest.optusnet.com.au/speedtest/latency.txt").Result;
                    return sw.ElapsedMilliseconds;
                }
                catch (HttpRequestException)
                {
                    return 3600;
                }
                
            }
        }
    }
}
