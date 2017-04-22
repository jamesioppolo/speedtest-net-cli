using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpeedtestNetCli.Command
{
    public class SpeedtestConfigCommand : IHttpQuery<XDocument>
    {
        public async Task<XDocument> Execute(HttpClient client)
        {
            XDocument xmlDocument = new XDocument();
            var response = await client.GetAsync("http://www.speedtest.net/speedtest-config.php?x=" + Guid.NewGuid().ToString());
            xmlDocument = XDocument.Load(await response.Content.ReadAsStreamAsync());
            return xmlDocument;
        }
    }
}
