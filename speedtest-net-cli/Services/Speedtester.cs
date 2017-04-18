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

            var configXml = config.Descendants("client").First();
            var currentLocation = GetServerLocation(configXml);
            foreach (XElement server in servers.Descendants("server"))
            {
                var serverLocation = GetServerLocation(server);
                var distanceToServer = Distance.Between(currentLocation, serverLocation);
            }
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
