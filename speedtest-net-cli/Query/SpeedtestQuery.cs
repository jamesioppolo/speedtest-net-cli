using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
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

        public async Task<double> Execute(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"{_imageUrl}?x={Guid.NewGuid()}", cancellationToken);
            var length = int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
            var megabitsDownloaded = length * 8.0 / 1000.0 / 1000.0;
            return megabitsDownloaded;
        }
    }
}
