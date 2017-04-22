using System;
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
            var imageUrl = bestServer.Attribute("url").Value.Replace("upload.php", "random2000x2000.jpg");
            var downSpeed = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl)).Result;
        }
    }
}
