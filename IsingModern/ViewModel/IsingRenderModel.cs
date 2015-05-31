using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IsingModern.Model;
using System.Threading;
using System.Diagnostics;
using IsingModern.ViewPages;

namespace IsingModern.ViewModel {
    class IsingRenderModel : Canvas {
        private readonly Image image = new Image();
        private Lattice model;
        private readonly WriteableBitmap _wbmap;
        private double cellSize;// this should be dynamic, yo.
        private const double viewsize = IsingRender.Pixels; // this should be dynamic, yo.
        private int _rectangleSize; //to convert mouseclicks
        public int N { get { return model.N; } }

        public IsingRenderModel(int n, bool periodicBoundary = false) {
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
                _wbmap = new WriteableBitmap(IsingRender.Pixels, IsingRender.Pixels, 96, 96, PixelFormats.Bgr24, null);
                image.Source = _wbmap;
                _rectangleSize = (int)viewsize / n;
                cellSize = viewsize / n;
            }
            //initializing model
            {
                model = new Lattice(n, 0);
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

        public void ScaleSize(int newSize) {
            while(N != newSize) {
                model.ScaleLattice(newSize < N);
            }
            model.UpdateStats();
            cellSize = viewsize / newSize;
            _rectangleSize = (int)viewsize / newSize;
            DrawLattice();
        }

        internal void SetBoundary(bool periodicBoundary) {
            model.SetBoundary(periodicBoundary);
            DrawLattice();
        }

        internal void Randomize(bool redraw = false) {
            model.Randomize();
            if(redraw) {
                DrawLattice();
            }
        }

        internal Tuple<double, double> Sweep() {

            return model.Sweep();
        }

        internal void ChangeTemperature(double T) {
            model.Beta = Math.Max(1.0 / T, 0.000000001); //prevent division by zero.
        }

        internal void ChangeCoupling(double j) {
            model.Coupling = j;
        }
        internal void ChangeField(double h) {
            model.Field = h;
        }

        #endregion

        #region Selection
        private Point mouseDownPoint;
        private readonly Shape _selectionShape = new Rectangle() {
            Opacity = 0.5,
            Stroke = new SolidColorBrush(Colors.DarkCyan),
            StrokeThickness = 5,
            //StrokeDashArray = new Stroke { 2,1};
        };
        private int _mouseState = 0;
        private readonly int[] coords = new int[4];
        private readonly SolidColorBrush[] _selectionColors = new SolidColorBrush[]{
            new SolidColorBrush(Colors.DeepSkyBlue),
            new SolidColorBrush(Colors.White),
            new SolidColorBrush(Colors.Gold),
        };

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            mouseDownPoint = e.GetPosition(this);
            if(_mouseState != 0) {
                _mouseState = 0;
                this.Children.Remove(_selectionShape);
                for(int i = 0; i < 4; i++) coords[i] = 0;
                return;
            }
            if(e.LeftButton == MouseButtonState.Pressed) {
                _mouseState = 1;
            } else if(e.RightButton == MouseButtonState.Pressed) {
                _mouseState = 2;
            } else if(e.MiddleButton == MouseButtonState.Pressed) {
                _mouseState = 3;
            } else {
                return;
            }
            // clean up mess left behind from previous selections
            {
                _selectionShape.Width = _selectionShape.Height = 0;
                this.Children.Remove(_selectionShape);
            }
            _selectionShape.Fill = _selectionColors[_mouseState - 1];
            this.Children.Add(_selectionShape);
            Mouse.Capture(this);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if(_mouseState != 0) {
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

                _selectionShape.Width = w;
                _selectionShape.Height = h;
                Canvas.SetLeft(_selectionShape, l);
                Canvas.SetTop(_selectionShape, t);

            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if(_mouseState != 0) {
                int maxx = coords[1], maxy = coords[3];
                _wbmap.Lock();
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
                _wbmap.Unlock();
            }
            _mouseState = 0;
            for(int i = 0; i < 4; i++) coords[i] = 0;
            this.Children.Remove(_selectionShape);
            Mouse.Capture(null);
            model.UpdateStats();
        }

        private void MouseAction(Spin spin) {
            if(_mouseState == 1)
                spin.Value = -1;
            if(_mouseState == 2)
                spin.ToggleBoundary(true);
            if(_mouseState == 3)
                spin.Value = 1;
            DrawSpin(spin);
        }

        #endregion

        #region Rendering
        private void DrawLattice() {
            _wbmap.Lock();
            int counter = 0;
            for(int y = 0; y < N; y++) {
                for(int x = 0; x < N; x++) {
                    DrawSpin(x, y, model.Spins[counter++].Color);
                }
            }
            _wbmap.Unlock();
        }

        private void DrawSpin(int left, int top, Color color) {
            left *= _rectangleSize;
            top *= _rectangleSize;
            int width = _rectangleSize, height = _rectangleSize;
            // Compute the pixel's color
            int colorData = color.R << 16; // R
            colorData |= color.G << 8; // G
            colorData |= color.B << 0; // B
            int bpp = _wbmap.Format.BitsPerPixel / 8;

            unsafe {
                for(int y = 0; y < height; y++) {
                    // Get a pointer to the back buffer
                    long pBackBuffer = (long)_wbmap.BackBuffer;

                    // Find the address of the pixel to draw
                    pBackBuffer += (top + y) * _wbmap.BackBufferStride;
                    pBackBuffer += left * bpp;

                    for(int x = 0; x < width; x++) {
                        // Assign the color data to the pixel
                        *((int*)pBackBuffer) = colorData;
                        // Increment the address of the pixel to draw
                        pBackBuffer += bpp;
                    }
                }
            }
            _wbmap.AddDirtyRect(new Int32Rect(left, top, width, height));
        }

        private void DrawSpin(Spin p) {
            int left = p.Index % N;
            int top = p.Index / N;
            _wbmap.Lock();
            DrawSpin(left, top, p.Color);
            _wbmap.Unlock();
        }

        public void Refresh() {
            DrawLattice();
        }


        #endregion





        internal void ChangeDynamic(string p) {
            model.Dynamic = model.Dynamics[p];
        }
        internal void ChangeAccept(string p) {
            model.Accept = model.Accepts[p];
        }


    }
}


