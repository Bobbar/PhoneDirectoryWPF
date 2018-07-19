using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
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

        public event EventHandler<ExtensionDeletedEventArgs> ExtensionDeleted;

        private bool inputEnabled = true;

        private bool isNew = false;

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

        private void OnExtensionDeleted(Extension extension)
        {
            ExtensionDeleted?.Invoke(this, new ExtensionDeletedEventArgs(extension));
        }

        public EditWindow()
        {
            InitializeComponent();
            saveButton.Visibility = Visibility.Collapsed;
            deleteButton.Visibility = Visibility.Collapsed;

            isNew = true;
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
            SetContextFromDatabase(extension);
        }

        private async void SetContextFromDatabase(Extension extension)
        {
            try
            {
                this.DataContext = await extension.FromDatabaseAsync();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                HandleSqlException(ex);

                return;
            }
        }

        private async void UpdateExtension()
        {
            // TODO: Field verification.
            SecurityFunctions.CheckForAccess(SecurityGroups.Modify);

            using (var spinner = new WaitSpinner(this, "Updating extension...", 150))
            {
                var ctx = (Extension)this.DataContext;

                try
                {
                    await ctx.UpdateAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    HandleSqlException(ex);

                    Console.WriteLine(ex.Number + "  " + ex.ToString());
                    return;
                }

                // Copy new values to the original context to update the main window values.
                this.extensionContext.CopyValues(ctx);
            }

            UserPrompts.PopupMessage(this, "Extension updated.", "Success!");
        }

        private async void AddExtension()
        {
            // TODO: Field verification.
            SecurityFunctions.CheckForAccess(SecurityGroups.Add);

            using (new WaitSpinner(this, "Adding extension...", 150))
            {
                try
                {
                    await extensionContext.InsertAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    HandleSqlException(ex);

                    Console.WriteLine(ex.Number + "  " + ex.ToString());
                    return;
                }

                this.DataContext = await extensionContext.FromDatabaseAsync();
                InputEnabled = false;
            }

            UserPrompts.PopupMessage(this, "Extension added.", "Success!");
        }

        private async void DeleteExtension()
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Delete);

            var result = (bool)await UserPrompts.PopupDialog(this, "Are you sure you want to delete this extension?", "Delete Extension", DialogButtons.YesNo);

            if (!result)
                return;

            var ctx = (Extension)this.DataContext;

            using (new WaitSpinner(this, "Deleting extension...", 150))
            {
                try
                {
                    await ctx.DeleteFromDatabaseAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    HandleSqlException(ex);

                    Console.WriteLine(ex.Number + "  " + ex.ToString());
                    return;
                }
            }

            await UserPrompts.PopupDialog(this, string.Format("Extension '{0}' deleted!", extensionContext.Number), "Success", DialogButtons.Default);
            OnExtensionDeleted(extensionContext);
            ((Extension)DataContext).Dispose();
            extensionContext.Dispose();
            this.Close();
        }

        private async void HandleSqlException(MySql.Data.MySqlClient.MySqlException ex)
        {
            Logging.Exception(ex);

            switch ((MySql.Data.MySqlClient.MySqlErrorCode)ex.Number)
            {
                case MySql.Data.MySqlClient.MySqlErrorCode.DuplicateKeyEntry:
                    var prompt = string.Format("An extension with the value '{0}' already exists in the database.", ((Extension)DataContext).Number);
                    UserPrompts.PopupMessage(this, prompt, "Duplicates Not Allowed");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.NoDefaultForField:
                    UserPrompts.PopupMessage(this, ex.Message, "Required Field Empty");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.DataTooLong:
                    UserPrompts.PopupMessage(this, ex.Message, "Data Too Long");
                    break;

                case MySql.Data.MySqlClient.MySqlErrorCode.UnableToConnectToHost:
                    await UserPrompts.PopupDialog(this, "Could not connect to the server.", "Connection Lost", DialogButtons.Default);
                    this.Close();
                    break;

                default:
                    UserPrompts.PopupMessage(this, ex.Message, "Unexpected Database Error!");
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

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteExtension();
        }

        private void EditWindow_ContentRendered(object sender, EventArgs e)
        {
            // If content is rendered before the data context has been populated
            // display a progress spinner until it has been.
            if (!isNew && ((Extension)this.DataContext).Number == null)
            {
                FieldGroupBox.IsEnabled = false;
                WaitingSpinner.Visibility = Visibility.Visible;
            }
        }

        private void EditWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If the datacontext is populated with a valid extension,
            // hide the progress spinner and enable the fields.
            if (((Extension)this.DataContext).Number != null && !isNew)
            {
                FieldGroupBox.IsEnabled = true;
                WaitingSpinner.Visibility = Visibility.Hidden;
            }
        }
    }
}