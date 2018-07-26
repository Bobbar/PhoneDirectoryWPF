using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows.Controls;
using System;

namespace PhoneDirectoryWPF.UI
{
    /// <summary>
    /// Interaction logic for SpinnerDialog.xaml
    /// </summary>
    public partial class SpinnerDialog : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string statusText = "";
        private DialogHost dialogHost;
        private DialogSession dialogSession;

        public string StatusText
        {
            get
            {
                return statusText;
            }

            set
            {
                statusText = value;
                OnNotifyPropertyChanged(nameof(StatusText));
            }
        }

        public SpinnerDialog()
        {
            InitializeComponent();
            Helpers.UIScaling.AddScaleTarget(this);
        }

        public SpinnerDialog(DialogHost host)
        {
            this.dialogHost = host;
            this.dialogHost.DialogOpened += DialogHost_DialogOpened;
            InitializeComponent();
            Helpers.UIScaling.AddScaleTarget(this);
        }

        public void DialogHost_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            dialogSession = eventArgs.Session;
        }

        private void OnNotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Hide()
        {
            if (dialogSession == null)
                return;

            if (!dialogSession.IsEnded)
                dialogSession.Close();
        }
    }
}