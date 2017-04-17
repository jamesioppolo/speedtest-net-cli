using Autofac;
using SpeedtestNetCli.Infrastructure;
using SpeedtestNetCli.Services;

namespace SpeedtestNetCli
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = IocBuilder.Build();
            var speedtester = container.Resolve<Speedtester>();
            speedtester.Execute();
        }
    }
}
