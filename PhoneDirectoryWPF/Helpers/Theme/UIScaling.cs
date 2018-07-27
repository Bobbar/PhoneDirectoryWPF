using System;
using System.Collections.Generic;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UIScaling
    {
        public static event EventHandler<int> ScaleChanged;

        public static List<int> ScaleValues { get { return scaleValues; } }

        private static List<int> scaleValues = new List<int> { 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180 };
        private static int currentScaleIndex = 10;
        private static int currentScalePercent = 0;

        public static int CurrentScalePercent
        {
            get
            {
                if (currentScalePercent == 0)
                    currentScalePercent = scaleValues[currentScaleIndex];

                return currentScalePercent;
            }

            set
            {
                if (currentScalePercent != value)
                {
                    currentScalePercent = value;
                    currentScaleIndex = scaleValues.IndexOf(currentScalePercent);

                    UserSettings.SaveSetting(AppSettings.UIScale, currentScalePercent);

                    OnScaledChanged(currentScalePercent);
                }
            }
        }

        private static void OnScaledChanged(int newScale)
        {
            ScaleChanged?.Invoke(null, newScale);
        }

        public static void ScaleUp()
        {
            if (currentScaleIndex + 1 <= scaleValues.Count - 1)
            {
                currentScaleIndex++;
                CurrentScalePercent = scaleValues[currentScaleIndex];
            }
        }

        public static void ScaleDown()
        {
            if (currentScaleIndex - 1 >= 0)
            {
                currentScaleIndex--;
                CurrentScalePercent = scaleValues[currentScaleIndex];
            }
        }
    }
}