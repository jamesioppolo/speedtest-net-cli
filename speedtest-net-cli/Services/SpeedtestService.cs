using System;
using log4net;
using SpeedtestNetCli.Configuration;
using SpeedtestNetCli.Infrastructure;

namespace SpeedtestNetCli.Services
{
    public interface ISpeedtestService
    {
    }

    public class SpeedtestService : ThreadedActionService, ISpeedtestService
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Service");

        private readonly IBestServerDeterminer _bestServerDeterminer;
        private readonly IDownloadSpeedTester _downloadSpeedTester;
        private readonly IUploadSpeedTester _uploadSpeedTester;
        private readonly SpeedtestConfiguration _speedtestConfiguration;

        public SpeedtestService(
            IBestServerDeterminer bestServerDeterminer,
            IDownloadSpeedTester downloadSpeedTester,
            IUploadSpeedTester uploadSpeedTester,
            SpeedtestConfiguration speedtestConfiguration)
            : base(speedtestConfiguration)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _downloadSpeedTester = downloadSpeedTester;
            _uploadSpeedTester = uploadSpeedTester;
            _speedtestConfiguration = speedtestConfiguration;
        }

        protected override void Run()
        {
            if (_speedtestConfiguration.List)
            {
                Log.Info("Please wait for retrieval of closest 20 servers...");
                foreach (var server in _bestServerDeterminer.GetClosestServers(20).Result)
                {
                    Log.Info($"Distance={Convert.ToDouble(server.Attribute("clientDistance").Value):N2}km, " +
                             $"{server.Attribute("host").Value} " +
                             $"(ID={server.Attribute("id").Value})");
                }
                return;
            }

            while (!_speedtestConfiguration.CancellationToken.IsCancellationRequested)
            {
                TryRunSpeedTest();
                _speedtestConfiguration.CancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(12));
            }
        }

        private void TryRunSpeedTest()
        {
            try
            {
                var bestServer = _bestServerDeterminer.GetBestServer().Result;
                var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
                var upSpeedMbps = _uploadSpeedTester.GetSpeedMbps(bestServer);

                var latency = Convert.ToDouble(bestServer.Attribute("latency").Value);
                var server = bestServer.Attribute("host").Value;

                Log.Info($"{latency:N2} {downSpeedMbps:N2} {upSpeedMbps:N2} {server}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
}
