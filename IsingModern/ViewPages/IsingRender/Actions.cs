using System;
using System.Threading;

namespace IsingModern.ViewPages {
    public partial class IsingRender {
        private readonly Semaphore sem = new Semaphore(1, 5);

        private void RandomizeLattice() {
            _viewmodel.Randomize(true);
        }

        private void NewLattice() {
            _viewmodel.ScaleSize(_currentN, averageMagnetization);
            //_updateLatticeSizeText(); //update text
        }

        private void Boundary() {
            _viewmodel.SetBoundary(_periodicBoundary);
            BoundaryText.Text = _periodicBoundary ? "Periodic" : "Walled";
        }

        private void Reset()
        {
           
        }

        private void ThreadedAction(Action action) {
            sem.WaitOne();
            action();
            sem.Release();
        }
    }
}
