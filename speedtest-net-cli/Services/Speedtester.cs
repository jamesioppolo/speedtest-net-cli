namespace SpeedtestNetCli.Services
{
    public interface ISpeedtester
    {
        void Execute();
    }

    public class Speedtester : ISpeedtester
    {
        private readonly IBestServerDeterminer _bestServerDeterminer;
        private readonly IDownloadSpeedTester _downloadSpeedTester;

        public Speedtester(
            IBestServerDeterminer bestServerDeterminer,
            IDownloadSpeedTester downloadSpeedTester)
        {
            _bestServerDeterminer = bestServerDeterminer;
            _downloadSpeedTester = downloadSpeedTester;
        }

        public void Execute()
        {
            var bestServer = _bestServerDeterminer.GetBestServer().Result;
            var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
        }
    }
}
