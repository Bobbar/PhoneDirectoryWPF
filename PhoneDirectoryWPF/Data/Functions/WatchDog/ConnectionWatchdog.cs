using System;
using System.Threading.Tasks;
using System.Threading;

namespace PhoneDirectoryWPF.Data.Functions
{
    public class ConnectionWatchdog : IDisposable
    {
        public event EventHandler<bool> CacheStatusChanged;

        private ManualResetEvent loopWaitHandle = new ManualResetEvent(false);
        private Task watchdogTask;
        private bool isInCacheMode = false;
        private const int watchdogInterval = 5000;
        private const int cyclesTillRefresh = 30;
        private int cycles = 0;

        private bool IsInCacheMode
        {
            get
            {
                return isInCacheMode;
            }

            set
            {
                if (isInCacheMode != value)
                {
                    isInCacheMode = value;
                    OnCacheStatusChanged(isInCacheMode);
                }
            }
        }

        public ConnectionWatchdog()
        {
            watchdogTask = new Task(() => DoWatchdogLoop(), TaskCreationOptions.LongRunning);
        }

        public void Start(bool cacheMode)
        {
            IsInCacheMode = cacheMode;
            OnCacheStatusChanged(cacheMode);
            watchdogTask.Start();
        }

        private void OnCacheStatusChanged(bool cacheMode)
        {
            if (loopWaitHandle.WaitOne(0))
                return;

            CacheStatusChanged?.Invoke(this, cacheMode);
        }

        private void DoWatchdogLoop()
        {
            while (!loopWaitHandle.WaitOne(0))
            {
                try
                {
                    var serverTime = DBFactory.GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT NOW()").ToString();

                    if (string.IsNullOrEmpty(serverTime))
                    {
                        IsInCacheMode = true;
                    }
                    else
                    {
                        IsInCacheMode = false;
                    }
                }
                catch
                {
                    IsInCacheMode = true;
                }

                // Periodically refresh the sqlite cache.
                if (cycles >= cyclesTillRefresh)
                {
                    cycles = 0;
                    CacheFunctions.RefreshCache();
                }

                cycles++;

                loopWaitHandle.WaitOne(watchdogInterval);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    loopWaitHandle.Set();
                    watchdogTask.Wait();
                    watchdogTask.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}