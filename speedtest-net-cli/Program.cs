using System;
using Autofac;
using log4net.Config;
using SpeedtestNetCli.Configuration;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;

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
            var taskParametersProvider = container.Resolve<ISpeedtestConfigurationProvider>();
            var config = new SpeedtestConfiguration();
            taskParametersProvider.SetConfiguration(config);
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
