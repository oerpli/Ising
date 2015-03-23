using IsingModern.Ising;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IsingModern.ViewModel {
    class IsingRenderModel : Canvas {
        private Image image = new Image();
        private Lattice model;
        private WriteableBitmap wbmap;
        private double cellSize, viewsize = 600;// this should be dynamic, yo.
        private int rectangleSize; //to convert mouseclicks
        public int N { get { return model.N; } }

        public IsingRenderModel(int n = 50, bool periodicBoundary = false) {
            this.Children.Add(image);
            this.SnapsToDevicePixels = false;
            image.Height = image.Width = viewsize; //only square lattices currently

            //some options - whatever
            {
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);
                //this.Stretch = Stretch.None;
                //this.HorizontalAlignment = HorizontalAlignment.Left;
                //this.VerticalAlignment = VerticalAlignment.Top;
            }
            //setting rendering up
            {
                //fixing the scaling at 96dpi works so far. "It's not as dumb as it looks" - Magnus Carlsen
                wbmap = new WriteableBitmap(600, 600, 96, 96, PixelFormats.Bgr24, null);
                image.Source = wbmap;
                rectangleSize = (int)viewsize / n;
                cellSize = viewsize / n;
            }
            //initializing model
            {
                model = new Lattice(n);
                this.SetBoundary(periodicBoundary);
            }

            //rendering:
            {
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
        }

        #region Manipulation
        public void ChangeSize(int newSize) {
            model = new Lattice(newSize);
            cellSize = viewsize / newSize;
            rectangleSize = (int)viewsize / newSize;
            DrawLattice();
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
            model.dynamic(p);
            DrawSpin(p);
        }

        internal void NextStep() {
            for(int i = 0; i < 5; i++)
                model.Sweep();
            DrawLattice();
        }

        internal void ChangeTemperature(double T) {
            model.Beta = Math.Max(1.0 / T, 0.000000001); //prevent division by zero.
        }

        internal void ChangeCoupling(double J) {
            if(Math.Abs(J) <= 0.009) J = 0;
            model.J = J;
        }
        internal void ChangeField(double h) {
            model.h = h;
        }


        internal void ChangeAccept(bool AcceptAlgorithm)
        {
            //TODO: Algorithmus auswählen 
        }


        #endregion

        #region Selection
        private Point mouseDownPoint;
        private Shape selectionShape = new Rectangle() {
            Opacity = 0.5,
            Stroke = new SolidColorBrush(Colors.DarkCyan),
            StrokeThickness = 5,
            //StrokeDashArray = new Stroke { 2,1};
        };
        private int mouseState = 0;
        private int[] coords = new int[4];
        private SolidColorBrush[] selectionColors = new SolidColorBrush[]{
            new SolidColorBrush(Colors.DeepSkyBlue),
            new SolidColorBrush(Colors.White),
            new SolidColorBrush(Colors.Gold),
        };

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            mouseDownPoint = e.GetPosition(this);
            if(mouseState != 0) {
                mouseState = 0;
                this.Children.Remove(selectionShape);
                for(int i = 0; i < 4; i++) coords[i] = 0;
                return;
            }
            if(e.LeftButton == MouseButtonState.Pressed) {
                mouseState = 1;
            } else if(e.RightButton == MouseButtonState.Pressed) {
                mouseState = 2;
            } else if(e.MiddleButton == MouseButtonState.Pressed) {
                mouseState = 3;
            } else {
                return;
            }
            // clean up mess left behind from previous selections
            {
                selectionShape.Width = selectionShape.Height = 0;
                this.Children.Remove(selectionShape);
            }
            selectionShape.Fill = selectionColors[mouseState - 1];
            this.Children.Add(selectionShape);
            Mouse.Capture(this);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if(mouseState != 0) {
                Point currentPoint = e.GetPosition(image);
                coords[0] = (int)(Math.Min(currentPoint.X, mouseDownPoint.X) + 0.5 * cellSize) / (int)cellSize; //x1
                coords[1] = (int)(Math.Max(currentPoint.X, mouseDownPoint.X) + 0.5 * cellSize) / (int)cellSize; //x2
                coords[2] = (int)(Math.Min(currentPoint.Y, mouseDownPoint.Y) + 0.5 * cellSize) / (int)cellSize; //y1
                coords[3] = (int)(Math.Max(currentPoint.Y, mouseDownPoint.Y) + 0.5 * cellSize) / (int)cellSize; //y2

                for(int i = 0; i < 4; i++) {
                    coords[i] = Math.Min(N, Math.Max(0, coords[i]));
                }

                double w = cellSize * (coords[1] - coords[0]);
                double h = cellSize * (coords[3] - coords[2]);
                double l = cellSize * coords[0];
                double t = cellSize * coords[2];

                selectionShape.Width = w;
                selectionShape.Height = h;
                Canvas.SetLeft(selectionShape, l);
                Canvas.SetTop(selectionShape, t);

            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if(mouseState != 0) {
                int maxx = coords[1], maxy = coords[3];
                wbmap.Lock();
                for(int x = coords[0]; x < maxx; x++) {
                    for(int y = coords[2]; y < maxy; y++) {
                        MouseAction(model.Spins[y * N + x]);
                    }
                }
                //if no area is selected change the Spin under the mouse
                if(coords[0] == coords[1] && coords[2] == coords[3]) {
                    var pos = e.GetPosition(image);
                    int x = (int)pos.X / (int)cellSize;
                    int y = (int)pos.Y / (int)cellSize;
                    MouseAction(model.Spins[y * N + x]);
                }
                wbmap.Unlock();
            }
            mouseState = 0;
            for(int i = 0; i < 4; i++) coords[i] = 0;
            this.Children.Remove(selectionShape);
            Mouse.Capture(null);
        }

        private void MouseAction(Spin spin) {
            if(mouseState == 1)
                spin.SetSpin();
            if(mouseState == 2)
                spin.ToggleBoundary(true);
            if(mouseState == 3)
                spin.ToggleSpin();
            DrawSpin(spin);
        }

        #endregion

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





        internal void ChangeAccept(string p)
        {
            model.accept = model.accepts[p]; 
        }
    }
}


