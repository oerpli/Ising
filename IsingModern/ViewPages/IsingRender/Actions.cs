using System;
using System.Windows;
using System.Windows.Media;

namespace IsingModern.ViewPages {
    public partial class IsingRender {

        private void RandomizeLattice() {
            _viewmodel.Randomize(true);
        }

        private void NewLattice() {
            _viewmodel.ScaleSize(_currentN, averageMagnetization);
            _updateLatticeSizeText(); //update text
        }

        private void Boundary() {
            _viewmodel.SetBoundary(_periodicBoundary);
            BoundaryText.Text = _periodicBoundary ? "Periodic" : "Walled";
        }

        private void ThreadedAction(Action action) {
            sem.WaitOne();
            action();
            sem.Release();
        }
    }
}
