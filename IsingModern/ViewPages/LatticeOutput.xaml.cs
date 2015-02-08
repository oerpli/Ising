using IsingModern.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace IsingModern.Render {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class LatticeOutput : UserControl {
        private IsingRenderModel viewmodel;
        private bool PeriodicBoundary = false;
        private int currentN = 600;
        private int rndCounter = 0;

        private const int maximalN = 600, minimalN = 3; //both should divide 600. 


        #region Initialization

        public LatticeOutput() {
            InitializeComponent();
            viewmodel = new IsingRenderModel(currentN, PeriodicBoundary);
            BoundaryText.Text = PeriodicBoundary ? "Periodic" : "Walled";
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

        private void TopRight_Click(object sender, RoutedEventArgs e) {
            viewmodel.TopRight();
        }
    }
}
