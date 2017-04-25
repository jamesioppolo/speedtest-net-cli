using System;

namespace SpeedtestNetCli.Infrastructure
{
    public interface IService
    {
        void Start();
        void Stop();
        event EventHandler Aborted;
    }
}