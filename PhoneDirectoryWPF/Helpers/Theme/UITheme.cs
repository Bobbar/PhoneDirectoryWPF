using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UITheme
    {
        public static void SetDark(bool isDark)
        {
            var pal = new PaletteHelper();
            pal.SetLightDark(isDark);

            UserSettings.SaveSetting(AppSettings.UIDark, isDark);
        }

    }
}
