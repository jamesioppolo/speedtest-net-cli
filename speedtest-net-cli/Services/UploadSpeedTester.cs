using System;
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
            return 0;
        }
    }
}
