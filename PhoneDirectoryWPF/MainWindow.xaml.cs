using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhoneDirectoryWPF.Data;
using PhoneDirectoryWPF.Containers;
using System.Data;


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

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void extensionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SearchExtension();
        }

        private void SearchExtension()
        {
            var value = extensionTextBox.Text.Trim();

            var query = "SELECT * FROM extensions WHERE extension LIKE '" + value + "%'";


            using (var results = DBFactory.GetMySQLDatabase().DataTableFromQueryString(query))
            {
                var resultList = new List<Extension>();

                foreach (DataRow row in results.Rows)
                {
                    resultList.Add(new Extension(row));

                }

                PopulateResults(resultList);

            }

        }


        private void PopulateResults(List<Extension> results)
        {
            resultListView.ItemsSource = null;
            resultListView.Items.Clear();
            resultListView.ItemsSource = results;

        }
    }
}
