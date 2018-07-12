using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.UI;
using System.Windows;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UserPrompts
    {
        public static void PopupMessage(string message, string header)
        {
            // We find the active window then search it for a dialoghost to display the popup.

            // Find the current active window.
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
          
            // Search the window for a dialog host by name.
            // Searching by type would work but we may have multiple dialog hosts per window in the future.
            var popHostControl = ControlHelper.FindControlByName(activeWindow, "PopupDialogHost");

            if (popHostControl == null)
                return;
            
            // Cast the dialog host and access the identifier.
            var popHost = (MaterialDesignThemes.Wpf.DialogHost)popHostControl;

            MaterialDesignThemes.Wpf.DialogHost.Show(new PopupDialog(message, header), popHost.Identifier);

        }
    }
}
