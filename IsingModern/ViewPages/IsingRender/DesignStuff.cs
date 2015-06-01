using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IsingModern.Model;
using OxyPlot.Annotations;

namespace IsingModern.ViewPages {
    public partial class IsingRender {

        private readonly SolidColorBrush[] bgColors =
        {
            new SolidColorBrush(Color.FromRgb(37, 37, 38)),
            new SolidColorBrush(Colors.White),
        };

        private readonly Color[] textColors = { Colors.White, Colors.Black };
        private readonly Color[] legendColors = { Color.FromRgb(50, 50, 50), Color.FromRgb(240, 240, 240) };
        private readonly Color[] lineColors = { Color.FromRgb(60, 60, 60), Color.FromRgb(220, 220, 220) };
        private readonly String[] trackerStrings = { "TDark", "TLight" };


        public void SwitchAccentColor(Color accentColor, Color accentDark) {
            var grad = new LinearGradientBrush {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            grad.GradientStops.Add(new GradientStop(accentDark, 0.0));
            grad.GradientStops.Add(new GradientStop(accentColor, 1.0));
            grad.Freeze();
            MagnetizationPlot.Color = accentColor;
            FieldRectangle.Fill = grad;
            mlineM.Color = accentColor.SetTransparency(120);
            mlineP.Color = accentColor.SetTransparency(120);
        }

        public void UpdateHelpLines() {
            double n = CurrentN * CurrentN;
            var active = n - Lattice.ZeroSpins;
            var maxM = active / n;

            mlineP.Intercept = maxM;
            mlineM.Intercept = -maxM;
        }

        public void SwitchTheme(bool dark) {
            var index = dark ? 0 : 1;
            Plot.Background = bgColors[index];
            Plot.LegendBackground = legendColors[index];
            Plot.TextColor = textColors[index];
            Plot.PlotAreaBorderColor = LeftAxis.TicklineColor = BottomAxis.TicklineColor = textColors[index];
            line.Color = lineColors[index];
            EnergyPlot.TrackerKey = MagnetizationPlot.TrackerKey = trackerStrings[index];
        }
    }

    public static class Extensions {
        public static Color SetTransparency(this Color color, byte a) {
            return Color.FromArgb(a, color.R, color.G, color.B);
        }
    }
}
