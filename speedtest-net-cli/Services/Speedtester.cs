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

            var imageUrl0 = bestServer.Attribute("url").Value.Replace("upload.php", "random750x750.jpg");
            var downSpeed0 = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl0)).Result;

            var imageUrl1 = bestServer.Attribute("url").Value.Replace("upload.php", "random1000x1000.jpg");
            var downSpeed1 = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl1)).Result;

            var imageUrl2 = bestServer.Attribute("url").Value.Replace("upload.php", "random1500x1500.jpg");
            var downSpeed2 = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl2)).Result;

            var imageUrl3 = bestServer.Attribute("url").Value.Replace("upload.php", "random2000x2000.jpg");
            var downSpeed3 = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl3)).Result;

            var imageUrl4 = bestServer.Attribute("url").Value.Replace("upload.php", "random2500x2500.jpg");
            var downSpeed4 = _httpQueryExecutor().Execute(new SpeedtestQuery(imageUrl4)).Result;
        }
    }
}
