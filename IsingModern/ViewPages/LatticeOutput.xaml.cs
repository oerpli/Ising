using IsingModern.Ising;
using IsingModern.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;


namespace IsingModern.Render {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class LatticeOutput : UserControl {
        IsingRenderModel rmtestmodel;
        bool PeriodicBoundary = false;
        int currentN = 100;

        public LatticeOutput() {
            InitializeComponent();
            rmtestmodel = new IsingRenderModel(50);
            rmtest.Children.Add(rmtestmodel);
            LatticeSizeInput.Text = currentN.ToString();
            //NewLattice(currentN);
            maingrid.DataContext = IsingModel.Current;
        }

        private void NewLattice(int n) {
            IsingModel.Current = new IsingModel(n);
            ToggleBoundary_Click();
            IsingModel.Redraw();
        }
        private int count = 0;
        private void RandomizeClick(object sender, RoutedEventArgs e) {
            IsingModel.Current.Randomize();
            StatusText.Text = "Count: " + (++count).ToString();
            rmtestmodel.InvalidateVisual();
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //Stuff.Dispatcher.BeginInvoke(
            //   DispatcherPriority.Normal,
            //   new UpdateLatticeDelegate());
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);
            //IsingModel.Redraw();
        }
        private void ToggleBoundary_Click(object sender = null, RoutedEventArgs e = null) {
            if(sender != null) PeriodicBoundary = !PeriodicBoundary;
            IsingModel.Current.SetBoundary(PeriodicBoundary);
            BoundaryText.Text = PeriodicBoundary ? "Periodic" : "Walled";
            IsingModel.Redraw();
        }
        private void LatticeSize_Click(object sender, RoutedEventArgs e) {
            //NewLattice(currentN);
        }
        private void LatticeSize_MouseWheel(object sender, MouseWheelEventArgs e) {
            int diff = e.Delta > 0 ? 1 : -1;
            currentN = Math.Min(80, Math.Max(3, currentN + diff));
            LatticeSizeInput.Text = currentN.ToString();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var s = (Ising.Point)(((Rectangle)sender).DataContext);
            s.Value *= -1;
            Console.WriteLine(s.Value.ToString());
        }

        private void Rectangle_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            var s = (Ising.Point)(((Rectangle)sender).DataContext);
            s.Value = s.Value == 0 ? 1 : 0;//s.Value*s.Value -1;
            Console.WriteLine(s.Value.ToString());
        }

        private void UpdateCorner(object sender, RoutedEventArgs e) {
            IsingModel.Current.Points[currentN * 2 - 2].Value *= -1;
        }
    }




    //public override void DoStuff() {
    //     Stopwatch watch = new Stopwatch();
    //     watch.Start();
    //     base.DoStuff();
    //     var thread = new Thread(new ThreadStart(delegate() {
    //         Parallel.For(0, N - 1, i => {
    //             Parallel.For(0, N - 1, j => {
    //                 var op = rects[i, j].Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
    //                     new Action(delegate() {
    //                     rects[i, j].Fill = Color(model[i, j]);
    //                     var x = rects[i, j].Fill;
    //                     var y = x;
    //                 }));
    //                 //rects[i, j].Fill = Color(model[i, j]);
    //                 op.Completed += new EventHandler(WriteStatus);
    //             });
    //         });
    //         watch.Stop();
    //         Console.WriteLine("base: " + watch.ElapsedMilliseconds); model[0, 0] = 3;
    //     }));
    //     thread.Start();
    //     //// The Work to perform on another thread 
    //     //ThreadStart start = delegate() { // ... 
    //     //    // Sets the Text on a TextBlock Control.
    //     //    // This will work as its using the dispatcher
    //     //    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(SetStatus), "From Other Thread");
    //     //}; // Create the thread and kick it started! 
    //     //new Thread(start).Start();
    // }
}
