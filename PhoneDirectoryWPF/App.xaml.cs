using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using System.Windows;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Data;
using System;

namespace PhoneDirectoryWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);
        }

        private async void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is InvalidAccessException)
            {
                var iae = (InvalidAccessException)e.Exception;
                await UserPrompts.PopupMessage(iae.Message, "Access Denied");
                e.Handled = true;
            }
            else if (e.Exception is MySql.Data.MySqlClient.MySqlException)
            {
                var mse = (MySql.Data.MySqlClient.MySqlException)e.Exception;

                switch ((MySql.Data.MySqlClient.MySqlErrorCode)mse.Number)
                {
                    case MySql.Data.MySqlClient.MySqlErrorCode.UnableToConnectToHost:
                        DBFactory.CacheMode = true;
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                Logging.Exception(e.Exception);
                await UserPrompts.PopupMessage(e.Exception.ToString(), "UNHANDLED ERROR!");
                e.Handled = true;
                Application.Current.Shutdown(-1);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            WatchDogInstance.WatchDog.Dispose();
        }
    }
}