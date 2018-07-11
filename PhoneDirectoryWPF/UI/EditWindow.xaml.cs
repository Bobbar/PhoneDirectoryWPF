using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;
using System;
using System.Windows;
using System.Windows.Input;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        // Reference to the extension from the main window.
        // This is updated with new values after a successful DB update.
        // The changes will then be reflected on the main window.
        private Extension extensionContext;

        public EditWindow()
        {
            InitializeComponent();
            extensionContext = new Extension();
            this.DataContext = extensionContext;
            saveButton.Visibility = Visibility.Collapsed;
        }

        public EditWindow(Extension extension)
        {
            InitializeComponent();
            addButton.Visibility = Visibility.Collapsed;

            this.extensionContext = extension;
            // Set this context to a copy from the database.
            this.DataContext = extension.FromDatabase();
        }

        private void UpdateExtension()
        {
            // TODO: Field verification.

            var ctx = (Extension)this.DataContext;
            ctx.Update();
            // Copy new values to the original context to update the main window values.
            this.extensionContext.CopyValues(ctx);
            this.DialogResult = true;
            this.Close();
        }

        private void AddExtension()
        {
            // TODO: Field verification.

            try
            {
                extensionContext.Insert();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                switch ((MySql.Data.MySqlClient.MySqlErrorCode)ex.Number)
                {
                    case MySql.Data.MySqlClient.MySqlErrorCode.DuplicateKeyEntry:
                        var prompt = string.Format("An extension with the value ({0}) already exists in the database.", ((Extension)DataContext).Number);
                        UserPrompts.PopupMessage(prompt, "Duplicates Not Allowed");
                        break;

                    case MySql.Data.MySqlClient.MySqlErrorCode.NoDefaultForField:
                        UserPrompts.PopupMessage(ex.Message, "Required Field Empty");
                        break;
                }

                // UserPrompts.PopupMessage(ex.ToString());
                Console.WriteLine(ex.Number + "  " + ex.ToString());
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateExtension();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddExtension();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void firstNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine(extensionContext.ToString());
        }
    }
}