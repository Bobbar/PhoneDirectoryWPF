using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryWPF.UI;
using System.Windows;
using System.Threading;
using MaterialDesignThemes.Wpf;

namespace PhoneDirectoryWPF.Helpers
{
    /// <summary>
    /// Wrapper for displaying a <see cref="SpinnerDialog"/>.  Use with a using block to ensure that the spinner is closed after the operation within the block finishes.
    /// </summary>
    public class WaitSpinner : IDisposable
    {
        private SpinnerDialog spinner;
        private ManualResetEvent delayShowWaitHandle = new ManualResetEvent(false);
        private int showDelayMS = 50;

        /// <summary>
        /// Set/update the status text currently displayed on the <see cref="SpinnerDialog"/>. 
        /// </summary>
        public string StatusText
        {
            get
            {
                return spinner.StatusText;
            }

            set
            {
                spinner.StatusText = value;
            }
        }

        /// <summary>
        /// Creates a new instance of and displays a <see cref="WaitSpinner"/>.
        /// </summary>
        /// <param name="window">Window which contains a valid <see cref="DialogHost"/> to display the spinner.</param>
        /// <param name="initialStatus">The initial status message to be displayed.</param>
        public WaitSpinner(Window window, string initialStatus) : this(window, initialStatus, 0)
        {
        }

        /// <summary>
        /// Creates a new instance of and displays a <see cref="WaitSpinner"/>.
        /// </summary>
        /// <param name="window">Window which contains a valid <see cref="DialogHost"/> to display the spinner.</param>
        /// <param name="initialStatus">The initial status message to be displayed.</param>
        /// <param name="showDelay">The time in milliseconds that must elapse before the spinner is displayed. Prevents the spinner from being loaded/displayed during very short operations.</param>
        public WaitSpinner(Window window, string initialStatus, int showDelay)
        {
            showDelayMS = showDelay;

            if (showDelayMS < 0)
                showDelayMS = 0;

            LoadSpinner(window, initialStatus);
        }

        private async Task LoadSpinner(Window window, string initialStatus)
        {
            // Async wait handle to delay showing the spinner if desired.
            await Task.Run(() => delayShowWaitHandle.WaitOne(showDelayMS));

            // If the wait handle has been set, then the spinner has been
            // disposed before the delay period has elapsed, so we will
            // not attempt to display the spinner.
            if (delayShowWaitHandle.WaitOne(0))
                return;

            spinner = DisplaySpinner(window);
            spinner.StatusText = initialStatus;
        }

        private SpinnerDialog DisplaySpinner(Window window)
        {
            // Close any open dialogs.
            DialogHost.CloseDialogCommand.Execute(new object(), null);

            // Create a new spinner instance and show it on the specifed window.
            var spinner = new SpinnerDialog();
            DialogHostEx.ShowDialog(window, spinner, spinner.DialogHost_DialogOpened);
            return spinner;
        }

        public void Dispose()
        {
            delayShowWaitHandle.Set();
            spinner?.Hide();
        }
    }
}
