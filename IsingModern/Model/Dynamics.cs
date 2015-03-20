using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsingModern.Ising {
    public partial class Lattice {
        #region PhysicalParameters

        public double J { get; set; } // nearest neighbour interaction strength. positive values => ferromagnetic
        public double h { get; set; } // field strength (bias for spins in either direction)
        public double Beta { get; set; } /*inverse temperature*/

        #endregion

        public double TotalEnergy;
        static private Random r = new Random();



        public void UpdateTotalEnergy() {
            var interactionEnergy = Spins.Aggregate(0.0, (sum, spin) => sum + spin.InteractionEnergy());
            var magnetization = Spins.Aggregate(0.0, (sum, spin) => sum + spin.Value);
            TotalEnergy = -0.5 * J * interactionEnergy - h * magnetization;
        }





        public void Sweep() {
            var n = N * N;
            for(int i = 0; i < n; i++) {
                dynamic(RandomSpin());
            }
        }




        private double CalculateEnergyChange(Spin Chosen) {
            int spinsum = Chosen.Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value);
            double EnergyChange = +2.0 * h * Chosen.Value + Chosen.Value * spinsum * J;
            return EnergyChange;
        }


        private double CalculateEnergyChangeKawasaki(Spin Chosen1, Spin Chosen2) {
            double EnergyChange = 0.0;
            int SameSpins = 0;
            foreach(var Neighbour in Chosen1.Neighbours) {
                SameSpins += (Neighbour.Value == Chosen1.Value) ? 1 : 0;
            }
            foreach(var Neighbour in Chosen2.Neighbours) {
                SameSpins += (Neighbour.Value == Chosen2.Value) ? 1 : 0;
            }
            EnergyChange = -12.0 * J + 4.0 * J * (double)SameSpins;
            return EnergyChange;
        }



        #region Delegates
        //Signaturen
        public delegate void DynamicsAlgorithm(Spin Chosen);
        public delegate bool AcceptanceFunction(double DeltaE);
        //Alle Algorithmen
        //DynamicsAlgorithm[] dynamics;
        //AcceptanceFunction[] accepts;
        //TODO hashmap o.Ä. implementieren um algorithmus auszuwählen.

        //Aktuelle Algorithmen
        private AcceptanceFunction accept;
        public DynamicsAlgorithm dynamic;


        #region DynamicsAlgorithms
        public void Kawasaki(Spin Chosen) {
            Spin Exchange = Chosen.Neighbours[r.Next(4)];
            if(Chosen.Value != Exchange.Value) {
                double EnergyDifference = CalculateEnergyChangeKawasaki(Chosen, Exchange);
                if(accept(EnergyDifference)) {
                    Chosen.ToggleSpin();
                    Exchange.ToggleSpin();
                    TotalEnergy += EnergyDifference;
                }
            }
        }
        public void SingleFlip(Spin Chosen) {
            double EnergyDifference = CalculateEnergyChange(Chosen);
            if(accept(EnergyDifference)) {
                Chosen.ToggleSpin();
                TotalEnergy += EnergyDifference;
            }
        }
        #endregion
        #region AcceptanceFunctions
        public bool Metropolis(double DeltaE) {
            return DeltaE <= 0.0 || r.NextDouble() < Math.Exp(-DeltaE * Beta);
        }
        public bool Glauber(double DeltaE) {
            return r.NextDouble() > (1.0 / (1.0 + Math.Exp(DeltaE * Beta)));
        }
        #endregion
        #endregion
    }
}
