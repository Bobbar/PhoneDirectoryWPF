using System.Windows;
using System.Windows.Media;

namespace PhoneDirectoryWPF.Helpers
{
    /// <summary>
    /// Manages scaling and resizing for windows and elements.
    /// </summary>
    public class UIScaler
    {
        private Window parentWindow;
        private FrameworkElement rootElement;
        private Size initialWindowSize;
        private bool autoSize = false;

        private int currentScale
        {
            get
            {
                return UIScaling.CurrentScalePercent;
            }
        }

        /// <summary>
        /// Creates a new instance and beings scaling and resizing operations.
        /// </summary>
        /// <param name="parent">Parent window containing elements to be scaled. Needed for window resizing.</param>
        /// <param name="scaledElement">The top-most or root element to be scaled.</param>
        /// <param name="autoSizeWindow">When true, the parent window will be resized to match the scaling.</param>
        public UIScaler(Window parent, FrameworkElement scaledElement, bool autoSizeWindow)
        {
            parentWindow = parent;
            autoSize = autoSizeWindow;
            rootElement = scaledElement;

            if (parent != null)
                parentWindow.Loaded += ParentWindow_Loaded;

            UIScaling.ScaleChanged += UIScaling_ScaleChanged;
        }

        /// <summary>
        /// Creates a new instance and beings scaling operations.
        /// </summary>
        /// <param name="scaledElement">The top-most or root element to be scaled.</param>
        public UIScaler(FrameworkElement scaledElement) : this(null, scaledElement, false)
        {
            PerformScaling(currentScale);
        }

        /// <summary>
        /// Applies the specified scaling, and resizes the window if specified.
        /// </summary>
        /// <param name="scale"></param>
        private void PerformScaling(int scale)
        {
            var scaleDecimal = scale * 0.01F;

            rootElement.LayoutTransform = new ScaleTransform(scaleDecimal, scaleDecimal, 0, 0);

            if (parentWindow != null && autoSize)
            {
                parentWindow.Height = initialWindowSize.Height * scaleDecimal;
                parentWindow.Width = initialWindowSize.Width * scaleDecimal;
                CenterWindowOnScreen();
            }
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = parentWindow.Width;
            double windowHeight = parentWindow.Height;
            parentWindow.Left = (screenWidth / 2) - (windowWidth / 2);
            parentWindow.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void ParentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            initialWindowSize = parentWindow.RenderSize;
            PerformScaling(currentScale);
        }

        private void UIScaling_ScaleChanged(object sender, int e)
        {
            PerformScaling(e);
        }
    }
}