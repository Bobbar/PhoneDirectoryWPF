using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace PhoneDirectoryWPF.Helpers
{
    /// <summary>
    /// Provides a suggestion popup list for a <see cref="TextBox"/>.
    /// </summary>
    public sealed class PopupSuggestions
    {
        private TextBox target;
        private List<object> items;
        private ListBox popList;
        private Popup popup;
        private ObservableCollection<object> view = new ObservableCollection<object>();

        public PopupSuggestions(TextBox targetTextBox)
        {
            this.target = targetTextBox;

            SetupPopup();
        }

        public PopupSuggestions(TextBox targetTextBox, List<object> suggestItems)
        {
            this.target = targetTextBox;
            items = suggestItems;

            SetupPopup();
        }

        private void SetupPopup()
        {
            InitListControl();
            InitPopup();
            AttachEvents();
        }

        private void InitListControl()
        {
            popList = new ListBox();
            popList.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            popList.Background = Application.Current.FindResource("MaterialDesignPaper") as Brush;
            popList.Foreground = Application.Current.FindResource("MaterialDesignBody") as Brush;
            popList.ItemsSource = view;
        }

        private void InitPopup()
        {
            popup = new Popup();
            popup.Child = popList;
            popup.StaysOpen = false;
            popup.AllowsTransparency = false;
            popup.PlacementTarget = target;
            popup.Placement = PlacementMode.Bottom;

            new UIScaler(popup);
        }

        private void AttachEvents()
        {
            target.KeyUp += Target_KeyUp;
            target.KeyDown += Target_KeyDown;
            target.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            popList.PreviewKeyDown += PopList_KeyDown;
            popList.PreviewMouseDown += PopList_MouseDown;
        }

        private void DetachEvents()
        {
            target.KeyUp -= Target_KeyUp;
            target.KeyDown -= Target_KeyDown;
            target.Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;

            popList.PreviewKeyDown -= PopList_KeyDown;
            popList.PreviewMouseDown -= PopList_MouseDown;
        }

        /// <summary>
        /// Rebuilds the displayed item list and shows or hides the popup list depending on results.
        /// </summary>
        private void RefreshItems()
        {
            view.Clear();

            // Refresh colors to react to theme changes.
            popList.Background = Application.Current.FindResource("MaterialDesignPaper") as Brush;
            popList.Foreground = Application.Current.FindResource("MaterialDesignBody") as Brush;

            var searchText = target.Text.Trim();

            // Iterate the items and only add matching results to the view.
            foreach (var item in items)
            {
                var itemString = item.ToString();

                bool inResults = itemString.Contains(searchText, StringComparison.CurrentCultureIgnoreCase);

                if (inResults)
                {
                    view.Add(item);
                }
            }

            // Only display the popup if we have data.
            if (view.Count > 0)
            {
                Show();
            }
            else
            {
                Hide();
            }


            // Set the popup list width to "Auto" and update the layout to resize it to the contents.
            popList.Width = Double.NaN;
            popList.UpdateLayout();

            // If the list is wider than the target control,
            // set the popup offset to align it to the left
            // size of the target with the list overflowing to the right.
            if (popList.ActualWidth > target.ActualWidth)
            {
                popup.HorizontalOffset = popList.ActualWidth - target.ActualWidth;
            }
            else // Otherwise set the offset to zero and set the list width to match the target.
            {
                popup.HorizontalOffset = 0;
                popList.Width = target.ActualWidth;
            }
        }

        private void Show()
        {
            popup.IsOpen = true;
        }

        private void Hide()
        {
            popup.IsOpen = false;
        }

        private void PopList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If a left-click is performed, set target text and hide.
            if (e.ChangedButton == MouseButton.Left)
            {
                var data = ((FrameworkElement)e.OriginalSource).DataContext;

                if (data != null)
                {
                    target.Text = data.ToString();
                }

                Hide();
            }
        }

        private void PopList_KeyDown(object sender, KeyEventArgs e)
        {
            // If enter or tab is pressed in the popup list,
            // set the target to the selected item and hide.
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                var data = ((FrameworkElement)e.OriginalSource).DataContext;

                if (data != null)
                {
                    target.Text = data.ToString();
                }

                Hide();
            }
        }

        private void Target_KeyDown(object sender, KeyEventArgs e)
        {
            // If enter or tab is pressed in the target control,
            // and the popup only has one item, set the target text and hide.
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (popup.IsOpen && view.Count == 1)
                {
                    target.Text = view[0].ToString();
                    Hide();
                }
            }
        }

        private void Target_KeyUp(object sender, KeyEventArgs e)
        {
            // Target has no text to search, hide and return silently.
            if (string.IsNullOrEmpty(target.Text))
            {
                Hide();
                return;
            }

            // If down arrow is pressed, select the first popup list item and focus it.
            if (e.Key == Key.Down)
            {
                if (popup.IsOpen)
                {
                    popList.SelectedIndex = 0;
                    var selectedItem = (ListBoxItem)popList.ItemContainerGenerator.ContainerFromIndex(0);
                    selectedItem.Focus();
                }
            }
            else
            {
                // For all other key presses, perform a refresh.
                RefreshItems();
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            DetachEvents();
            items.Clear();
            items = null;
            popList.ItemsSource = null;
            view.Clear();
            view = null;
        }
    }
}