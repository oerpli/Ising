using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using IsingModern.ViewPages.Settings;

namespace IsingModern.ViewPages {
    public partial class IsingRender {


        public void RandomizeGrid() {
            _viewmodel.Randomize(true);
            randomize = false;
        }

        public void Color(Color accentDark, Color accentColor) {
            var grad = new LinearGradientBrush {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            grad.GradientStops.Add(new GradientStop(accentDark, 0.0));
            grad.GradientStops.Add(new GradientStop(accentColor, 1.0));
            grad.Freeze();
            FieldRectangle.Fill = grad;
        }

        public void NewSize() {
        }

    }
}
