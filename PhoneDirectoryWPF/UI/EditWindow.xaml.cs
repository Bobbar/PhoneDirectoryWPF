using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PhoneDirectoryWPF.Containers;
using PhoneDirectoryWPF.Data.Functions;
using System.Data.Sql;
using System.Data.SqlClient;
namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
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
            this.extensionContext = extension;
            this.DataContext = extensionContext;
            addButton.Visibility = Visibility.Collapsed;
        }

        private void UpdateExtension()
        {
            // TODO: Field verification.
            MapObjectFunctions.UpdateMapObject(extensionContext);
        }

        private void AddExtension()
        {
            // TODO: Field verification.

            try
            {

                MapObjectFunctions.InsertMapObject(extensionContext);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine(ex.GetType().ToString() + "  " + ex.ToString());
                            
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
