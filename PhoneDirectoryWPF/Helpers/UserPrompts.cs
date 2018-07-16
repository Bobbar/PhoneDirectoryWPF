using MaterialDesignThemes.Wpf;
using PhoneDirectoryWPF.UI;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UserPrompts
    {
        public async static Task PopupMessage(string message, string header)
        {
            var popHost = FindActiveDialogHost();

            if (popHost == null)
                return;

            await DialogHost.Show(new PopupDialog(message, header), popHost.Identifier);
        }

        public async static Task<object> PopupDialog(string message, string header, DialogButtons buttons)
        {
            var popHost = FindActiveDialogHost();

            if (popHost == null)
                return null;

            return await DialogHost.Show(new PopupDialog(message, header, buttons), popHost.Identifier);
        }

        public async static Task<SpinnerDialog> SpinnerPopup()
        {
            var popHost = FindActiveDialogHost();

            if (popHost == null)
                return null;

            var spinner = new SpinnerDialog(popHost);

            DialogHost.Show(spinner, popHost.Identifier);

            return spinner;
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

            // Cast the dialog host and return it.
            return (DialogHost)popHostControl;
        }
    }
}