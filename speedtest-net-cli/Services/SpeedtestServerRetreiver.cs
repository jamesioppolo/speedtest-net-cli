using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;

namespace SpeedtestNetCli.Services
{
    public interface ISpeedtestServerRetriever
    {
        Task<XmlDocument> GetServers();
    }

    public class SpeedtestServerRetriever : ISpeedtestServerRetriever
    {
        XmlDocument xmlDocument = new XmlDocument();

        public async Task<XmlDocument> GetServers()
        {
            var httpHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            using (var client = new HttpClient(httpHandler))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                client.DefaultRequestHeaders.CacheControl.NoCache = true;

                var response = await client.GetAsync("http://c.speedtest.net/speedtest-servers-static.php");
                xmlDocument.LoadXml(await response.Content.ReadAsStringAsync());
                return xmlDocument;
            }
        }
    }
}
