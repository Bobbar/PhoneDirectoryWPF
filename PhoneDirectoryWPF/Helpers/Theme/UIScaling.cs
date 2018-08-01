using System;
using System.Collections.Generic;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UIScaling
    {
        public static event EventHandler<int> ScaleChanged;

        private static int currentScalePercent = 100;

        public static int CurrentScalePercent
        {
            get
            {
                return currentScalePercent;
            }

            set
            {
                if (currentScalePercent != value && value >= 50 && value <= 180)
                {
                    currentScalePercent = value;

                    UserSettings.SaveSetting(AppSettings.UIScale, currentScalePercent);

                    OnScaledChanged(currentScalePercent);
                }
            }
        }

        private static void OnScaledChanged(int newScale)
        {
            ScaleChanged?.Invoke(null, newScale);
        }
    }
}