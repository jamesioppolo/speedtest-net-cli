namespace SpeedtestNetCli.Configuration
{
    public interface ISpeedtestConfigurationProvider
    {
        void SetConfiguration(SpeedtestConfiguration speedtestConfiguration);
        SpeedtestConfiguration GetConfiguration();
    }

    public class SpeedtestConfigurationProvider : ISpeedtestConfigurationProvider
    {
        private SpeedtestConfiguration _speedtestConfiguration;

        public void SetConfiguration(SpeedtestConfiguration speedtestConfiguration)
        {
            _speedtestConfiguration = speedtestConfiguration;
        }

        public SpeedtestConfiguration GetConfiguration()
        {
            return _speedtestConfiguration;
        }
    }
}
