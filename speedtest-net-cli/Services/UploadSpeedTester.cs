using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace SpeedtestNetCli.Services
{
    public interface IUploadSpeedTester
    {
        double GetSpeedMbps(XElement server);
    }

    public class UploadSpeedTester : IUploadSpeedTester
    {
        public double GetSpeedMbps(XElement server)
        {
            var uploadSizes = new List<int> { 32768, 65536, 131072, 262144, 524288, 1048576, 7340032 };
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var payload = "content1=";
            var numIterations = 524288 / chars.Length;
            for (var iter = 0; iter < numIterations; iter++)
            {
                payload += chars;
            }
            var bytes = Encoding.ASCII.GetBytes(payload);

            return 0;
        }
    }
}
