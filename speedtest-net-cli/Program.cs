using Autofac;
using log4net.Config;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;

namespace SpeedtestNetCli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            IocBuilder.Build().Resolve<Speedtester>().Execute();
        }
    }
}
