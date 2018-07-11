using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.UI;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UserPrompts
    {
        public static void PopupMessage(string message)
        {
            MaterialDesignThemes.Wpf.DialogHost.Show(new PopupDialog(message));
        }

        public static void PopupMessage(string message, string header)
        {
            MaterialDesignThemes.Wpf.DialogHost.Show(new PopupDialog(message, header));
        }
    }
}
