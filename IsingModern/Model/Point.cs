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
using System.ComponentModel;

namespace IsingModern.Ising {
    public abstract class Settings {
        static public bool DEBUG = false;
    }
    public class Point {
        private int _value;
        public int Value {
            get {
                return _value;
            }
            internal set {
                _value = value;
            }
        }

        public Point[] Neighbours { get; private set; } //North East South West


        public Point(int val) {
            //_value = val;
            //InitDraw();
            Value = val;
        }

        public void SetNeighbours(Point North, Point East, Point South, Point West) {
            Neighbours = new Point[4] { North, East, South, West };
        }



        #region Rendering

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null) {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }


        static Point() {
            var brushes = PointColors.Values.AsEnumerable();
            foreach(var x in brushes) {
                if(x.CanFreeze) {
                    x.Freeze();
                }
            }
        }

        public static Dictionary<int, SolidColorBrush> PointColors = new Dictionary<int, SolidColorBrush>() 
            {   { -1 , new SolidColorBrush(Colors.DeepSkyBlue) }
            ,   { 1, new SolidColorBrush(Colors.DarkBlue)  } 
            ,   { 0 , new SolidColorBrush(Colors.Black)}};
        private static SolidColorBrush failColor = new SolidColorBrush(Colors.Gold);

        public SolidColorBrush Color {
            get {
                if(PointColors.ContainsKey(Value)) {
                    return PointColors[Value];
                } else {
                    return failColor;
                }
            }
        }
        public String Text {
            get { return Value.ToString(); }
        }

        #endregion


        internal void ToggleSpin() {
            this.Value *= -1;
        }

        internal void ToggleBoundary() {
            this.Value = Value == 0 ? 1 : 0;
        }
    }
}
