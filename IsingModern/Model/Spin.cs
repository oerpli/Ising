using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace IsingModern.Model {
    public abstract class Settings {
        static public bool Debug = false;
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

        public void SetNeighbours(Spin north, Spin east, Spin south, Spin west) {
            Neighbours = new Spin[4] { north, east, south, west };
        }

        #endregion

        #region Manipulation

        internal void ToggleSpin() {
            this.Value *= -1;
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
        private static Color _failColor = Colors.Gold;


        public Color Color {
            get {
                if(PointColors.ContainsKey(Value)) {
                    return PointColors[Value];
                } else {
                    return _failColor;
                }
            }
        }

        #endregion

    }
}
