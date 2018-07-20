using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PhoneDirectoryWPF.Helpers
{
    // TODO: Add support for positioning and sizing. 
    public class PopupSpinner : IDisposable
    {
        private ProgressBar spinner;
        private Popup popup;
        private FrameworkElement target;
        private int delay = 100;
        private ManualResetEvent delayShowWaitHandle = new ManualResetEvent(true);

        public PopupSpinner(FrameworkElement target, int displayDelay)
        {
            this.target = target;
            this.delay = displayDelay;

            InitSpinner();
            InitPopup();
        }

        public PopupSpinner(FrameworkElement target)
        {
            this.target = target;

            InitSpinner();
            InitPopup();
        }

        private void InitSpinner()
        {
            spinner = new ProgressBar();
            spinner.Style = Application.Current.FindResource("MaterialDesignCircularProgressBar") as Style;
            spinner.Height = 50;
            spinner.Width = 50;
            spinner.IsIndeterminate = true;
            spinner.Value = 0;
        }

        private void InitPopup()
        {
            popup = new Popup();
            popup.Child = spinner;
            popup.StaysOpen = true;
            popup.AllowsTransparency = true;
            popup.PlacementTarget = target;
            popup.Placement = PlacementMode.Center;

            var targetWindow = Window.GetWindow(target);
            targetWindow.LocationChanged += PopupSpinner_LocationChanged;
            targetWindow.SizeChanged += TargetWindow_SizeChanged;
        }

        // Bump the horizontal offset to trigger a repositioning of the popup, 
        // since it doesn't detect the target movements on its own.
        private void BumpOffset()
        {
            if (popup.IsOpen)
            {
                var offset = popup.HorizontalOffset;
                popup.HorizontalOffset = offset + 1;
                popup.HorizontalOffset = offset;
            }
        }

        private void TargetWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BumpOffset();
        }

        private void PopupSpinner_LocationChanged(object sender, EventArgs e)
        {
            BumpOffset();
        }

        public async void Wait()
        {
            Wait(delay);
        }

        public async void Wait(int displayDelay)
        {
            if (!delayShowWaitHandle.WaitOne(0))
                return;

            delayShowWaitHandle.Reset();

            await Task.Run(() => delayShowWaitHandle.WaitOne(displayDelay));

            if (delayShowWaitHandle.WaitOne(0))
                return;

            popup.IsOpen = true;
        }

        public void Hide()
        {
            delayShowWaitHandle.Set();
            popup.IsOpen = false;
        }

        public void Dispose()
        {
            delayShowWaitHandle.Set();
            popup.IsOpen = false;
            delayShowWaitHandle.Dispose();
        }
    }
}