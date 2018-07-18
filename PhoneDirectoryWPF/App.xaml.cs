﻿using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using ShowMeTheXAML;
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

            XamlDisplay.Init();
            base.OnStartup(e);
        }

       
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is InvalidAccessException)
            {
                var iae = (InvalidAccessException)e.Exception;
                UserPrompts.PopupMessage(iae.Message, "Access Denied");
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
                UserPrompts.PopupMessage(e.Exception.ToString(), "UNHANDLED ERROR!");
                e.Handled = false;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            WatchDogInstance.WatchDog.Dispose();
        }
    }
}