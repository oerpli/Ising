using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;

using System.Windows.Controls;

using IsingModern.Ising;

namespace IsingModern.ViewModel {
    class IsingRenderModel : Image {
        private IsingModel model;
        private double viewsize = 600;// this should be dynamic, yo.
        private double cellSize;
        private Pen pen = new Pen() { Thickness = 0 };
        public int N { get { return model.N; } }


        //private VisualCollection children;
        //private DrawingVisual visual = new DrawingVisual();
        public WriteableBitmap wbmap;

        public IsingRenderModel(int n = 50, bool periodicBoundary = false) {
            pen.Freeze(); // increases performance of rendering
            model = new IsingModel(n);
            cellSize = viewsize / n; //casting once may be enough. idgaf though.
            this.SnapsToDevicePixels = false;
            this.Height = this.Width = viewsize; //only square lattices currently
            this.SetBoundary(periodicBoundary);



            //some options - whatever
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //this.Stretch = Stretch.None;
            //this.HorizontalAlignment = HorizontalAlignment.Left;
            //this.VerticalAlignment = VerticalAlignment.Top;

            //fixing the scaling at 96dpi works so far. "It's not as dumb as it looks" - Magnus Carlsen
            wbmap = new WriteableBitmap(600, 600, 96, 96, PixelFormats.Bgr24, null);
            this.Source = wbmap;

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

        private void DrawLattice() {
            wbmap.Lock();
            int size = (int)viewsize / N;
            Random rnd = new Random(DateTime.Now.Millisecond);
            int counter = 0;
            for(int y = 0; y < N; y += 1) {
                for(int x = 0; x < N; x += 1) {
                    DrawRectangle(size * x, size * y, size, size, model.Points[counter++].Color);
                }
            }
            wbmap.Unlock();
        }

        private void DrawRectangle(int left, int top, int width, int height, Color color) {
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



        public void ChangeSize(int newSize) {
            model = new IsingModel(newSize);
            cellSize = viewsize / newSize;
        }



        //this method is useful when only rendering up to 100 elements -- afterwards it's to slow
        //        protected override void OnRender(DrawingContext dc) {
        //#if DEBUG
        //            var sw = new Stopwatch();
        //            sw.Start();
        //#endif


        //            //int counter = 0;
        //            //var rect = new Rect(0 * cellSize, 0 * cellSize, cellSize, cellSize);
        //            //foreach(var x in model.Points) {
        //            //    if(counter % model.N != 0) rect.Offset(cellSize, 0);
        //            //    else if(counter > 0) rect.Offset(-(model.N - 1) * cellSize, cellSize);
        //            //    dc.DrawRectangle(x.Color, pen, rect);
        //            //    counter++;
        //            //}
        //#if DEBUG
        //            sw.Stop();
        //            Console.WriteLine(sw.ElapsedMilliseconds + "ms " + sw.ElapsedTicks);
        //#endif
        //        }


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
            this.InvalidateVisual();
        }

        internal void TopRight() {
            var p = model.Points[2 * N - 2];
            p.ToggleSpin();
            this.InvalidateVisual();

            //using(var dc = visual.RenderOpen()) {
            //    var rect = new Rect((N - 2) * cellSize, cellSize, cellSize, cellSize);
            //    dc.DrawRectangle(p.Color, pen, rect);
            //}
        }
    }
}
