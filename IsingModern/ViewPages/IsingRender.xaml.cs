using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using IsingModern.ViewModel;
using OxyPlot;

namespace IsingModern.ViewPages {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class IsingRender : UserControl {
        static IsingRender Current;
        private IsingRenderModel viewmodel;

        private bool PeriodicBoundary = false;
        private bool Ferromagnetic = true;
        private bool Metropolis = true;

        private const int maximalN = 200, minimalN = 3; //both should divide 600. 
        private int currentN = 20;



        #region Initialization


        public IsingRender() {
            viewmodel = new IsingRenderModel(currentN, PeriodicBoundary);
            InitializeComponent();
            plotinit(); //test
            Current = this;
            BoundaryText.Text = PeriodicBoundary ? "Periodic" : "Walled";
            CouplingText.Text = Ferromagnetic ? "Ferromagnetic" : "Anti-Ferromagnetic";
            AlgorithmText.Text = Metropolis ? "Metropolis" : "Glauber";
            UpdateThumb(1.0, 0.0);
            TemperatureTextBox.Text = "1.0";
            MagnFieldTextBox.Text = "0.0";
            ModelParentElement.Children.Add(viewmodel);
            LatticeSizeInput.Text = currentN.ToString();
        }

        private void NewLattice(int n) {
            viewmodel.ChangeSize(n);
            viewmodel.SetBoundary(PeriodicBoundary);
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
            viewmodel.Randomize();
            e.Handled = true;
        }

