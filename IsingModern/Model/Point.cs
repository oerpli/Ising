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


namespace IsingModern.Ising {
    public class Point {
        private int _value;
        public int Value {
            get {
                return _value;
            }
            internal set {
                if(_value != value) {
                    Rectangle.Fill = Color;
                    _value = value;
                }
            }
        }
        public Point[] Neighbours { get; private set; } //North East South West


        public Point(int val) {
            _value = val;
            InitRectangle();
        }

        public void SetNeighbours(Point North, Point East, Point South, Point West) {
            Neighbours = new Point[4] { North, East, South, West };
        }



        #region Rendering
        public Rectangle Rectangle { get; private set; }
        private void InitRectangle() {
            if(Rectangle == null) {
                Rectangle = new Rectangle() {
                    Fill = Color,
                    MinWidth = 1,
                    MinHeight = 1,
                    Margin = new Thickness(0),
                    Stretch = Stretch.UniformToFill
                };
            }
        }

        static Point() {
            var brushes = colors.Values.AsEnumerable();
            foreach(var x in brushes) {
                if(x.CanFreeze) {
                    x.Freeze();
                }
            }
        }

        private static readonly Dictionary<int, SolidColorBrush> colors = new Dictionary<int, SolidColorBrush>() 
            {   { -1, new SolidColorBrush(Colors.DodgerBlue)  } 
            ,   { 1 , new SolidColorBrush(Colors.DeepSkyBlue) }
            ,   { 0 , new SolidColorBrush(Colors.Black)}};
        private static SolidColorBrush failColor = new SolidColorBrush(Colors.Gold);

        private SolidColorBrush Color {
            get {
                if(colors.ContainsKey(Value)) {
                    return colors[Value];
                } else {
                    return failColor;
                }
            }
        }
        #endregion
    }
}
