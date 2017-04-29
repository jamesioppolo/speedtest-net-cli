using System.Linq.Expressions;
using System.Threading;
using CommandLine;
using CommandLine.Text;

namespace SpeedtestNetCli.Configuration
{
    public class SpeedtestConfiguration
    {
        public bool HelpRequested;

        [Option('i', "Interval", Required = false, HelpText = "Interval in minutes between successive speed tests (Default = 12 minutes)")]
        public int IntervalMinutes { get; set; } = 12;

        [Option('l', "List", Required = false, HelpText = "Lists the closest 20 speedtest servers")]
        public bool List { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpRequested = true;
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public CancellationToken CancellationToken { get; set; }
    }
}
