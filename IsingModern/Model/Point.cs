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
    public class Point : INotifyPropertyChanged {
        private int _value;
        public int Value {
            get {
                return _value;
            }
            internal set {
                //if(_value != value) {
                //rectangle.Fill = Color;
                //text.Text = value.ToString();
                _value = value;
                OnPropertyChanged("Color");
                //OnPropertyChanged("Color");
                //OnPropertyChanged("Text");
                //}
            }
        }

        //public int Value { get; internal set; }
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


        //private Rectangle rectangle;
        //private TextBlock text;
        //public Grid RenderElement { get; private set; }
        //private void InitDraw() {
        //    if(RenderElement == null) {
        //        RenderElement = new Grid() {
        //            Width = 600,
        //            Height = 600
        //        };
        //    }
        //    if(rectangle == null) {
        //        rectangle = new Rectangle() {
        //            Fill = this.ColorX,
        //            MinWidth = 1,
        //            MinHeight = 1,
        //            Margin = new Thickness(0),
        //            Stretch = Stretch.UniformToFill
        //        };
        //        RenderElement.Children.Add(rectangle);

        //    }
        //    if(Settings.DEBUG && text == null) {
        //        text = new TextBlock() {
        //            Text = this.Text,
        //            FontSize = 20,
        //            TextAlignment = TextAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center,
        //            HorizontalAlignment = HorizontalAlignment.Center
        //        };
        //        RenderElement.Children.Add(text);
        //    }
        //}

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

        //private int _lastvalue = int.MinValue;
        //internal void Redraw(bool force = false) {
        //    if(Value != _lastvalue || force) {
        //        if(Settings.DEBUG) text.Text = Text;
        //        rectangle.Fill = ColorX;
        //        ColorX.Freeze();
        //        _lastvalue = Value;
        //    }
        //}
        #endregion

    }
}
