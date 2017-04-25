using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpeedtestNetCli.Query
{
    public class SpeedtestUploadQuery : IHttpQuery<bool>
    {
        private readonly string _url;
        private readonly string _payload;

        public SpeedtestUploadQuery(string url, string payload)
        {
            _url = url;
            _payload = payload;
        }

        public async Task<bool> Execute(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsync($"{_url}?x={Guid.NewGuid()}", new StringContent(_payload), cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
