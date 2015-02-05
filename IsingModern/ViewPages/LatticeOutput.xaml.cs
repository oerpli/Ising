using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using IsingModern.Ising;
using System.Diagnostics;

namespace IsingModern.Render {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class LatticeOutput : UserControl {
        IsingModel model = new IsingModel(30);
        public LatticeOutput() {
            InitializeComponent();
            isingGrid.Columns = model.N;
            foreach(var elem in model.GetRectangles()) {
                isingGrid.Children.Add(elem);
            }

        }

        private delegate void UpdateLatticeDelegate();
        private int count = 0;
        private void RandomizeClick(object sender, RoutedEventArgs e) {
            model.Randomize();
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //Stuff.Dispatcher.BeginInvoke(
            //   DispatcherPriority.Normal,
            //   new UpdateLatticeDelegate());
            StatusText.Text = (++count).ToString() + " " + model.points[1, 1].Value + " " + model.points[1, 2].Value;
            //watch.Stop();
            //Console.WriteLine(watch.ElapsedMilliseconds);
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
