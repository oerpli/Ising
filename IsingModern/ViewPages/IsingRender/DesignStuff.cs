using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Wpf.Properties;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf.Properties;

namespace IsingModern.ViewPages {
    public partial class IsingRender {

        private readonly SolidColorBrush[] bgColors =
        {
            new SolidColorBrush(Color.FromRgb(50, 50, 50)),
            new SolidColorBrush(Colors.White)
        };

        private readonly Color[] textColors = { Colors.White, Colors.Black };
        private readonly Color[] legendColors = { Color.FromRgb(30, 30, 30), Color.FromRgb(240, 240, 240) };
        private readonly Color[] lineColors = { Color.FromRgb(40, 40, 40), Color.FromRgb(220, 220, 220) };


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
        }
        public void SwitchTheme(bool dark) {
            var index = dark ? 0 : 1;
            Plot.Background = bgColors[index];
            Plot.LegendBackground = legendColors[index];
            Plot.TextColor = textColors[index];
            Plot.PlotAreaBorderColor = LeftAxis.TicklineColor = BottomAxis.TicklineColor = textColors[index];
            line.Color = lineColors[index];

            EnergyPlot.TrackerKey = MagnetizationPlot.TrackerKey = dark ? "TDark" : "TLight";
            var c = new ControlTemplate();
            var t = new TrackerControl();
            t.Background = new SolidColorBrush(legendColors[index]);

        }
    }
}
