using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.ComponentModel;

namespace IsingModern.Ising {
    public abstract class Settings {
        static public bool DEBUG = false;
    }
    public class Point {
        private int _value;
        public int Index { get; private set; }
        public int Value {
            get {
                return _value;
            }
            internal set {
                _value = value;
            }
        }

        public Point[] Neighbours { get; private set; } //North East South West


        public Point(int val, int index) {
            Index = index;
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
            var brushes = PointColorBrushes.Values.AsEnumerable();
            foreach(var x in brushes) {
                if(x.CanFreeze) {
                    x.Freeze();
                }
            }
        }

        public static Dictionary<int, SolidColorBrush> PointColorBrushes = new Dictionary<int, SolidColorBrush>() 
            {   { -1 , new SolidColorBrush(Colors.DeepSkyBlue) }
            ,   { 1, new SolidColorBrush(Colors.DarkBlue)  } 
            ,   { 0 , new SolidColorBrush(Colors.White)}};

        public static Dictionary<int, Color> PointColors = new Dictionary<int, Color>() 
            {   { -1 , Colors.DeepSkyBlue }
            ,   { 1, Colors.DarkBlue  } 
            ,   { 0 , Colors.White}};
        private static SolidColorBrush failColorBrush = new SolidColorBrush(Colors.Gold);
        private static Color failColor = Colors.Gold;

        public SolidColorBrush ColorBrush {
            get {
                if(PointColorBrushes.ContainsKey(Value)) {
                    return PointColorBrushes[Value];
                } else {
                    return failColorBrush;
                }
            }
        }

        public Color Color {
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
            if(this.Value == 0)
                this.Value = 1;
            this.Value *= -1;
        }

        internal void ToggleBoundary(bool? boundary = null) {
            this.Value = boundary ?? Value != 0 ? 0 : -1;
        }
    }
}
