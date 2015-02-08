using IsingModern.Ising;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IsingModern.ViewModel {
    class IsingRenderModel : Image {
        private IsingModel model;
        private WriteableBitmap wbmap;
        private double cellSize,viewsize = 600;// this should be dynamic, yo.
        private int rectangleSize;
        public int N { get { return model.N; } }

        public IsingRenderModel(int n = 50, bool periodicBoundary = false) {
            this.SnapsToDevicePixels = false;
            this.Height = this.Width = viewsize; //only square lattices currently


            {
                //some options - whatever
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
                //this.Stretch = Stretch.None;
                //this.HorizontalAlignment = HorizontalAlignment.Left;
                //this.VerticalAlignment = VerticalAlignment.Top;
            }
            //setting rendering up
            {
                //fixing the scaling at 96dpi works so far. "It's not as dumb as it looks" - Magnus Carlsen
                wbmap = new WriteableBitmap(600, 600, 96, 96, PixelFormats.Bgr24, null);
                this.Source = wbmap;
                rectangleSize = (int)viewsize / n;
                cellSize = viewsize / n; //casting once may be enough. idgaf though.
            }
            //initializing model
            {
                model = new IsingModel(n);
                this.SetBoundary(periodicBoundary);
            }

            //rendering:
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            DrawLattice();
#if DEBUG
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + "ms " + sw.ElapsedTicks);
#endif
        }

        #region Rendering
        private void DrawLattice() {
            wbmap.Lock();
            Random rnd = new Random(DateTime.Now.Millisecond);
            int counter = 0;
            for(int y = 0; y < N; y++) {
                for(int x = 0; x < N; x++) {
                    DrawSpin(x, y, model.Spins[counter++].Color);
                }
            }
            wbmap.Unlock();
        }

        private void DrawSpin(int left, int top, Color color) {
            left *= rectangleSize;
            top *= rectangleSize;
            int width = rectangleSize, height = rectangleSize;
            // Compute the pixel's color
            int colorData = color.R << 16; // R
            colorData |= color.G << 8; // G
            colorData |= color.B << 0; // B
            int bpp = wbmap.Format.BitsPerPixel / 8;

            unsafe {
                for(int y = 0; y < height; y++) {
                    // Get a pointer to the back buffer
                    int pBackBuffer = (int)wbmap.BackBuffer;

                    // Find the address of the pixel to draw
                    pBackBuffer += (top + y) * wbmap.BackBufferStride;
                    pBackBuffer += left * bpp;

                    for(int x = 0; x < width; x++) {
                        // Assign the color data to the pixel
                        *((int*)pBackBuffer) = colorData;

                        // Increment the address of the pixel to draw
                        pBackBuffer += bpp;
                    }
                }
            }
            wbmap.AddDirtyRect(new Int32Rect(left, top, width, height));
        }

        private void DrawSpin(Spin p) {
            int left = p.Index % N;
            int top = p.Index / N;
            wbmap.Lock();
            DrawSpin(left, top, p.Color);
            wbmap.Unlock();
        }

        internal void Refresh() {
            DrawLattice();
        }


        #endregion

        #region Manipulation
        public void ChangeSize(int newSize) {
            model = new IsingModel(newSize);
            cellSize = viewsize / newSize;
            rectangleSize = (int)viewsize / newSize;
            DrawLattice();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            var pos = e.GetPosition(this);
            int x = (int)pos.X;
            int y = (int)pos.Y;
            x /= (int)cellSize;
            y /= (int)cellSize;

            Console.WriteLine(x + " " + y);
            var p = model.Spins[model.N * y + x];
            if(e.LeftButton == MouseButtonState.Pressed) {
                p.ToggleSpin();
            }
            if(e.RightButton == MouseButtonState.Pressed) {
                p.ToggleBoundary(true);
            }
            DrawSpin(p);
            //var rect = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);
            //_dc.DrawRectangle(p.Color, pen, rect);
        }

        internal void SetBoundary(bool PeriodicBoundary) {
            model.SetBoundary(PeriodicBoundary);
            DrawLattice();
        }

        internal void Randomize() {
            model.Randomize();
            DrawLattice();
        }

        internal void ToggleTopRight() {
            var p = model.Spins[2 * N - 2];
            p.ToggleSpin();
            DrawSpin(p);
        }

        #endregion
    }
}
