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
            var retriever = container.Resolve<SpeedtestConfigurationRetriever>();
            var doc = retriever.GetConfig().Result;
        }
    }
}
