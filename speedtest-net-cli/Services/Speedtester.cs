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
            var bestServer = _bestServerDeterminer.GetBestServer().Result;
            var downSpeedMbps = _downloadSpeedTester.GetSpeedMbps(bestServer);
            var upSpeedMbps = _uploadSpeedTester.GetSpeedMbps(bestServer);
        }
    }
}
