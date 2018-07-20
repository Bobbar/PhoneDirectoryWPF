using Database.Data;
using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data;
using PhoneDirectoryWPF.Data.Functions;
using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PopupSpinner workingSpinner;

        public MainWindow()
        {
            InitializeComponent();
            InitDBControls();

            workingSpinner = new PopupSpinner(resultListView);

            WatchDogInstance.WatchDog.CacheStatusChanged += WatchDog_CacheStatusChanged;
        }

        private void WatchDog_CacheStatusChanged(object sender, bool e)
        {
            DBFactory.CacheMode = e;

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => SetCacheStatus(e)));
            }
            else
            {
                SetCacheStatus(e);
            }
        }

        private void SetCacheStatus(bool cacheMode)
        {
            if (cacheMode)
            {
                ConnectStatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.LanDisconnect;
                ConnectStatusText.Text = "Offline Mode";
            }
            else
            {
                ConnectStatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.LanConnect;
                ConnectStatusText.Text = "Connected";
            }
        }

        private async void InitConnection()
        {
            bool canReach = false;
            bool cacheVerified = false;

            using (var spinner = new WaitSpinner(this, "Connecting to database...", 2000))
            {
                await Task.Run(() =>
                {
                    canReach = DBFactory.CanReachServer();

                    spinner.StatusText = "Checking cache...";

                    cacheVerified = CacheFunctions.VerifyCache();

                    if (canReach)
                    {
                        spinner.StatusText = "Loading data...";

                        CacheFunctions.RefreshCache();
                    }

                    SecurityFunctions.PopulateUserAccess();
                    SecurityFunctions.PopulateAccessGroups();
                });
            }

            if (!cacheVerified && !canReach)
            {
                await UserPrompts.PopupMessage(this, "Cannot connect to the database and the local cache was not verified.", "Cannot Run");
                Application.Current.Shutdown();
            }
            else if (cacheVerified && !canReach)
            {
                WatchDogInstance.WatchDog.Start(true);
            }
            else
            {
                WatchDogInstance.WatchDog.Start(false);
            }

            FieldsGrid.IsEnabled = true;
        }

        private void InitDBControls()
        {
            extensionTextBox.Tag = new DBControlAttribute("extension", ControlSearchType.StartsWith);
            userTextBox.Tag = new DBControlAttribute("user", ControlSearchType.EntireValue);
            departmentTextBox.Tag = new DBControlAttribute("department", ControlSearchType.StartsWith);
        }

        private List<DBQueryParameter> GetQueryParams()
        {
            var queryParams = new List<DBQueryParameter>();
            var controls = ControlHelper.GetChildControls(this);

            foreach (Control ctl in controls)
            {
                if (ctl.Tag != null && ctl.Tag is DBControlAttribute)
                {
                    var dbAttr = (DBControlAttribute)ctl.Tag;
                    string ctlValue = null;

                    if (ctl is TextBox)
                    {
                        ctlValue = ((TextBox)ctl).Text.Trim();
                    }

                    if (!string.IsNullOrEmpty(ctlValue))
                    {
                        switch (dbAttr.SearchType)
                        {
                            case ControlSearchType.EntireValue:
                                queryParams.Add(new DBQueryParameter(dbAttr.ColumnName, ctlValue, WildCardType.Both, "AND"));
                                break;

                            case ControlSearchType.StartsWith:
                                queryParams.Add(new DBQueryParameter(dbAttr.ColumnName, ctlValue, WildCardType.StartsWith, "AND"));
                                break;

                            case ControlSearchType.EndsWith:
                                queryParams.Add(new DBQueryParameter(dbAttr.ColumnName, ctlValue, WildCardType.EndsWith, "AND"));
                                break;
                        }
                    }
                }
            }

            return queryParams;
        }

        private bool searchRunning = false;
        private bool searchNeeded = false;

        private async void SearchExtension()
        {
            // Reentrancy logic:
            // If a search is already running, set a variable so
            // that we know that we need to run another search
            // after the current one finishes.
            if (searchRunning)
            {
                searchNeeded = true;
                return;
            }

            searchRunning = true;
            searchNeeded = false;

            try
            {
                var query = "SELECT * FROM extensions WHERE";
                var queryParams = GetQueryParams();

                if (queryParams.Count < 1) return;

                workingSpinner.Wait(200);

                var searchResults = await Task.Run(() =>
                {
                    using (var results = DBFactory.GetDatabase().DataTableFromParameters(query, queryParams))
                    {
                        var resultList = new List<Extension>();

                        foreach (DataRow row in results.Rows)
                        {
                            resultList.Add(new Extension(row));
                        }

                        return resultList;
                    }
                });

                PopulateResults(searchResults);

                workingSpinner.Hide();
            }
            finally
            {
                searchRunning = false;

                if (searchNeeded)
                    SearchExtension();
            }
        }

        private void PopulateResults(List<Extension> results)
        {
            resultListView.ItemsSource = null;
            resultListView.Items.Clear();
            resultListView.ItemsSource = results;
        }

        private void EditExtension(Extension extension)
        {
            if (extension == null)
                return;

            SecurityFunctions.CheckForAccess(SecurityGroups.Modify);

            var editWindow = new EditWindow(extension);
            editWindow.ExtensionDeleted -= EditWindow_ExtensionDeleted;
            editWindow.ExtensionDeleted += EditWindow_ExtensionDeleted;
            editWindow.ShowDialog();
        }

        private void EditWindow_ExtensionDeleted(object sender, ExtensionDeletedEventArgs e)
        {
            var source = (List<Extension>)resultListView.ItemsSource;

            if (source.Contains(e.Extension))
                source.Remove(e.Extension);

            var view = CollectionViewSource.GetDefaultView(source);
            view.Refresh();
        }

        private void NewExtension()
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Add);

            var editWindow = new EditWindow();
            editWindow.Show();
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            extensionTextBox.Clear();
            userTextBox.Clear();
            departmentTextBox.Clear();
            resultListView.ItemsSource = null;
            resultListView.Items.Clear();
        }

        private void extensionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchExtension();
        }

        private void userTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchExtension();
        }

        private void departmentTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchExtension();
        }

        private void resultListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = ((FrameworkElement)e.OriginalSource).DataContext;

            if (data != null && data is Extension)
            {
                EditExtension((Extension)data);
            }
        }

        private void resultListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var data = ((FrameworkElement)e.OriginalSource).DataContext;

                if (data != null && data is Extension)
                {
                    EditExtension((Extension)data);
                }
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            NewExtension();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditExtension((Extension)resultListView.SelectedItem);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InitConnection();
        }
    }
}