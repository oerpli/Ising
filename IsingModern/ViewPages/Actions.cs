using System.Windows;
using System.Windows.Media;

namespace IsingModern.ViewPages {
    public partial class IsingRender {

        private bool randomize = false;


        public void RectangleColor(Color accentColor, Color accentDark) {
            var grad = new LinearGradientBrush {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };
            grad.GradientStops.Add(new GradientStop(accentDark, 0.0));
            grad.GradientStops.Add(new GradientStop(accentColor, 1.0));
            grad.Freeze();
            FieldRectangle.Fill = grad;
        }

        private void RandomizeLattice() {
            _viewmodel.Randomize(true);
            randomize = false;
        }

        private void NewLattice() {
            _viewmodel.ChangeSize(_currentN, averageMagnetization);
            //reapply settings from previous model:
            _viewmodel.SetBoundary(_periodicBoundary);
            _viewmodel.ChangeTemperature(temperature);
            _viewmodel.ChangeField(magneticfield);
            _viewmodel.ChangeDynamic(AlgorithmText.Text);
            _viewmodel.ChangeCoupling(couplingconstant);
            _updateLatticeSizeText();
        }

        private void Boundary() {
            _viewmodel.SetBoundary(_periodicBoundary);
            BoundaryText.Text = _periodicBoundary ? "Periodic" : "Walled";

        }
    }
}
