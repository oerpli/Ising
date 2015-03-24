using IsingModern.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Wpf;


using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace IsingModern.Render {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class IsingRender : UserControl {
        static IsingRender Current;
        private IsingRenderModel viewmodel;

        private bool PeriodicBoundary = false;
        private bool Ferromagnetic = true;
        private bool Metropolis = true;
        private int rndCounter = 0;

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
            TemperatureTextBox.Text = "1.0";
            MagnFieldTextBox.Text = "0.0";
            modelParentElement.Children.Add(viewmodel);
            LatticeSizeInput.Text = currentN.ToString();
        }

        private void NewLattice(int n) {
            viewmodel.ChangeSize(n);
            viewmodel.SetBoundary(PeriodicBoundary);
        }

        #endregion

        #region LatticeManipulation
        private void maingrid_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.R) {
                RandomizeClick(null, null);
                e.Handled = true;
            }
            if(e.Key == Key.N) {
                Start_Click(null, null);
                e.Handled = true;
            }
        }

        private void RandomizeClick(object sender, RoutedEventArgs e) {
            viewmodel.Randomize();
            StatusText.Text = "Count: " + (++rndCounter).ToString();
            e.Handled = true;
        }

        private void ToggleBoundary_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) PeriodicBoundary = !PeriodicBoundary;
            viewmodel.SetBoundary(PeriodicBoundary);
            BoundaryText.Text = PeriodicBoundary ? "Periodic" : "Walled";
        }

        /* private void TopRight_Click(object sender, RoutedEventArgs e) {
             viewmodel.ToggleTopRight();
         }*/

        private void Time_Click(object sender, RoutedEventArgs e) {
            viewmodel.NextStep();
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

        private void Temperature_MouseWheel(object sender, MouseWheelEventArgs e) {
            Temperature.Value += Math.Sign(e.Delta) * 0.01;
            e.Handled = true;
        }

        /*private void CouplingConstant_MouseWheel(object sender, MouseWheelEventArgs e) {
            CouplingConstant.Value += Math.Sign(e.Delta) * 0.01;
            e.Handled = true;
        }*/

        private void MagneticField_MouseWheel(object sender, MouseWheelEventArgs e) {
            MagneticField.Value += Math.Sign(e.Delta) * 0.009;
        }

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
        bool fixed_temperature = false;
        bool fixed_magnfield = false;

        private void FixTemperature_Checked(object sender, RoutedEventArgs e) {
            fixed_temperature = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }

        private void FixMagneticField_Checked(object sender, RoutedEventArgs e) {
            fixed_magnfield = ((CheckBox)sender).IsChecked ?? false;
            e.Handled = true;
        }
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e) {
            if(!fixed_temperature)
                Canvas.SetLeft(myThumb, Math.Max(15, Math.Min(TemperatureMagneticField.ActualWidth - 25, Canvas.GetLeft(myThumb) + e.HorizontalChange)));
            if(!fixed_magnfield)
                Canvas.SetTop(myThumb, Math.Max(15, Math.Min(TemperatureMagneticField.ActualHeight - 25, Canvas.GetTop(myThumb) + e.VerticalChange)));
            UpdateParameters(Canvas.GetLeft(myThumb), Canvas.GetTop(myThumb));
            e.Handled = true;
        }

        private void UpdateParameters(double x, double y) {
            var temp = (x - 15) / (TemperatureMagneticField.ActualWidth - 40) * 5;
            var field = 0.5 - (y - 15) / (TemperatureMagneticField.ActualHeight - 40) * 1;
            viewmodel.ChangeTemperature(temp);
            viewmodel.ChangeField(field);
            TemperatureTextBox.Text = temp.ToString("0.00");
            MagnFieldTextBox.Text = field.ToString("0.00");
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

        public IList<DataPoint> Points { get; private set; }

        private void plotinit() {
            this.Points = new List<DataPoint>();
            var x = new Random();
            for(int i = 0; i < 100; i++) {
                Points.Add(new DataPoint(i, x.Next(-200, 200)));
            }
            EnergyPlot.ItemsSource = Points;
            MagnetizationPlot.ItemsSource = Points.Select(a => new DataPoint(a.X, a.Y * x.NextDouble()));
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
            } else {
                worker.CancelAsync();
            }
        }

        private BackgroundWorker worker_Init() {
            var worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            worker.DoWork += worker_Work;
            worker.ProgressChanged += worker_Progress;
            worker.RunWorkerCompleted += worker_Completed;
            return worker;
        }

        private void worker_Work(object sender, DoWorkEventArgs e) {
            if(e.Argument != null) {
                var max = (int)e.Argument;
                for(int i = 0; i < max; i++) {
                    (sender as BackgroundWorker).ReportProgress(i);
                    viewmodel.NextStep();
                }
            } else {
                int i = 0;
                while(true) {
                    (sender as BackgroundWorker).ReportProgress(i++);
                    viewmodel.NextStep();
                }
            }
        }

        long timerefresh;
        private void worker_Progress(object sender, ProgressChangedEventArgs e) {
            long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if(time - timerefresh > 100) {
                timerefresh = time;
                StatusText.Text = e.ProgressPercentage.ToString();
                viewmodel.Refresh();
            }
        }

        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e) {
            viewmodel.Refresh();
        }



        #endregion







    }
}
