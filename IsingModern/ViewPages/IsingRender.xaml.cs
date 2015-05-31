using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using IsingModern.ViewModel;
using OxyPlot;
using OxyPlot.Annotations;
using LineAnnotation = OxyPlot.Wpf.LineAnnotation;


namespace IsingModern.ViewPages {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>

    public partial class IsingRender : UserControl {
        public static IsingRender Current;
        private IsingRenderModel _viewmodel;

        private bool _periodicBoundary = true;
        private bool _ferromagnetic = true;
        private bool _singleFlip = true;
        private double _tempMax = 5.0;
        private double _magnMax = 0.5;

        private int sliderMin = 0, sliderMax = 3;

        private int CurrentN {
            get { return 25 * (1 << (int)SizeSlider.Value); }
        }
        private const int MaximalN = 200, MinimalN = 25; //both should divide Pixels. 
        public const int Pixels = 800;



        #region Initialization

        public IsingRender() {
            InitializeComponent();
            _viewmodel = new IsingRenderModel(CurrentN, _periodicBoundary);
            Plotinit(); //test
            Current = this;
            ModelParentElement.Children.Add(_viewmodel);
            SizeText.Text = CurrentN.ToString();
            SizeSlider.Maximum = sliderMax;
            Reset();

            _worker = worker_Init();
            StatusText.Text = "0";
            _worker.RunWorkerAsync();
            run.WaitOne();
        }

        #endregion

