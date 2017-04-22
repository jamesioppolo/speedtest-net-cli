using Autofac;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;

namespace SpeedtestNetCli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IocBuilder.Build().Resolve<Speedtester>().Execute();
        }
    }
}
