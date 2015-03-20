using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsingModern.Ising {
    abstract public class Dynamics {
        static public Lattice Lattice; //lattice to apply changes on.
        #region PhysicalParameters

        static public double J { get; set; } // nearest neighbour interaction strength. positive values => ferromagnetic
        static public double h { get; set; } // field strength (bias for spins in either direction)
        static public double Beta { get; set; } /*inverse temperature*/

        #endregion

        static public double TotalEnergy;
        static private Random r = new Random();



        static public void UpdateTotalEnergy() {
            var interactionEnergy = Lattice.Spins.Aggregate(0.0, (sum, spin) => sum + spin.InteractionEnergy());
            var magnetization = Lattice.Spins.Aggregate(0.0, (sum, spin) => sum + spin.Value);
            TotalEnergy = -0.5 * J * interactionEnergy - h * magnetization;
        }





        static public void Sweep() {
            var n = Lattice.N * Lattice.N;
            for(int i = 0; i < n; i++) {
                dynamic(Lattice.RandomSpin());
            }
        }




        static private double CalculateEnergyChange(Spin Chosen) {
            int spinsum = Chosen.Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value);
            double EnergyChange = + 2.0 * h * Chosen.Value + Chosen.Value * spinsum * J ;
            return EnergyChange;
        }


        static private double CalculateEnergyChangeKawasaki(Spin Chosen1, Spin Chosen2) {
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
        static DynamicsAlgorithm[] dynamics = new DynamicsAlgorithm[] { SingleFlip, Kawasaki };
        static AcceptanceFunction[] accepts = new AcceptanceFunction[] { Metropolis, Glauber };

        //Aktuelle Algorithmen
        static private AcceptanceFunction accept = accepts[0];
        static public DynamicsAlgorithm dynamic = dynamics[0];

        //TODO hashmap o.Ä. implementieren um algorithmus auszuwählen.

        #region DynamicsAlgorithms
        static public void Kawasaki(Spin Chosen) {
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
        static public void SingleFlip(Spin Chosen) {
            double EnergyDifference = CalculateEnergyChange(Chosen);
            if(accept(EnergyDifference)) {
                Chosen.ToggleSpin();
                TotalEnergy += EnergyDifference;
            }
        }
        #endregion
        #region AcceptanceFunctions
        static public bool Metropolis(double DeltaE) {
            return DeltaE <= 0.0 || r.NextDouble() < Math.Exp(-DeltaE * Beta);
        }
        static public bool Glauber(double DeltaE) {
            return r.NextDouble() > (1.0 / (1.0 + Math.Exp(DeltaE * Beta)));
        }
        #endregion
        #endregion
    }
}
