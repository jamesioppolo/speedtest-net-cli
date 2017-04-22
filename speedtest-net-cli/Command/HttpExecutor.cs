﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpeedtestNetCli.Command
{ 
    public interface IHttpQuery<T>
    {
        Task<T> Execute(HttpClient client);
    }

    public interface IHttpQueryExecutor
    {
        Task<T> Execute<T>(IHttpQuery<T> query);
    }

    public class HttpQueryExecutor : IHttpQueryExecutor
    {
        public async Task<T> Execute<T>(IHttpQuery<T> query)
        {
            var httpHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            using (var httpClient = new HttpClient(httpHandler))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, sdch");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.8");

                httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue();
                httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;

                return await query.Execute(httpClient);
            }
        }
    }
}
