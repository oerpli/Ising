using System;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;

using IsingModern.Ising;

namespace IsingModern.ViewModel {
    class IsingRenderModel : FrameworkElement {
        private IsingModel model;
        private double viewsize = 600;// this should be dynamic, yo.
        private double cellSize;
        private Pen pen = new Pen() { Thickness = 0 };
        public int N { get { return model.N; } }

        public IsingRenderModel(int n = 50, bool periodicBoundary = false) {
            pen.Freeze(); // increases performance of rendering
            model = new IsingModel(n);
            cellSize = viewsize / n; //casting once may be enough. idgaf though.
            this.SnapsToDevicePixels = false;
            this.Height = this.Width = viewsize; //only square lattices currently
            this.SetBoundary(periodicBoundary);
        }

        public void ChangeSize(int newSize) {
            model = new IsingModel(newSize);
            cellSize = viewsize / newSize;
        }


        protected override void OnRender(DrawingContext dc) {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            int counter = 0;
            var rect = new Rect(0 * cellSize, 0 * cellSize, cellSize, cellSize);
            foreach(var x in model.Points) {
                if(counter % model.N != 0) rect.Offset(cellSize, 0);
                else if(counter > 0) rect.Offset(-(model.N - 1) * cellSize, cellSize);
                dc.DrawRectangle(x.Color, pen, rect);
                counter++;
            }
#if DEBUG
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms " + sw.ElapsedTicks);
#endif
        }


        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            var pos = e.GetPosition(this);
            int x = (int)pos.X;
            int y = (int)pos.Y;
            x /= (int)cellSize;
            y /= (int)cellSize;

            var p = model.Points[model.N * y + x];
            if(e.LeftButton == MouseButtonState.Pressed) {
                p.ToggleSpin();
            }
            if(e.RightButton == MouseButtonState.Pressed) {
                p.ToggleBoundary(true);
            }
            //var rect = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);
            //_dc.DrawRectangle(p.Color, pen, rect);
            this.InvalidateVisual();
        }

        internal void SetBoundary(bool PeriodicBoundary) {
            model.SetBoundary(PeriodicBoundary);
            this.InvalidateVisual();
        }

        internal void Randomize() {
            model.Randomize();
        }

        internal void TopRight() {
            model.Points[2 * N - 2].ToggleSpin();
        }
    }
}
