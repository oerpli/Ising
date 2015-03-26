using System;
using System.Collections.Generic;
using System.Linq;

namespace IsingModern.Model {
    public sealed partial class Lattice {
        #region PhysicalParameters

        public double Coupling { get; set; } // nearest neighbour interaction strength. positive values => ferromagnetic
        public double Field { get; set; } // field strength (bias for spins in either direction)
        public double Beta { get; set; } /*inverse temperature*/

        #endregion

        public double TotalInteraction;
        public double TotalMagnetization;
        static private readonly Random Rnd = new Random();


        public Tuple<double, double> Sweep() {
            var n = N * N;
            for(int i = 0; i < n; i++) {
                Dynamic(RandomSpin());
            }
            UpdateStats();
            return Tuple.Create((TotalInteraction - TotalMagnetization * Field) / n, TotalMagnetization / n);
        }

        #region DynamicsAlgorithms
        public void Kawasaki(Spin chosen) {
            Spin exchange = chosen.Neighbours[Rnd.Next(4)];
            if(chosen.Value == exchange.Value) return;

            var energyDifference = CalculateEnergyChangeKawasaki(chosen, exchange);
            if(Accept(energyDifference, 0)) {
                chosen.ToggleSpin();
                exchange.ToggleSpin();
                TotalInteraction += energyDifference;
            }
        }
        public void SingleFlip(Spin chosen) {
            var energyDifference = 2 * CalculateEnergyNN(chosen);
            if(Accept(energyDifference, chosen.Value)) {
                chosen.ToggleSpin();
                TotalInteraction += energyDifference;
                TotalMagnetization -= 2 * chosen.Value;
            }
        }
        #endregion


        #region EnergyCalculations
        private void UpdateStats() {
            var interactionEnergy = Spins.Aggregate(0.0, (sum, spin) => sum + spin.InteractionEnergy());
            TotalMagnetization = Spins.Aggregate(0.0, (sum, spin) => sum + spin.Value);
            TotalInteraction = 0.5 * Coupling * interactionEnergy - Field * TotalMagnetization;
        }

        private static int CalculateEnergyNN(Spin s) {
            return s.Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value) * s.Value;
        }

        private static int CalculateEnergyChangeKawasaki(Spin s1, Spin s2) {
            return -2 * (CalculateEnergyNN(s2) + CalculateEnergyNN(s2) - 1);
        }

        #endregion

        #region AcceptanceFunctions

        private bool MetropolisCached(int nearestNeighbourEnergy, int magnetizationEnergy) {
            return Metropolis(nearestNeighbourEnergy * Coupling + magnetizationEnergy * Field);
        }

        private bool GlauberCached(int nearestNeighbourEnergy, int magnetizationEnergy) {
            return Glauber(nearestNeighbourEnergy * Coupling + magnetizationEnergy * Field);
        }


        public bool Metropolis(double deltaE) {
            return deltaE <= 0.0 || Rnd.NextDouble() < Math.Exp(-deltaE * Beta);
        }
        public bool Glauber(double deltaE) {
            return Rnd.NextDouble() < (1.0 / (1.0 + Math.Exp(deltaE * Beta)));
        }
        #endregion

        #region Delegates
        //Signaturen
        public delegate void DynamicsAlgorithm(Spin chosen);
        public delegate bool AcceptanceFunction(int nearestNeighbourEnergy, int magnetizationEnergy);

        public readonly Dictionary<string, AcceptanceFunction> Accepts;
        //Aktuelle Algorithmen
        public AcceptanceFunction Accept;
        public readonly DynamicsAlgorithm Dynamic;
        #endregion

    }
}
