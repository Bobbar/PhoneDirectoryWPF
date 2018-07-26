using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.Data;
using PhoneDirectoryWPF.Containers;
using System.IO;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UserSettings
    {
        private static Dictionary<string, object> settingsRepo = new Dictionary<string, object>();

        private static void GetFromDisk()
        {
            try
            {
                settingsRepo.Clear();

                var repoList = (List<Setting>)Serializer.DeSerializeObject(typeof(List<Setting>), Paths.Settings);

                foreach (var item in repoList)
                {
                    settingsRepo.Add(item.Name, item.Value);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // No settings file yet. Fail silently.
            }
            catch (System.InvalidOperationException)
            {
                // Probably a malformed settings file.
                // Delete it an fail silently.
                File.Delete(Paths.Settings);
            }
        }

        private static void WriteToDisk()
        {
            var repoList = new List<Setting>();

            foreach (var item in settingsRepo)
            {
                repoList.Add(new Setting(item.Key, item.Value));
            }

            Serializer.SerializeObject(typeof(List<Setting>), repoList, Paths.Settings);
        }

        public static object GetSetting(AppSettings setting)
        {
            GetFromDisk();

            if (settingsRepo.ContainsKey(setting.ToString()))
                return settingsRepo[setting.ToString()];

            return null;
        }

        public static void SaveSetting(AppSettings setting, object value)
        {
            GetFromDisk();

            if (!settingsRepo.ContainsKey(setting.ToString()))
            {
                settingsRepo.Add(setting.ToString(), value);
                WriteToDisk();
            }
            else
            {
                if (settingsRepo[setting.ToString()] != value)
                {
                    settingsRepo[setting.ToString()] = value;
                    WriteToDisk();
                }
            }
        }
    }

    public enum AppSettings
    {
        UIScale,
        UIDark,
        UITheme
    }
}
