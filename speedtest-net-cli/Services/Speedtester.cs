using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpeedtestNetCli.Query;

namespace SpeedtestNetCli.Services
{
    public interface ISpeedtester
    {
        void Execute();
    }

    public class Speedtester : ISpeedtester
    {
        private readonly IBestServerDeterminer _bestServerDeterminer;
        private readonly Func<IHttpQueryExecutor> _httpQueryExecutor;

        public Speedtester(
            IBestServerDeterminer bestServerDeterminer,
            Func<IHttpQueryExecutor> httpQueryExecutor)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _httpQueryExecutor = httpQueryExecutor;
        }

        public void Execute()
        {
            var bestServer = _bestServerDeterminer.GetBestServer().Result;
            var downSpeedMbps = CalculateDownloadSpeed(bestServer);
        }

        private double CalculateDownloadSpeed(XElement bestServer)
        {
            var imageSizes = new List<string> { "350", "500", "750", "1000", "1500", "2000", "2500", "3000" };

            var imageUrls = new List<string>();
            foreach (var imageSize in imageSizes)
            {
                var imageUrl = bestServer.Attribute("url").Value.Replace("upload.php", $"random{imageSize}x{imageSize}.jpg");
                imageUrls.AddRange(Enumerable.Repeat(imageUrl, 4));
            }

            var tasks = imageUrls.Select(url => _httpQueryExecutor().Execute(new SpeedtestQuery(url))).ToList();

            var stopwatch = Stopwatch.StartNew();
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            var totalMegabitsDownloaded = tasks.Where(x => x.Status == TaskStatus.RanToCompletion).Sum(x => x.Result);
            return totalMegabitsDownloaded / (stopwatch.ElapsedMilliseconds / 1000.0);
        }
    }
}
