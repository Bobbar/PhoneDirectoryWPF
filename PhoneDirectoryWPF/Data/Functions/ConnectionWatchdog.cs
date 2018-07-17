using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Data;

namespace PhoneDirectoryWPF.Data.Functions
{
    public class ConnectionWatchdog : IDisposable
    {
        //public static event EventHandler CacheStatusChanged;

        //public void Start()
        //{

        //}

        private async Task DoWatchdogLoop()
        {
            while (!disposedValue)
            {

                try
                {
                    var serverTime = DBFactory.GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT NOW()").ToString();

                }
                catch
                {

                }



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
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
