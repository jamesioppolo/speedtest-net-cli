using Autofac;
using SpeedtestNetCli.Command;
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
            builder.RegisterType<SpeedtestConfigurationRetriever>().AsImplementedInterfaces();
            builder.RegisterType<SpeedtestServerRetriever>().AsImplementedInterfaces();
            builder.RegisterType<HttpQueryExecutor>().AsImplementedInterfaces();
        }
    }
}
