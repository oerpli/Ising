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
            _viewmodel.ScaleSize(_currentN, averageMagnetization);
            //reapply settings from previous model:
            _updateLatticeSizeText();
        }

        private void Boundary() {
            _viewmodel.SetBoundary(_periodicBoundary);
            BoundaryText.Text = _periodicBoundary ? "Periodic" : "Walled";
        }
    }
}
