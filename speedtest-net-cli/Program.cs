using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace speedtest_net_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            using (var client = new HttpClient(httpHandler))
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("http://www.speedtest.net/speedtest-config.php"),
                    Method = HttpMethod.Get,
                };
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                client.DefaultRequestHeaders.CacheControl.NoCache = true;

                var guid = Guid.NewGuid();
                var response = client.GetAsync("http://www.speedtest.net/speedtest-config.php?x=" + guid.ToString()).Result;
                var text = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
