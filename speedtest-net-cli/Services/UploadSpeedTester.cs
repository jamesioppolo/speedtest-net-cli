using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpeedtestNetCli.Query;

namespace SpeedtestNetCli.Services
{
    public interface IUploadSpeedTester
    {
        double GetSpeedMbps(XElement server);
    }

    public class UploadSpeedTester : IUploadSpeedTester
    {
        private readonly Func<IHttpQueryExecutor> _httpQueryExecutor;

        public UploadSpeedTester(Func<IHttpQueryExecutor> httpQueryExecutor)
        {
            _httpQueryExecutor = httpQueryExecutor;
        }

        public double GetSpeedMbps(XElement server)
        {
            var payload = GetUploadTestPayload();

            var numUploadThreads = 50;
            var tasks = new List<Task>();
            for (var task = 0; task < numUploadThreads; task++)
                tasks.Add(_httpQueryExecutor().Execute(new SpeedtestUploadQuery(server.Attribute("url").Value, payload)));

            var stopwatch = Stopwatch.StartNew();
            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();

            var totalMegabitsUploaded = numUploadThreads * 8 * 524288 / 1000.0 / 1000.0;
            var elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
            var upspeedMbps = totalMegabitsUploaded / elapsedSeconds;
            return upspeedMbps;
        }

        private static string GetUploadTestPayload()
        {
            var uploadSizes = new List<int> {32768, 65536, 131072, 262144, 524288, 1048576, 7340032};
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var payload = "content1=";
            var numIterations = 524288 / chars.Length;
            for (var iter = 0; iter < numIterations; iter++)
            {
                payload += chars;
            }
            return payload;
        }
    }
}
