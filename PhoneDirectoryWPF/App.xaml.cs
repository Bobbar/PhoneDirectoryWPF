using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using ShowMeTheXAML;
using System.Windows;

namespace PhoneDirectoryWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Init();
            XamlDisplay.Init();
            base.OnStartup(e);
        }

        private void Init()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            SecurityFunctions.PopulateUserAccess();
            SecurityFunctions.PopulateAccessGroups();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is InvalidAccessException)
            {
                var iae = (InvalidAccessException)e.Exception;
                UserPrompts.PopupMessage(iae.Message, "Access Denied");
                e.Handled = true;
            }
            else
            {
                UserPrompts.PopupMessage(e.Exception.ToString(), "UNHANDLED ERROR!");
                e.Handled = false;
            }
        }
    }
}