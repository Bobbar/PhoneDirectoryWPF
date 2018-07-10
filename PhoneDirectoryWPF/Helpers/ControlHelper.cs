using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhoneDirectoryWPF.Helpers
{
    public static class ControlHelper
    {
        public static List<Control> GetChildControls(Visual parent, List<Control> childList = null)
        {
            if (childList == null)
                childList = new List<Control>();

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var vis = (Visual)VisualTreeHelper.GetChild(parent, i);

                if (vis is Control)
                {
                    childList.Add((Control)vis);
                }

                if (VisualTreeHelper.GetChildrenCount(vis) > 0)
                {
                    childList.AddRange(GetChildControls(vis));
                }
            }

            return childList;
        }
    }
}