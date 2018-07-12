using System.Windows.Controls;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for PopupDialog.xaml
    /// </summary>
    public partial class PopupDialog : UserControl
    {
        public PopupDialog()
        {
            InitializeComponent();
        }

        public PopupDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        public PopupDialog(string message, string header)
        {
            InitializeComponent();
            MessageText.Text = message;
            MessageHeader.Header = header;
        }
    }
}