        private void ToggleBoundary_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) PeriodicBoundary = !PeriodicBoundary;
            viewmodel.SetBoundary(PeriodicBoundary);
            BoundaryText.Text = PeriodicBoundary ? "Periodic" : "Walled";
        }

        private void Time_Click(object sender, RoutedEventArgs e) {
            viewmodel.Sweep();
        }

        private void TemperatureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            viewmodel.ChangeTemperature(e.NewValue);
            e.Handled = true;
        }

        private void CouplingConstant_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            viewmodel.ChangeCoupling(e.NewValue);
            e.Handled = true;
        }

        private void MagneticField_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            viewmodel.ChangeField(e.NewValue);
            e.Handled = true;
        }

        //private void Temperature_MouseWheel(object sender, MouseWheelEventArgs e) {
        //    Temperature.Value += Math.Sign(e.Delta) * 0.01;
        //    e.Handled = true;
        //}

        /*private void CouplingConstant_MouseWheel(object sender, MouseWheelEventArgs e) {
            CouplingConstant.Value += Math.Sign(e.Delta) * 0.01;
            e.Handled = true;
        }*/

        //private void MagneticField_MouseWheel(object sender, MouseWheelEventArgs e) {
        //    MagneticField.Value += Math.Sign(e.Delta) * 0.009;
        //}

        private void Stop_Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
        }

        private void Coupling_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) Ferromagnetic = !Ferromagnetic;
            viewmodel.ChangeCoupling(Ferromagnetic ? 1.0 : -1.0);
            CouplingText.Text = Ferromagnetic ? "Ferromagnetic" : "Anti-Ferromagnetic";
        }

        private void Algorithm_Click(object sender, RoutedEventArgs e) {
            if(sender != null) Metropolis = !Metropolis;

            AlgorithmText.Text = Metropolis ? "Metropolis" : "Glauber";
            viewmodel.ChangeAccept(AlgorithmText.Text);
        }

        #endregion

        #region Drag&Drop
        private const double snappingTolerance = 0.04;
        private const double tempMax = 5, fieldMax = 0.5;
        private const double thumbRadius = 5;

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            if(!fixed_temperature)
                Canvas.SetLeft(fieldThumb, Math.Max(-thumbRadius, Math.Min(TempMagField.ActualWidth - thumbRadius, Canvas.GetLeft(fieldThumb) + e.HorizontalChange)));
            if(!fixed_magnfield)
                Canvas.SetTop(fieldThumb, Math.Max(-thumbRadius, Math.Min(TempMagField.ActualHeight - thumbRadius, Canvas.GetTop(fieldThumb) + e.VerticalChange)));
            UpdateParameters(Canvas.GetLeft(fieldThumb), Canvas.GetTop(fieldThumb));
            e.Handled = true;
        }

        private void UpdateParameters(double x, double y) {
            var temp = (x + thumbRadius) / (TempMagField.ActualWidth) * tempMax;
            var field = fieldMax - (y + thumbRadius) / (TempMagField.ActualHeight) * (2 * fieldMax);
            if(snapping && Math.Abs(field) < snappingTolerance) { //snapping to 0 field
                field = 0;
                UpdateThumb(temp, field);
            }
            viewmodel.ChangeTemperature(temp);
            viewmodel.ChangeField(field);
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

            var x = temp * (w) / tempMax - thumbRadius;
            var y = -(field - fieldMax) * (h) - thumbRadius;
            Canvas.SetLeft(fieldThumb, x);
            Canvas.SetTop(fieldThumb, y);
        }

        private bool fixed_temperature = false;
        private bool fixed_magnfield = false;
        private bool snapping = true;
        private void FixTemperature_Checked(object sender, RoutedEventArgs e) {
            fixed_temperature = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }
        private void FixMagneticField_Checked(object sender, RoutedEventArgs e) {
            fixed_magnfield = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }
        private void Toggle_Snapping(object sender, RoutedEventArgs e) {
            snapping = ((CheckBox)sender).IsChecked ?? false;
        }

        #endregion

        #region LatticeSize
        private void LatticeSize_Click(object sender, RoutedEventArgs e) {
            NewLattice(currentN);
            _updateLatticeSizeText();
            e.Handled = true;
        }
        private void LatticeSize_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Left) {
                _changeLatticeSize(-1);
                e.Handled = true;
            } else if(e.Key == Key.Right) {
                _changeLatticeSize(1);
                e.Handled = true;
            }
        }

        private void LatticeSize_MouseWheel(object sender, MouseWheelEventArgs e) {
            _changeLatticeSize(e.Delta > 0 ? 1 : -1, true);
            e.Handled = true;
        }

        //if using scrollwheel increase/decrase to next divisor of 600 (to avoid ugly rendering) - can be finetuned with left/right keys if necessary
        private void _changeLatticeSize(int diff, bool mouse = false) {
            do {
                Console.WriteLine(600 % currentN);
                currentN = Math.Min(maximalN, Math.Max(minimalN, currentN + diff));
            } while(mouse && 600 % currentN != 0);
            _updateLatticeSizeText();

        }

        private void _updateLatticeSizeText() {
            LatticeSizeInput.Text = (currentN != viewmodel.N ? "(" + viewmodel.N + ") " : "") + currentN.ToString();
        }
        #endregion

        #region Rendering
        static internal void RefreshRender() {
            Current.viewmodel.Refresh();
        }

        #endregion

        #region Plotting
        public string Title { get; private set; }

        public IList<DataPoint> EnergyPoints { get; private set; }
        public IList<DataPoint> MagnetizationPoints { get; private set; }

        private void plotinit() {
            EnergyPoints = new List<DataPoint>();
            MagnetizationPoints = new List<DataPoint>();
            EnergyPlot.ItemsSource = EnergyPoints;
            MagnetizationPlot.ItemsSource = MagnetizationPoints;
            Plot.IsLegendVisible = true;
            Plot.LegendBackground = System.Windows.Media.Colors.AliceBlue;
        }

        #endregion

        #region Threading
        private bool running = false;
        private BackgroundWorker worker;

        private void Start_Click(object sender, RoutedEventArgs e) {
            running = !running;
            if(running) {
                worker = worker_Init();
                StatusText.Text = "0";
                worker.RunWorkerAsync();
            }
        }

        private BackgroundWorker worker_Init() {
            var worker = new BackgroundWorker() { WorkerReportsProgress = true };
            worker.DoWork += worker_Work;
            worker.ProgressChanged += worker_Progress;
            worker.RunWorkerCompleted += worker_Completed;
            return worker;
        }

        private void worker_Work(object sender, DoWorkEventArgs e) {
            int i = 0;
            while(running) {
                var data = viewmodel.Sweep();
                (sender as BackgroundWorker).ReportProgress(i++, data);
            }
        }

        long timerefresh;

        private bool overwritePlot = false;
        private int plotDataMax = 200;
        private int plotIndex = 0;

        private void worker_Progress(object sender, ProgressChangedEventArgs e) {
            long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if(time - timerefresh > 40) {
                StatusText.Text = e.ProgressPercentage.ToString();
                var data = (Tuple<double, double>)e.UserState; //not checking for null due to performance reasons.
                if(overwritePlot) {
                    EnergyPoints[plotIndex] = new DataPoint(plotIndex, data.Item1);
                    MagnetizationPoints[plotIndex] = new DataPoint(plotIndex, data.Item2);
                    plotIndex++;
                    if(plotIndex == plotDataMax) {
                        EnergyPlot.ItemsSource = EnergyPoints;
                        MagnetizationPlot.ItemsSource = MagnetizationPoints;
                        plotIndex = 0;
                    }
                } else {
                    EnergyPoints.Add(new DataPoint(plotIndex, data.Item1));
                    MagnetizationPoints.Add(new DataPoint(plotIndex, data.Item2));
                    if(EnergyPoints.Count >= plotDataMax) {
                        overwritePlot = true;
                        plotIndex = 0;
                    }
                    plotIndex++;
                }
                viewmodel.Refresh();
                timerefresh = time;
                Plot.InvalidatePlot();
            }
        }

        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e) {
            viewmodel.Refresh();
        }

        #endregion



    }
}
