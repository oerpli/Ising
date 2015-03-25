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

        private int CalculateEnergyNN(Spin s){
            return s.Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value) * s.Value;
        }
        
        private int CalculateEnergyChangeKawasaki(Spin s1, Spin s2) {
            return -2*(CalculateEnergyNN(s2) + CalculateEnergyNN(s2) - 1);
        }



        #region Delegates
        //Signaturen
        public delegate void DynamicsAlgorithm(Spin Chosen);
        public delegate bool AcceptanceFunction(double DeltaE);

        public Dictionary<string, AcceptanceFunction> accepts;
        //Alle Algorithmen
        //DynamicsAlgorithm[] dynamics;
        //AcceptanceFunction[] accepts;
        //TODO hashmap o.Ä. implementieren um algorithmus auszuwählen.

        //Aktuelle Algorithmen
        public AcceptanceFunction accept;
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
            if(accept(2*EnergyDifference)) {
                Chosen.ToggleSpin();
                TotalEnergy += EnergyDifference;
            }
        }
        #endregion
        #region AcceptanceFunctions
        public bool MetropolisCached(double NN, double M){
            return false;
        }
        
        public bool GlauberCached(double NN, double M){
            return false;
        }
        
        
        public bool Metropolis(double DeltaE) {
            return DeltaE <= 0.0 || r.NextDouble() < Math.Exp(-DeltaE * Beta);
        }
        public bool Glauber(double DeltaE) {
            return r.NextDouble() < (1.0 / (1.0 + Math.Exp(DeltaE * Beta)));
        }
        #endregion
        #endregion
 
    }
}
