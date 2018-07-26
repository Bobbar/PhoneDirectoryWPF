using System.Windows;
using System.Windows.Controls;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for PopupDialog.xaml
    /// </summary>
    public partial class PopupDialog : UserControl
    {
        public PopupDialog() : this(string.Empty, string.Empty, DialogButtons.Default)
        {
        }

        public PopupDialog(string message) : this(message, string.Empty, DialogButtons.Default)
        {
        }

        public PopupDialog(string message, string header) : this(message, header, DialogButtons.Default)
        {
        }

        public PopupDialog(string message, string header, DialogButtons buttons)
        {
            InitializeComponent();
            MessageText.Text = message;
            MessageHeader.Header = header;
            SetupButtons(buttons);
            Helpers.UIScaling.AddScaleTarget(this);
        }

        private void SetupButtons(DialogButtons buttons)
        {
            switch (buttons)
            {
                case DialogButtons.Default:
                    OKButton.Visibility = Visibility.Collapsed;
                    CancelButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Collapsed;
                    NoButton.Visibility = Visibility.Collapsed;
                    break;

                case DialogButtons.OKCancel:
                    DefaultOKButton.Visibility = Visibility.Collapsed;
                    OKButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;

                case DialogButtons.OKOnly:
                    DefaultOKButton.Visibility = Visibility.Collapsed;
                    OKButton.Visibility = Visibility.Visible;
                    break;

                case DialogButtons.YesNo:
                    DefaultOKButton.Visibility = Visibility.Collapsed;
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
            }
        }
    }

    public enum DialogButtons
    {
        Default,
        YesNo,
        OKOnly,
        OKCancel
    }
}