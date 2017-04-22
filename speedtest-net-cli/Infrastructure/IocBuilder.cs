using Autofac;
using SpeedtestNetCli.Query;
using SpeedtestNetCli.Services;

namespace SpeedtestNetCli.Infrastructure
{
    public static class IocBuilder
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<SpeedtestModule>();
            return builder.Build();
        }
    }

    public class SpeedtestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Speedtester>().AsSelf();
            builder.RegisterType<HttpQueryExecutor>().AsImplementedInterfaces();
            builder.RegisterType<BestServerDeterminer>().AsImplementedInterfaces();
            builder.RegisterType<DownloadSpeedTester>().AsImplementedInterfaces();

        }
    }
}
