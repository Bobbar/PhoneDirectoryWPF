using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;
using System;
using System.ComponentModel;
using System.Windows;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window, INotifyPropertyChanged
    {
        // Reference to the extension from the main window.
        // This is updated with new values after a successful DB update.
        // The changes will then be reflected on the main window.
        private Extension extensionContext;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool inputEnabled = true;

        public bool InputEnabled
        {
            get
            {
                return inputEnabled;
            }

            set
            {
                inputEnabled = value;
                OnPropertyChanged(nameof(InputEnabled));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EditWindow()
        {
            InitializeComponent();
            saveButton.Visibility = Visibility.Collapsed;

            this.Title = "New";
            this.FieldGroupBox.Header = "Add New Extension";

            extensionContext = new Extension();
            this.DataContext = extensionContext;
        }

        public EditWindow(Extension extension)
        {
            InitializeComponent();
            addButton.Visibility = Visibility.Collapsed;

            this.Title = "Edit";
            this.FieldGroupBox.Header = "Edit Extension";

            this.extensionContext = extension;
            // Set this context to a copy from the database.
            this.DataContext = extension.FromDatabase();
        }

        private void UpdateExtension()
        {
            // TODO: Field verification.

            var ctx = (Extension)this.DataContext;

            try
            {
                ctx.Update();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                HandleSqlException(ex);

                Console.WriteLine(ex.Number + "  " + ex.ToString());
                return;
            }

            // Copy new values to the original context to update the main window values.
            this.extensionContext.CopyValues(ctx);
            UserPrompts.PopupMessage("Extension updated.", "Success!");
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
                HandleSqlException(ex);

                Console.WriteLine(ex.Number + "  " + ex.ToString());
                return;
            }

            this.DataContext = extensionContext.FromDatabase();
            InputEnabled = false;
            UserPrompts.PopupMessage("Extension added.", "Success!");
        }

        private void HandleSqlException(MySql.Data.MySqlClient.MySqlException ex)
        {
            switch ((MySql.Data.MySqlClient.MySqlErrorCode)ex.Number)
            {
                case MySql.Data.MySqlClient.MySqlErrorCode.DuplicateKeyEntry:
                    var prompt = string.Format("An extension with the value '{0}' already exists in the database.", ((Extension)DataContext).Number);
                    UserPrompts.PopupMessage(prompt, "Duplicates Not Allowed");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.NoDefaultForField:
                    UserPrompts.PopupMessage(ex.Message, "Required Field Empty");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.DataTooLong:
                    UserPrompts.PopupMessage(ex.Message, "Data Too Long");
                    break;
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
    }
}