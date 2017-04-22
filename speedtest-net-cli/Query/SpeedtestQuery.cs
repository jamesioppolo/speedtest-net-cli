using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpeedtestNetCli.Query
{
    public class SpeedtestQuery : IHttpQuery<double>
    {
        private readonly string _imageUrl;

        public SpeedtestQuery(string imageUrl)
        {
            _imageUrl = imageUrl;
        }

        public async Task<double> Execute(HttpClient httpClient)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = await httpClient.GetAsync($"{_imageUrl}?x={Guid.NewGuid().ToString()}");
            stopWatch.Stop();

            int length = int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
            double lengthMbits = length * 8.0 / 1024.0 / 1024.0;
            var downTimeSeconds = stopWatch.ElapsedMilliseconds / 1000.0;
            return lengthMbits / downTimeSeconds;
        }
    }
}
