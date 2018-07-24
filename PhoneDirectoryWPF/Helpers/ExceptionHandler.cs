using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.UI;

namespace PhoneDirectoryWPF.Helpers
{
    public static class ExceptionHandler
    {
        public static async void MySqlException(Window parent, MySql.Data.MySqlClient.MySqlException ex)
        {
            Logging.Exception(ex);

            switch ((MySql.Data.MySqlClient.MySqlErrorCode)ex.Number)
            {
                case MySql.Data.MySqlClient.MySqlErrorCode.DuplicateKeyEntry:
                    var prompt = string.Format("An extension with the value '{0}' already exists in the database.", ((Extension)parent.DataContext).Number);
                    UserPrompts.PopupMessage(parent, prompt, "Duplicates Not Allowed");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.NoDefaultForField:
                    UserPrompts.PopupMessage(parent, ex.Message, "Required Field Empty");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.DataTooLong:
                    UserPrompts.PopupMessage(parent, ex.Message, "Data Too Long");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.UnableToConnectToHost:
                    await UserPrompts.PopupDialog(parent, "Could not connect to the server.", "Connection Lost", DialogButtons.Default);
                    parent.Close();
                    break;

                default:
                    UserPrompts.PopupMessage(parent, ex.Message, "Unexpected Database Error!");
                    break;
            }
        }
    }
}
