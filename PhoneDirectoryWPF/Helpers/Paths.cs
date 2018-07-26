using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryWPF.Helpers
{
    public static class Paths
    {
        public static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\PhoneDirectory";
        public static readonly string Log = AppData + @"\logs\";
        public static readonly string Settings = AppData + @"\settings.xml";
    }
}
