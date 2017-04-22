﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpeedtestNetCli.Query;
using SpeedtestNetCli.Utilities;

namespace SpeedtestNetCli.Services
{
    public interface IBestServerDeterminer
    {
        Task<XElement> GetBestServer();
    }

    public class BestServerDeterminer : IBestServerDeterminer
    {
        private readonly Func<IHttpQueryExecutor> _httpExecutor;

        public BestServerDeterminer(Func<IHttpQueryExecutor> httpExecutor)
        {
            _httpExecutor = httpExecutor;
        }

        public async Task<XElement> GetBestServer()
        {
            var client = await _httpExecutor().Execute(new SpeedtestConfigQuery());
            var servers = await _httpExecutor().Execute(new SpeedtestServerQuery());

            var clientLocation = new Location(client.Descendants("client").First());
            foreach (var server in servers.Descendants("server"))
            {
                server.Add(new XAttribute("clientDistance", clientLocation.DistanceTo(new Location(server))));
            }

            var closestServers = servers.Descendants("server")
                .OrderBy(server => Convert.ToDouble(server.Attribute("clientDistance").Value))
                .Take(5)
                .ToList();

            return GetLowestLatencyServerFrom(closestServers);
        }

        private static XElement GetLowestLatencyServerFrom(IList<XElement> closestServers)
        {
            foreach (var server in closestServers)
            {
                var averageLatency = 0.0;
                for (var latencyIteration = 0; latencyIteration < 5; latencyIteration++)
                {
                    averageLatency += DetermineLatencyTo(server.Attribute("host").Value) / 5.0;
                }
                server.Add(new XAttribute("latency", averageLatency));
            }

            return closestServers.OrderBy(server => Convert.ToDouble(server.Attribute("latency").Value)).FirstOrDefault();
        }

        private static long DetermineLatencyTo(string url)
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