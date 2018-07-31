using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private bool inputEnabled = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<ExtensionDeletedEventArgs> ExtensionDeleted;

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

            new UIScaler(this, RootGrid, true);

            saveButton.Visibility = Visibility.Collapsed;
            deleteButton.Visibility = Visibility.Collapsed;

            this.Title = "New";
            this.FieldGroupBox.Header = "Add New Extension";

            extensionContext = new Extension();
            this.DataContext = extensionContext;

            InitSuggestions();
        }

        public EditWindow(Extension localExtension, Extension remoteExtension)
        {
            InitializeComponent();

            new UIScaler(this, RootGrid, true);

            addButton.Visibility = Visibility.Collapsed;

            this.Title = string.Format("Edit - {0}", localExtension.User);
            this.FieldGroupBox.Header = "Edit Extension";
            this.extensionContext = localExtension;
            this.DataContext = remoteExtension;

            InitSuggestions();
        }
      
        private async void InitSuggestions()
        {
            try
            {
                var departments = await MiscFunctions.DepartmentList();
                new PopupSuggestions(departmentTextBox, departments);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                ExceptionHandler.MySqlException(this, ex);

                return;
            }
        }

        private async void SetContextFromDatabase(Extension extension)
        {
            try
            {
                this.DataContext = await extension.FromDatabaseAsync();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                ExceptionHandler.MySqlException(this, ex);

                return;
            }
        }

        private bool FieldsValid(DependencyObject obj)
        {
            var valid = !Validation.GetHasError(obj) &&
                        LogicalTreeHelper.GetChildren(obj)
                        .OfType<DependencyObject>()
                        .All(FieldsValid);

            if (!valid)
                UserPrompts.PopupMessage(this, "One or more required fields are empty or invalid.", "Invalid Data");

            return valid;
        }

        private async void UpdateExtension()
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Modify);

            if (!FieldsValid(this))
                return;

            using (var spinner = new WaitSpinner(this, "Updating extension...", 150))
            {
                var ctx = (Extension)this.DataContext;

                try
                {
                    await ctx.UpdateAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    ExceptionHandler.MySqlException(this, ex);

                    return;
                }

                // Copy new values to the original context to update the main window values.
                this.extensionContext.CopyValues(ctx);
            }

            UserPrompts.PopupMessage(this, "Extension updated.", "Success!");
        }

        private async void AddExtension()
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Add);

            if (!FieldsValid(this))
                return;

            using (new WaitSpinner(this, "Adding extension...", 150))
            {
                try
                {
                    await extensionContext.InsertAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    ExceptionHandler.MySqlException(this, ex);

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
                    ExceptionHandler.MySqlException(this, ex);

                    return;
                }
            }

            await UserPrompts.PopupDialog(this, string.Format("Extension '{0}' deleted!", extensionContext.Number), "Success", DialogButtons.Default);
            OnExtensionDeleted(extensionContext);
            ((Extension)DataContext).Dispose();
            extensionContext.Dispose();
            this.Close();
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
    }
}