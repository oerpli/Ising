using IsingModern.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Wpf;

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

        bool captured = false;
        bool fixed_temperature = false;
        bool fixed_magnfield = false; 
        double x_shape, x_canvas, y_shape, y_canvas;
        System.Windows.UIElement source = null;

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
                viewmodel.NextStep();
                e.Handled = true;
            }
        }

        private void RandomizeClick(object sender, RoutedEventArgs e) {
            viewmodel.Randomize();
            StatusText.Text = "Count: " + (++rndCounter).ToString();
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

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            viewmodel.NextStep();
        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void Coupling_Click(object sender = null, RoutedEventArgs e = null)
        {
            if (sender != null) Ferromagnetic = !Ferromagnetic;
            viewmodel.ChangeCoupling(Ferromagnetic ? 1.0 : -1.0);
            CouplingText.Text = Ferromagnetic ? "Ferromagnetic" : "Anti-Ferromagnetic";
        }

        private void Algorithm_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null) Metropolis = !Metropolis;
           
            AlgorithmText.Text = Metropolis ? "Metropolis" : "Glauber";
            viewmodel.ChangeAccept(AlgorithmText.Text);
        }

        #endregion

        #region Drag&Drop
        private void shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (System.Windows.UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = e.GetPosition(TemperatureMagneticField).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = e.GetPosition(TemperatureMagneticField).Y;
        }
        private void shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(TemperatureMagneticField).X;
                double y = e.GetPosition(TemperatureMagneticField).Y;
                if (!fixed_temperature) x_shape += x - x_canvas;
                if (x_shape > TemperatureMagneticField.ActualWidth - 30.0  || x_shape < 20.0)
                {
                    x_shape -= x - x_canvas;
                    captured = false;
                }
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                if (!fixed_magnfield) y_shape += y - y_canvas;
                if (y_shape > TemperatureMagneticField.ActualHeight - 30.0 || y_shape < 20.0)
                {
                    y_shape -= y - y_canvas;
                    captured = false; 
                }
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
            }
        }
        private void shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            x_shape = Canvas.GetLeft(source);
            y_shape = Canvas.GetTop(source);
            double new_temperature = -100.0/(TemperatureMagneticField.ActualWidth - 40.0)+(x_shape + 5.0) * 5.0 / (TemperatureMagneticField.ActualWidth-40.0);
            double new_magnfield = 0.5 + 20.0/(TemperatureMagneticField.ActualHeight -40.0) - (y_shape + 5.0) / (TemperatureMagneticField.ActualHeight - 40.0);
            viewmodel.ChangeTemperature(new_temperature);
            viewmodel.ChangeField(new_magnfield);
            TemperatureTextBox.Text=new_temperature.ToString("0.00");
            MagnFieldTextBox.Text = new_magnfield.ToString("0.00"); 
            Mouse.Capture(null);
            captured = false;
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

        private void FixTemperature_Checked(object sender, RoutedEventArgs e)
        {
            fixed_temperature = !fixed_temperature; 
        }

        private void FixMagneticField_Checked(object sender, RoutedEventArgs e)
        {
            fixed_magnfield = !fixed_magnfield; 
        }


        


    }
}
