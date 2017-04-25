using System;
using System.Threading;
using log4net;

namespace SpeedtestNetCli.Services
{
    public interface ISpeedtester
    {
        void Execute();
    }

    public class Speedtester : ISpeedtester
    {
        private static readonly ILog Log = LogManager.GetLogger(string.Empty);

        private readonly IBestServerDeterminer _bestServerDeterminer;
        private readonly IDownloadSpeedTester _downloadSpeedTester;
        private readonly IUploadSpeedTester _uploadSpeedTester;

        public Speedtester(
            IBestServerDeterminer bestServerDeterminer,
            IDownloadSpeedTester downloadSpeedTester,
            IUploadSpeedTester uploadSpeedTester)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _downloadSpeedTester = downloadSpeedTester;
            _uploadSpeedTester = uploadSpeedTester;
        }

        public void Execute()
        {
            while (true)
            {
                PerformSingleSpeedTest();
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }

        private void PerformSingleSpeedTest()
        {
            var bestServer = _bestServerDeterminer.GetBestServer().Result;
            var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
            var upSpeedMbps = _uploadSpeedTester.GetSpeedMbps(bestServer);

            var latency = Convert.ToDouble(bestServer.Attribute("latency").Value);
            var server = bestServer.Attribute("host").Value;

            Log.Info($"{latency:N2} {downSpeedMbps:N2} {upSpeedMbps:N2} {server}");
        }
    }
}
