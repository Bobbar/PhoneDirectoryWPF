using System;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data.Functions
{
    public class ConnectionWatchdog : IDisposable
    {
        public event EventHandler<bool> CacheStatusChanged;

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
            CacheStatusChanged?.Invoke(this, cacheMode);
        }

        private void DoWatchdogLoop()
        {
            while (!disposedValue)
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

                Task.Delay(watchdogInterval).Wait();
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
                  
                   // watchdogTask.Dispose();
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