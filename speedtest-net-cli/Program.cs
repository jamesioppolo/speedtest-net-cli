using SpeedtestNetCli.Services;

namespace SpeedtestNetCli
{
    class Program
    {
        static void Main(string[] args)
        {
            var retriever = new SpeedtestConfigurationRetriever();
            var doc = retriever.GetConfig().Result;
        }
    }
}
