using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Data.Functions
{
    public static class WatchDogInstance
    {
        public static ConnectionWatchdog WatchDog = new ConnectionWatchdog();
    }
}
