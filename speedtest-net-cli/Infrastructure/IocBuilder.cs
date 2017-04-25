using Autofac;
using SpeedtestNetCli.Configuration;
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

            builder.RegisterType<SpeedtestService>().AsSelf();
            builder.RegisterType<SpeedtestConfigurationProvider>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<HttpQueryExecutor>().AsImplementedInterfaces();
            builder.RegisterType<BestServerDeterminer>().AsImplementedInterfaces();
            builder.RegisterType<DownloadSpeedTester>().AsImplementedInterfaces();
            builder.RegisterType<UploadSpeedTester>().AsImplementedInterfaces();

        }
    }
}
