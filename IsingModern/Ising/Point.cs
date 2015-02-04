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
    class Point {

        public int Value { get; private set; }


        public Point(int val) {
            Value = val;
            InitRectangle();
        }





        #region Rendering
        public Rectangle Rect { get; private set; }
        private void InitRectangle() {
            if(Rect == null) {
                Rect = new Rectangle() {
                    Fill = GetColor(this),
                    MinWidth = 1,
                    MinHeight = 1,
                    Margin = new Thickness(0),
                    Stretch = Stretch.UniformToFill
                };
            }
        }

        private static Dictionary<int, SolidColorBrush> colors = new Dictionary<int, SolidColorBrush>() 
            {   { -1, new SolidColorBrush(Colors.Red)  } 
            ,   { 1 , new SolidColorBrush(Colors.Blue) }
            ,   { 0 , new SolidColorBrush(Colors.Black)}};
        private static SolidColorBrush failColor = new SolidColorBrush(Colors.Gold);

        private static SolidColorBrush GetColor(Point p) {
            if(colors.ContainsKey(p.Value)) {
                return colors[p.Value];
            } else {
                return failColor;
            }
        }
        #endregion
    }
}
