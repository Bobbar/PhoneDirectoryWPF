using Database.Data;
using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data;
using PhoneDirectoryWPF.Helpers;
using PhoneDirectoryWPF.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitDBControls();
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

        private async void SearchExtension()
        {
            var query = "SELECT * FROM extensions WHERE ";
            var queryParams = GetQueryParams();

            if (queryParams.Count < 1) return;

            await Task.Run(() =>
            {
                using (var results = DBFactory.GetMySqlDatabase().DataTableFromParameters(query, queryParams))
                {
                    var resultList = new List<Extension>();

                    foreach (DataRow row in results.Rows)
                    {
                        resultList.Add(new Extension(row));
                    }

                    PopulateResults(resultList);
                }
            });
        }

        private void EditExtension(Extension extension)
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Modify);

            var editWindow = new EditWindow(extension);
            editWindow.ShowDialog();
        }

        private void NewExtension()
        {
            SecurityFunctions.CheckForAccess(SecurityGroups.Add);

            var editWindow = new EditWindow();
            editWindow.Show();
        }

        private void PopulateResults(List<Extension> results)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => PopulateResults(results)));
            }
            else
            {
                resultListView.ItemsSource = null;
                resultListView.Items.Clear();
                resultListView.ItemsSource = results;
            }
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
            var item = resultListView.SelectedItem;
            Console.WriteLine(item.ToString());

            EditExtension((Extension)item);
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            NewExtension();
        }

    }
}