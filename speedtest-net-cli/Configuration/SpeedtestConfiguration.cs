using System.Threading;
using CommandLine;

namespace SpeedtestNetCli.Configuration
{
    public class SpeedtestConfiguration
    {
        [Option('l', "List", Required = false, HelpText = "Lists the closest 20 speedtest servers")]
        public bool List { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
