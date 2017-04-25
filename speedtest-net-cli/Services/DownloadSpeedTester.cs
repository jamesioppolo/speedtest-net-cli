using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpeedtestNetCli.Query;

namespace SpeedtestNetCli.Services
{
    public interface IDownloadSpeedTester
    {
        double GetSpeedMbps(XElement server);
    }

    public class DownloadSpeedTester : IDownloadSpeedTester
    {
        private readonly Func<IHttpQueryExecutor> _httpQueryExecutor;

        public DownloadSpeedTester(Func<IHttpQueryExecutor> httpQueryExecutor)
        {
            _httpQueryExecutor = httpQueryExecutor;
        }

        public double GetSpeedMbps(XElement server)
        {
            var imageUrls = GetImageUrls(server);
            var tasks = imageUrls.Select(url => _httpQueryExecutor().Execute(new SpeedtestQuery(url))).ToList();

            var stopwatch = Stopwatch.StartNew();
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            var totalMegabitsDownloaded = tasks.Where(x => x.Status == TaskStatus.RanToCompletion).Sum(x => x.Result);
            return totalMegabitsDownloaded / (stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private static IEnumerable<string> GetImageUrls(XElement bestServer)
        {
            var imageSizes = new List<string> {"350", "500", "750", "1000", "1500"};// "2000", "2500", "3000" };

            var imageUrls = new List<string>();
            foreach (var imageSize in imageSizes)
            {
                var imageUrl = bestServer.Attribute("url").Value.Replace("upload.php", $"random{imageSize}x{imageSize}.jpg");
                imageUrls.AddRange(Enumerable.Repeat(imageUrl, 4));
            }
            return imageUrls;
        }
    }
}
