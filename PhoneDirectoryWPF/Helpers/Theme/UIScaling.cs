using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PhoneDirectoryWPF.Helpers
{
    public static class UIScaling
    {
        public static List<int> ScaleValues { get { return scaleValues; } }

        private static List<int> scaleValues = new List<int> { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200 };
        private static int currentScaleIndex = 5;
        private static List<FrameworkElement> scaleTargets = new List<FrameworkElement>();
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

                    PerformScaleChange(currentScalePercent);
                }
            }
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

        public static void AddScaleTarget(FrameworkElement target)
        {
            if (!scaleTargets.Contains(target))
                scaleTargets.Add(target);

            PerformScaleChange(CurrentScalePercent);
        }

        private static void PerformScaleChange(int scale)
        {
            // Convert scale from percent to decimal.  (125% = 1.25)
            var scaleDecimal = scale * 0.01F;

            foreach (var target in scaleTargets)
            {
                target.LayoutTransform = new ScaleTransform(scaleDecimal, scaleDecimal, 0, 0);
            }
        }
    }
}