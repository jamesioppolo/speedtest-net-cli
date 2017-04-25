using System;
using System.ServiceProcess;

namespace SpeedtestNetCli.Infrastructure
{
    public class ActionService : ServiceBase
    {
        public Action StartAction { get; set; }
        public Action StopAction { get; set; }

        public ActionService()
        {
            AutoLog = true;
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            StartAction?.Invoke();
        }

        protected override void OnStop()
        {
            StopAction?.Invoke();
        }
    }
}