        #region LatticeManipulation
        private void maingrid_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.N) {
                Start_Click(null, null);
                e.Handled = true;
            }
        }


        private void RandomizeClick(object sender, RoutedEventArgs e) {
            ThreadedAction(RandomizeLattice);
            e.Handled = true;
        }

        private void Reset_Click(object sender, RoutedEventArgs e) {
            Reset();
        }

        private void ToggleBoundary_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) _periodicBoundary = !_periodicBoundary;
            ThreadedAction(Boundary);
        }

        private void Coupling_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) _ferromagnetic = !_ferromagnetic;
            ChangeCoupling();
            if(e != null) e.Handled = true;
        }

        private void Algorithm_Click(object sender, RoutedEventArgs e) {
            if(sender != null) _singleFlip = !_singleFlip;
            ChangeAlgorithm();
            e.Handled = true;
        }

        #endregion

        #region Drag&Drop
        private const double SnappingTolerance = 0.04;
        private const double TempMax = 5, FieldMax = 0.5;
        private const double ThumbRadius = 5;

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e) {
            if(!_fixedTemperature)
                Canvas.SetLeft(FieldThumb, Math.Max(-ThumbRadius, Math.Min(TempMagField.ActualWidth - ThumbRadius, Canvas.GetLeft(FieldThumb) + e.HorizontalChange)));
            if(!_fixedMagnfield)
                Canvas.SetTop(FieldThumb, Math.Max(-ThumbRadius, Math.Min(TempMagField.ActualHeight - ThumbRadius, Canvas.GetTop(FieldThumb) + e.VerticalChange)));
            UpdateParameters(Canvas.GetLeft(FieldThumb), Canvas.GetTop(FieldThumb));
            e.Handled = true;
        }



        private double magneticfield = 0.0;
        private double temperature = 1.0;
        private void UpdateParameters(double x, double y) {
            var temp = (x + ThumbRadius) / (TempMagField.ActualWidth) * TempMax;
            var field = FieldMax - (y + ThumbRadius) / (TempMagField.ActualHeight) * (2 * FieldMax);
            if(_snapping && Math.Abs(field) < SnappingTolerance) { //snapping to 0 field
                field = 0;
                UpdateThumb(temp, field);
            }
            _viewmodel.ChangeTemperature(temp);
            _viewmodel.ChangeField(field);
            temperature = temp;
            magneticfield = field;
            TemperatureTextBox.Text = temp.ToString("0.00");
            MagnFieldTextBox.Text = field.ToString("0.00");
        }

        private void UpdateThumb(double temp, double field) {
            var w = TempMagField.ActualWidth;
            var h = TempMagField.ActualHeight;

            if(w <= 0 || h <= 0) {
                w = TempMagField.Width;
                h = TempMagField.Height;
            }

            var x = temp * (w) / TempMax - ThumbRadius;
            var y = -(field - FieldMax) * (h) - ThumbRadius;
            Canvas.SetLeft(FieldThumb, x);
            Canvas.SetTop(FieldThumb, y);
        }

        private bool _fixedTemperature = false;
        private bool _fixedMagnfield = false;
        private bool _snapping = true;
        private void FixTemperature_Checked(object sender, RoutedEventArgs e) {
            _fixedTemperature = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }
        private void FixMagneticField_Checked(object sender, RoutedEventArgs e) {
            _fixedMagnfield = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }
        private void Toggle_Snapping(object sender, RoutedEventArgs e) {
            _snapping = ((CheckBox)sender).IsChecked ?? false;
        }

        private void Temperature_TextChanged(object sender, KeyEventArgs e) //TextChangedEventArgs e)
        {
            if(e.Key == Key.Enter) {
                double temp;
                if(Double.TryParse(TemperatureTextBox.Text, out temp)) {
                    temp = Math.Min(_tempMax, Math.Max(temp, 0));
                    _viewmodel.ChangeTemperature(temp);
                    temperature = temp;
                    TemperatureTextBox.Text = temperature.ToString("0.00");
                } else {
                    TemperatureTextBox.Text = temperature.ToString("0.00");
                }
                UpdateThumb(temperature, magneticfield);
            }

        }

        private void MagnField_TextChanged(object sender, KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                double magn;
                if(Double.TryParse(MagnFieldTextBox.Text, out magn)) {
                    magn = Math.Min(_magnMax, Math.Max(-_magnMax, magn));
                    _viewmodel.ChangeField(magn);
                    magneticfield = magn;
                    MagnFieldTextBox.Text = magn.ToString("0.00");
                } else {
                    MagnFieldTextBox.Text = magneticfield.ToString("0.00");
                }
                UpdateThumb(temperature, magneticfield);
            }

        }

        #endregion

        #region LatticeSize
        private void LatticeSize_Click(object sender, RoutedEventArgs e) {
            ThreadedAction(NewLattice);
            e.Handled = true;
        }


        private void SizeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(SizeText != null) SizeText.Text = CurrentN.ToString();

        }
        private void SizeSliderDragCompleted(object sender, DragCompletedEventArgs e) {
            ThreadedAction(NewLattice);
            e.Handled = true;
        }

        private void LatticeSize_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Left) {
                _changeLatticeSize(false);
                e.Handled = true;
            } else if(e.Key == Key.Right) {
                _changeLatticeSize(true);
                e.Handled = true;
            }
        }

        private void LatticeSize_MouseWheel(object sender, MouseWheelEventArgs e) {
            _changeLatticeSize(e.Delta > 0);
            e.Handled = true;
        }

        //if using scrollwheel increase/decrase to next divisor of Pixel (800) (to avoid ugly rendering) - can be finetuned with left/right keys if necessary
        private void _changeLatticeSize(bool bigger) {
            SizeSlider.Value += bigger ? 1 : -1;
            ThreadedAction(NewLattice);
        }


        #endregion

        #region Rendering
        static internal void RefreshRender() {
            Current._viewmodel.Refresh();
        }

        #endregion

        #region Plotting

        private LineAnnotation line;
        public string Title { get; private set; }

        public IList<DataPoint> EnergyPoints { get; private set; }
        public IList<DataPoint> MagnetizationPoints { get; private set; }


        private void Plotinit() {
            line = new LineAnnotation() { Type = LineAnnotationType.Vertical, X = -10, StrokeThickness = 10, LineStyle = LineStyle.Solid, Color = lineColors[0] };

            EnergyPlot.ItemsSource = EnergyPoints = new List<DataPoint>();
            MagnetizationPlot.ItemsSource = MagnetizationPoints = new List<DataPoint>();
            EnergyPlot.Color = Colors.Red;
            MagnetizationPlot.Color = Colors.DeepSkyBlue;
            Plot.IsLegendVisible = true;

            EnergyPlot.TrackerFormatString = MagnetizationPlot.TrackerFormatString = "{0}: {4:0.00}";
            //setup tick/background etc color (matching the dark theme)
            SwitchTheme(true);
            for(int i = 0; i < _plotDataMax; i++) {
                EnergyPoints.Add(new DataPoint(i, 0));
                MagnetizationPoints.Add(new DataPoint(i, 0));
            }
            Plot.Annotations.Add(line);
        }

        #endregion

        #region Threading
        private bool _running = false;
        private BackgroundWorker _worker;
        private void Start_Click(object sender, RoutedEventArgs e) {
            _running = !_running;
            ToggleSimulation.Content = _running ? "Stop" : "Start";
            if(_running)
                run.Release();
            else
                run.WaitOne();
        }

        private BackgroundWorker worker_Init() {
            var worker = new BackgroundWorker() { WorkerReportsProgress = true };
            worker.DoWork += worker_Work;
            worker.ProgressChanged += worker_Progress;
            return worker;
        }

        private void worker_Work(object sender, DoWorkEventArgs e) {
            int i = 0;
            while(true) {
                run.WaitOne();
                sem.WaitOne();
                var data = _viewmodel.Sweep();
                var backgroundWorker = sender as BackgroundWorker;
                if(backgroundWorker != null) backgroundWorker.ReportProgress(i++, data);
                sem.Release();
                run.Release();
            }
        }

        long _timerefresh;

        private int _plotDataMax = 500;
        private int _plotIndex = 0;

        private double averageMagnetization = 1;
        private void worker_Progress(object sender, ProgressChangedEventArgs e) {
            long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if(time - _timerefresh > 40) {
                StatusText.Text = e.ProgressPercentage.ToString();
                var data = (Tuple<double, double>)e.UserState; //not checking for null due to performance reasons.
                EnergyPoints[_plotIndex] = new DataPoint(_plotIndex, data.Item1);
                MagnetizationPoints[_plotIndex] = new DataPoint(_plotIndex, -data.Item2);
                _plotIndex = (_plotIndex + 1) % _plotDataMax;
                line.X = _plotIndex;
                _viewmodel.Refresh();
                _timerefresh = time;
                Plot.InvalidatePlot();
            }
        }

        #endregion

    }
}
