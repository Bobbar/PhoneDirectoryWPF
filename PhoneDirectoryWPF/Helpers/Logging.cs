using System;
using System.IO;

namespace PhoneDirectoryWPF.Helpers
{
    public static class Logging
    {
        private static string logFullPath = Paths.Log + "log.log";

        public static void Logger(string message)
        {
            try
            {
                int maxLogSizeKiloBytes = 500;
                string dateStamp = DateTime.Now.ToString();

                if (!Directory.Exists(Paths.Log))
                    Directory.CreateDirectory(Paths.Log);

                if (!File.Exists(logFullPath))
                {
                    using (StreamWriter sw = File.CreateText(logFullPath))
                    {
                        sw.WriteLine(dateStamp + ": Log Created...");
                        sw.WriteLine(dateStamp + ": " + message);
                    }
                }
                else
                {
                    var infoReader = new FileInfo(logFullPath);

                    if ((infoReader.Length / 1000) < maxLogSizeKiloBytes)
                    {
                        using (StreamWriter sw = File.AppendText(logFullPath))
                        {
                            sw.WriteLine(dateStamp + ": " + message);
                        }
                    }
                    else
                    {
                        if (RotateLogs())
                        {
                            using (StreamWriter sw = File.AppendText(logFullPath))
                            {
                                sw.WriteLine(dateStamp + ": " + message);
                            }
                        }
                    }
                }
            }
            catch
            {
                //Shhhh.
            }
        }

        public static void Exception(Exception ex)
        {
            Logger("ERROR: Type: " + ex.GetType().Name + "  #:" + ex.HResult + "  Message:" + ex.Message);
            Logger("STACK TRACE: " + ex.StackTrace);
        }

        private static bool RotateLogs()
        {
            try
            {
                File.Copy(logFullPath, logFullPath + ".old", true);
                File.Delete(logFullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}