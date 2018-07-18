using MaterialDesignThemes.Wpf;
using PhoneDirectoryWPF.UI;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Collections.Generic;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UserPrompts
    {
        public async static Task PopupMessage(string message, string header)
        {
            var popHost = FindActiveDialogHost();

            if (popHost == null)
                return;

            DialogHost.CloseDialogCommand.Execute(new object(), null);

            await DialogHost.Show(new PopupDialog(message, header), popHost.Identifier);
        }

        public async static Task PopupMessage(Window window, string message, string header)
        {
            DialogHost.CloseDialogCommand.Execute(new object(), null);

            await DialogHostEx.ShowDialog(window, new PopupDialog(message, header));
        }

        public async static Task<object> PopupDialog(Window window, string message, string header, DialogButtons buttons)
        {
            DialogHost.CloseDialogCommand.Execute(new object(), null);

            return await DialogHostEx.ShowDialog(window, new PopupDialog(message, header, buttons));
        }

        private static DialogHost FindActiveDialogHost()
        {
            // We find the active window then search it for a dialoghost to display the popup.

            // Find the current active window.
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            // Search the window for a dialog host by name.
            // Searching by type would work but we may have multiple dialog hosts per window in the future.
            var popHostControl = ControlHelper.FindControlByName(activeWindow, "PopupDialogHost");

            if (popHostControl == null)
                return null;

            var popHostDialog = (DialogHost)popHostControl;

            return popHostDialog;
        }
    }
}