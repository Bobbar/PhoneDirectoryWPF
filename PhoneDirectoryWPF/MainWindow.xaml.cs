using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhoneDirectoryWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void extensionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchExtension();
        }

        private async void SearchExtension()
        {
            var value = extensionTextBox.Text.Trim();

            var query = "SELECT * FROM extensions WHERE extension LIKE '" + value + "%'";

            await Task.Run(() =>
            {
                using (var results = DBFactory.GetMySQLDatabase().DataTableFromQueryString(query))
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
    }
}