using SpeedtestNetCli.Command;
using SpeedtestNetCli.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private ISpeedtestServerRetriever _speedtestServerRetriever;
        private IHttpQueryExecutor _httpExecutor;

        public Speedtester(ISpeedtestServerRetriever speedtestServerRetriever,
            IHttpQueryExecutor httpExecutor)
        {
            _speedtestServerRetriever = speedtestServerRetriever;
            _httpExecutor = httpExecutor;
        }

        public void Execute()
        {
            var client = _httpExecutor.Execute(new SpeedtestConfigCommand()).Result;
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

            var randomImageUrl = bestServer.Attribute("url").Value.Replace("upload.php", "random2000x2000.jpg");

            var httpHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            using (var httpClient = new HttpClient(httpHandler))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = httpClient.GetAsync($"{randomImageUrl}?x={Guid.NewGuid().ToString()}").Result;
                stopWatch.Stop();

                int length = int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
                double lengthMbits = length * 8.0 / 1024.0 / 1024.0;
                var downTimeSeconds = stopWatch.ElapsedMilliseconds / 1000.0;
                var downSpeed = lengthMbits / downTimeSeconds;
            }
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
