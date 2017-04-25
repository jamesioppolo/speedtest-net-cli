using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using SpeedtestNetCli.Configuration;

namespace SpeedtestNetCli.Infrastructure
{
    public abstract class ThreadedActionService : IService, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Application");

        private readonly object _syncRoot = new object();
        private bool _isRunning;
        private bool _isDisposed;
        public event EventHandler Aborted;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        private readonly ISpeedtestConfigurationProvider _speedtestConfigurationProvider;

        protected ThreadedActionService(ISpeedtestConfigurationProvider speedtestConfigurationProvider)
        {
            _speedtestConfigurationProvider = speedtestConfigurationProvider;
        }

        public void Start()
        {
            lock (_syncRoot)
            {
                CheckDisposed(ToString(), "Start()");
                Log.Debug("Starting...");
                TryRunningTask();
                Log.Debug("Started");
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                CheckDisposed(ToString(), "Stop()");
                Log.Debug("Stopping...");
                TryCancellingTask();
                Log.Debug("Stopped");
            }
        }

        protected void OnAbort(EventArgs e)
        {
            lock (_syncRoot)
            {
                Aborted?.Invoke(this, e);
            }
        }

        protected abstract void Run();

        private void TryRunningTask()
        {
            try
            {
                RunTask();
            }
            catch (Exception e)
            {
                Log.Fatal($"Start failed: {e}");
                CancelTask();
                throw;
            }
        }

        private void RunTask()
        {
            if (_isRunning)
            {
                Log.Fatal("Already running");
                throw new InvalidOperationException("Already running");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _speedtestConfigurationProvider.GetConfiguration().CancellationToken = _cancellationTokenSource.Token;
            _task = new Task(
                Run,
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning);
            _task.ContinueWith(task => OnAbort(new EventArgs()), TaskContinuationOptions.NotOnCanceled);
            _task.Start();
            _isRunning = true;
        }

        private void TryCancellingTask()
        {
            try
            {
                CancelTask();
            }
            catch (Exception e)
            {
                Log.Fatal($"Stop failed: {e}");
                throw;
            }
        }

        private void CancelTask()
        {
            if (!_isRunning)
            {
                Log.Fatal("Not running");
                return;
            }

            if (_cancellationTokenSource != null)
            {
                TryCancellingToken();

                if (_task != null)
                {
                    TryWaitForTask();
                    _task.Dispose();
                    _task = null;
                }

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            _isRunning = false;
        }

        private void TryWaitForTask()
        {
            try
            {
                _task.Wait();
            }
            catch (AggregateException ae)
            {
                LogInnerExceptions(ae);
            }
        }

        private void TryCancellingToken()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (AggregateException ae)
            {
                LogInnerExceptions(ae);
            }
        }

        private void LogInnerExceptions(AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                Log.Fatal($"Caught exception: {e}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedObjects)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposeManagedObjects)
            {
                if (_isRunning)
                {
                    Stop();
                }
            }
            _isDisposed = true;
        }

        protected void CheckDisposed(string className, string method)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(className, $"Object already disposed in: {method}");
            }
        }
    }

}
