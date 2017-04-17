using Autofac;
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

            builder.RegisterType<SpeedtestConfigurationRetriever>().AsSelf();
        }
    }
}
