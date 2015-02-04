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
using Ising.Ising;
using System.Diagnostics;

namespace Ising.Render {
    /// <summary>
    /// Interaction logic for LatticeOutput.xaml
    /// </summary>
    public partial class LatticeOutput : UserControl {
        IsingModelRender model;
        public LatticeOutput() {
            InitializeComponent();
            model = new IsingModelRender(30);
            isingGrid.Columns = model.N;
            foreach(var elem in model.GetRectangles()) {
                isingGrid.Children.Add(elem);
            }

        }

        private void Stuff_Click(object sender, RoutedEventArgs e) {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            model.DoStuff();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }

    public class IsingModelRender : IsingModel {
        private Rectangle[,] rects;
        private SolidColorBrush[,] cols;
        public IsingModelRender(int n)
            : base(n) {
            rects = new Rectangle[n, n];
            cols = new SolidColorBrush[n, n];

            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    rects[i, j] = new Rectangle() { 
                        Fill = Color(model[i, j]),
                        MinWidth = 1,
                        MinHeight = 1,
                        Margin = new Thickness(0),
                        Stretch = Stretch.UniformToFill
                    };
                }
            }
        }



        static SolidColorBrush[] colors = { new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Gold) };

        static private SolidColorBrush Color(int val) {
            switch(val) {
                case -1: return colors[0];
                case 1: return colors[1];
                case 0: return colors[2];
                default: return colors[3];
            }
        }

        public async override void DoStuff() {

            base.DoStuff();
            //Parallel.For(1, N - 1, a => {
            //    Parallel.For(1, N - 1, b => {
            //        rects[a, b].Fill = Color(model[a, b]);
            //    });
            //});
            for(int i = 0; i < N - 1; i++) {
                for(int j = 0; j < N - 1; j++) {
                    rects[i, j].Fill = Color(model[i, j]);
                }
            }



            //// The Work to perform on another thread 
            //ThreadStart start = delegate() { // ... 
            //    // Sets the Text on a TextBlock Control.
            //    // This will work as its using the dispatcher
            //    Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(SetStatus), "From Other Thread");
            //}; // Create the thread and kick it started! 
            //new Thread(start).Start();
        }

        public IEnumerable<Rectangle> GetRectangles() {
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    yield return rects[i, j];
                }
            }
        }
    }
}
