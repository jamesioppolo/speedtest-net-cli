using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpeedtestNetCli.Query
{
    public class SpeedtestServerQuery : IHttpQuery<XDocument>
    {
        public async Task<XDocument> Execute(HttpClient client, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync("http://c.speedtest.net/speedtest-servers-static.php", cancellationToken);
            var xmlDocument = XDocument.Load(await response.Content.ReadAsStreamAsync());
            return xmlDocument;
        }
    }
}
