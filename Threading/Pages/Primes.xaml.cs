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
// from https://msdn.microsoft.com/en-us/library/ms741870%28v=vs.110%29.aspx
namespace Threading.Pages {
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl {
        public Home() {
            InitializeComponent();
        }



        public delegate void NextPrimeDelegate();

        //Current number to check  
        private long num = 3;

        private bool continueCalculating = false;

        private void startStopButton_Click(object sender, EventArgs e) {
            if(continueCalculating) {
                continueCalculating = false;
                startStopButton.Content = "Resume";
            } else {
                continueCalculating = true;
                startStopButton.Content = "Stop";
                startStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new NextPrimeDelegate(CheckNextNumber));
            }
        }

        public void CheckNextNumber() {
            // Reset flag.
            NotAPrime = false;

            for(long i = 3; i <= Math.Sqrt(num); i++) {
                if(num % i == 0) {
                    // Set not a prime flag to true.
                    NotAPrime = true;
                    break;
                }
            }

            // If a prime number. 
            if(!NotAPrime) {
                bigPrime.Text = num.ToString();
            }

            num += 2;
            if(continueCalculating) {
                startStopButton.Dispatcher.BeginInvoke(
                    DispatcherPriority.SystemIdle,
                    new NextPrimeDelegate(this.CheckNextNumber));
            }
        }

        private bool NotAPrime = false;
    }
}
