using System;
using System.Threading.Tasks;
using System.Xml.Linq;
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
        private readonly ISpeedtestConfigurationProvider _configurationProvider;

        public SpeedtestService(
            IBestServerDeterminer bestServerDeterminer,
            IDownloadSpeedTester downloadSpeedTester,
            IUploadSpeedTester uploadSpeedTester,
            ISpeedtestConfigurationProvider configurationProvider)
            : base(configurationProvider)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _downloadSpeedTester = downloadSpeedTester;
            _uploadSpeedTester = uploadSpeedTester;
            _configurationProvider = configurationProvider;
        }

        protected override void Run()
        {
            while (true)
            {
                var bestServer = _bestServerDeterminer.GetBestServer().Result;
                var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
                var upSpeedMbps = _uploadSpeedTester.GetSpeedMbps(bestServer);

                var latency = Convert.ToDouble(bestServer.Attribute("latency").Value);
                var server = bestServer.Attribute("host").Value;

                Log.Info($"{latency:N2} {downSpeedMbps:N2} {upSpeedMbps:N2} {server}");
                _configurationProvider.GetConfiguration().CancellationToken.WaitHandle.WaitOne(TimeSpan.FromMinutes(5));
            }
        }
        
    }
}
