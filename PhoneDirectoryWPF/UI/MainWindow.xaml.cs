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
using System.ComponentModel;
using System.Windows.Media;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private PopupSpinner workingSpinner;
        private bool searchRunning = false;
        private bool searchNeeded = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int SelectedScale
        {
            get
            {
                return UIScaling.CurrentScalePercent;
            }

            set
            {
                UIScaling.CurrentScalePercent = value;
                OnPropertyChanged(nameof(SelectedScale));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            InitDBControls();

            workingSpinner = new PopupSpinner(resultListView);

            WatchDogInstance.WatchDog.CacheStatusChanged += WatchDog_CacheStatusChanged;

            UIScaling.AddScaleTarget(RootGrid);

            RestoreUserSettings();
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

            // If current user can modify, add edit button column to grid.
            if (SecurityFunctions.CanAccess(SecurityGroups.Delete) || SecurityFunctions.CanAccess(SecurityGroups.Modify))
                AddEditButton();

            FieldsGrid.IsEnabled = true;
        }

        private void RestoreUserSettings()
        {
            var uiScale = UserSettings.GetSetting(AppSettings.UIScale);
            if (uiScale != null)
                SelectedScale = (int)uiScale;
        }

        private void InitDBControls()
        {
            extensionTextBox.Tag = new DBControlAttribute("extension", ControlSearchType.StartsWith);
            userTextBox.Tag = new DBControlAttribute("user", ControlSearchType.EntireValue);
            departmentTextBox.Tag = new DBControlAttribute("department", ControlSearchType.EntireValue);
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

                var asyncResults = await Task.Run(() =>
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

                PopulateResults(asyncResults);

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

        private async void EditExtension(Extension extension)
        {
            if (extension == null)
                return;

            SecurityFunctions.CheckForAccess(SecurityGroups.Modify);

            Extension remoteExtension;

            using (new WaitSpinner(this, "Loading extension...", 150))
            {
                try
                {
                    remoteExtension = (Extension)await extension.FromDatabaseAsync();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    ExceptionHandler.MySqlException(this, ex);

                    return;
                }
            }

            var editWindow = new EditWindow(extension, remoteExtension);
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

        private void AddEditButton()
        {
            var col = new GridViewColumn();
            col.Width = 30;
            col.CellTemplate = FindResource("EditButtonTemplate") as DataTemplate;

            resultGridView.Columns.Insert(0, col);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            extensionTextBox.Clear();
            userTextBox.Clear();
            departmentTextBox.Clear();
            resultListView.ItemsSource = null;
            resultListView.Items.Clear();
        }

        private void searchField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift && e.Key != Key.RightShift && e.Key != Key.Enter)
            {
                SearchExtension();
            }
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
            var data = ((FrameworkElement)e.OriginalSource).DataContext;

            if (data != null && data is Extension)
            {
                EditExtension((Extension)data);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InitConnection();
        }

        private void ScaleDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIScaling.ScaleDown();
            SelectedScale = UIScaling.CurrentScalePercent;
        }

        private void ScaleUpButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIScaling.ScaleUp();
            SelectedScale = UIScaling.CurrentScalePercent;
        }
    }
}