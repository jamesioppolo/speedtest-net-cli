using System;
using System.Linq;
using Autofac;
using log4net.Config;
using SpeedtestNetCli.Configuration;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;
using CommandLine;

namespace SpeedtestNetCli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var container = IocBuilder.Build();
            SetupCommandLineOptions(args, container);
            StartSpeedtest(container);
        }

        private static void SetupCommandLineOptions(string[] args, IComponentContext container)
        {
            var config = container.Resolve<SpeedtestConfiguration>();
            if (args.Any())
                Parser.Default.ParseArguments(args, config);
        }

        private static void StartSpeedtest(IComponentContext container)
        { 
            var speedtestService = container.Resolve<SpeedtestService>();
            using (var runner = new ActionServiceRunner(speedtestService))
            {
                Environment.Exit(runner.Run());
            }
        }
    }
}
