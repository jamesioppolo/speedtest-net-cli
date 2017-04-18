using SpeedtestNetCli.Utilities;
using System;
using System.Linq;
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
            var config =  _speedtestConfigurationRetriever.GetConfig().Result;
            var servers = _speedtestServerRetriever.GetServers().Result;

            var currentLocation = GetServerLocation(config.Descendants("client").First());
            foreach (var server in servers.Descendants("server"))
            {
                server.Add(new XAttribute("d", Distance.Between(currentLocation, GetServerLocation(server))));
            }

            var closestFiveServers = (from node in servers.Descendants("server")
                                      orderby Convert.ToDouble(node.Attribute("d").Value) ascending
                                      select node).Take(5);
             
        }

        private Location GetServerLocation(XElement node)
        {
            return new Location
            {
                Latitude = Convert.ToDouble(node.Attribute("lat").Value),
                Longitude = Convert.ToDouble(node.Attribute("lon").Value)
            };
        }
    }
}
