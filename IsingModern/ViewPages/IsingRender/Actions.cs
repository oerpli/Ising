using System.Windows;
using System.Windows.Media;

namespace IsingModern.ViewPages {
    public partial class IsingRender {

        private bool randomize = false;
        private bool boundary = false;
        private bool newlattice = false;


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
