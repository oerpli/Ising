using System;
using System.Collections.Generic;
using System.Linq;

namespace IsingModern.Model {
    public sealed partial class Lattice {
        #region PhysicalParameters
        private double _field, _coupling, _beta;
        public double Coupling { get { return _coupling; } set { _coupling = value; ResetCache(); } } // nearest neighbour interaction strength. positive values => ferromagnetic
        public double Field { get { return _field; } set { _field = value; ResetCache(); } } // field strength (bias for spins in either direction)
        public double Beta { get { return _beta; } set { _beta = value; ResetCache(); } } /*inverse temperature*/

        #endregion

        public double TotalInteraction;
        public double TotalMagnetization;
        public static readonly Random Rnd = new Random();

        public Tuple<double, double> Sweep() {
            var n = N * N;
            for(int i = 0; i < n; i++) {
                Dynamic(RandomSpin());
            }
            return Tuple.Create((TotalInteraction * Coupling - TotalMagnetization * Field) / n, TotalMagnetization / n);
        }

        #region DynamicsAlgorithms

        public void SingleFlip(Spin chosen) {
            var energyDifference = 2 * CalculateEnergyNn(chosen);
            if(Accept(energyDifference, chosen.Value)) {
                chosen.ToggleSpin();
                TotalInteraction += energyDifference;
                TotalMagnetization += 2 * chosen.Value;
            }
        }
        public void Kawasaki(Spin chosen) {
            Spin exchange = chosen.Neighbours[Rnd.Next(4)];
            if(chosen.Value == 0 || exchange.Value == 0 || chosen.Value == exchange.Value) return;
            var energyDifference = 2 * CalculateEnergyChangeKawasaki(chosen, exchange);
            if(Accept(energyDifference, 0)) {
                chosen.ToggleSpin();
                exchange.ToggleSpin();
                TotalInteraction += energyDifference;
            }
        }

        #endregion


        #region EnergyCalculations
        public void UpdateStats() {
            var interactionEnergy = Spins.Aggregate(0.0, (sum, spin) => sum + spin.InteractionEnergy());
            TotalMagnetization = Spins.Aggregate(0.0, (sum, spin) => sum + spin.Value);
            TotalInteraction = 0.5 * Coupling * interactionEnergy - Field * TotalMagnetization;
        }

        private static int CalculateEnergyNn(Spin s) {
            return s.Neighbours.Aggregate(0, (sum, spin) => sum + spin.Value) * s.Value;
        }

        private static int CalculateEnergyChangeKawasaki(Spin s1, Spin s2) {
            return CalculateEnergyNn(s1) + CalculateEnergyNn(s2) + 2;
        }

        #endregion

        #region AcceptanceFunctions

        private readonly Dictionary<double, double> metropolisDictionary = new Dictionary<double, double>();

        private void ResetCache() {
            lock(metropolisDictionary) {
                metropolisDictionary.Clear();
            }
        }

        private bool MetropolisCached(int nearestNeighbourInteraction, int magnetizationEnergy) {
            lock(metropolisDictionary) {
                var e = nearestNeighbourInteraction * Coupling + magnetizationEnergy * Field;
                if(e <= 0) return true;
                if(!metropolisDictionary.ContainsKey(e)) {
                    metropolisDictionary.Add(e, Metropolis(e));
                }
                return Rnd.NextDouble() < metropolisDictionary[e];
            }
        }

        private bool GlauberCached(int nearestNeighbourInteraction, int magnetizationEnergy) {
            return Glauber(nearestNeighbourInteraction * Coupling + magnetizationEnergy * Field);
        }


        private double Metropolis(double deltaE) {
            return Math.Exp(-deltaE * Beta);
        }
        private bool Glauber(double deltaE) {
            return Rnd.NextDouble() < (1.0 / (1.0 + Math.Exp(deltaE * Beta)));
        }
        #endregion

        #region Delegates
        //Signaturen
        public delegate void DynamicsAlgorithm(Spin chosen);
        public delegate bool AcceptanceFunction(int nearestNeighbourEnergy, int magnetizationEnergy);

        public readonly Dictionary<string, AcceptanceFunction> Accepts;
        public readonly Dictionary<string, DynamicsAlgorithm> Dynamics;
        //Aktuelle Algorithmen
        public AcceptanceFunction Accept;
        public DynamicsAlgorithm Dynamic;
        #endregion

    }
}
