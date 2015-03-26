using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace IsingModern.Model {
    public abstract class Settings {
        static public bool DEBUG = false;
    }
    public class Spin {
        public int Index { get; private set; }
        public int Value { get; internal set; }

        public Spin[] Neighbours { get; private set; } //North East South West

        #region Initialization
        public Spin(int val, int index) {
            Index = index;
            Value = val;
        }

        public void SetNeighbours(Spin North, Spin East, Spin South, Spin West) {
            Neighbours = new Spin[4] { North, East, South, West };
        }

        #endregion

        #region Manipulation

        internal void ToggleSpin() {
            this.Value *= -1;
        }

        internal void SetSpin() {
            this.Value = -1;
        }

        internal void ToggleBoundary(bool? boundary = null) {
            this.Value = boundary ?? Value != 0 ? 0 : -1;
        }
        #endregion

        public int InteractionEnergy() {
            int spinsum = Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value);
            return -Value * spinsum;
        }

        #region Rendering

        public static Dictionary<int, Color> PointColors = new Dictionary<int, Color>() 
            {   { -1 , Colors.DeepSkyBlue }
            ,   { 1, Colors.DarkBlue  } 
            ,   { 0 , Colors.White}};
        private static Color failColor = Colors.Gold;


        public Color Color {
            get {
                if(PointColors.ContainsKey(Value)) {
                    return PointColors[Value];
                } else {
                    return failColor;
                }
            }
        }

        #endregion

    }
}
