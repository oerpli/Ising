using System;
using System.Collections.Generic;


namespace IsingModern.Ising {
    public partial class Lattice {
        public int InstanceNumber { get; private set; }
        public int N;
        public int Count { get; private set; }
        public Spin[] Spins { get; private set; }

        /*Field parameters*/
        public double J;
        public double h;

        public double Beta; /*inverse temperature*/
        public double TotalEnergy;

        private Random r = new Random();

        public Lattice(int n) {
            Spin[,] points;
            N = n;
            points = new Spin[N, N];
            Spins = new Spin[N * N];
            var r = new Random();
            Count = 0;
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    Spins[Count] = points[i, j] = new Spin(-1, Count);
                    Count++;
                }
            }
            SetBoundary(true);
            //Current = this;
            InitializeNeighbours(points);
            h = 0.0; 
            J = 1.0; 
            Beta = 1.0; 
            TotalEnergy = 0.0; 
        }
        private void InitializeNeighbours(Spin[,] points) {
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    Spin n, e, s, w;
                    n = points[(i - 1 + N) % N, j];
                    e = points[i, (j + 1) % N];
                    s = points[(i + 1) % N, j];
                    w = points[i, (j - 1 + N) % N];

                    points[i, j].SetNeighbours(n, e, w, s);
                }
            }
        }

        public void SetBoundary(bool periodic) {
            var r = new Random();
            foreach(var p in Boundary) {
                p.Value = periodic ? -1 : 0;
            }
        }

        private IEnumerable<Spin> Boundary {
            get {
                for(int i = 0; i < N; i++) {
                    for(int j = 0; j < N; j++) {
                        if(i == 0 || j == 0 || i == N - 1 || j == N - 1) {
                            yield return Spins[N * j + i];
                        }
                    }
                }
            }
        }

        public virtual void Randomize() {
            foreach(var p in Spins) {
                if(p.Value != 0) {
                    p.Value = r.NextDouble() > 0.5 ? -1 : 1;
                }
            }
            UpdateTotalEnergy(); 
        }

        #region Hamiltonian
        public void UpdateTotalEnergy() {
            TotalEnergy = 0.0;
            for(int i = 0; i < N; i++) {
                TotalEnergy -= h * Spins[i].Value;
                foreach(var Spin in Spins[i].Neighbours) {
                    TotalEnergy -= 0.5 * J * Spins[i].Value * Spin.Value;
                }
            }
        }

        public double CalculateLocalEnergy(Spin Chosen) {
            double LocalEnergy = 0.0;
            foreach(var Neighbour in Chosen.Neighbours) {
                LocalEnergy -= Chosen.Value * Neighbour.Value;
            }
            LocalEnergy *= J;
            LocalEnergy -= h * Chosen.Value;
            return LocalEnergy;
        }

        public double CalculateLocalEnergy(Spin Chosen1, Spin Chosen2)
        {
            double LocalEnergy = 0.0;
            
            foreach (var Neighbour in Chosen1.Neighbours)
            {
                LocalEnergy -= J*Chosen1.Value * Neighbour.Value;
            }

            foreach (var Neighbour in Chosen2.Neighbours)
            {
                LocalEnergy -= J * Chosen2.Value * Neighbour.Value;
            }

            LocalEnergy += J * Chosen1.Value * Chosen2.Value; 

            return LocalEnergy;
        }


        #endregion

        #region Dynamics
        public void SingleFlip() {
            Spin Chosen = Spins[r.Next(N*N)];
            
            double EnergyDifference = 0.0 - CalculateLocalEnergy(Chosen);
            
            Chosen.ToggleSpin();

            EnergyDifference += CalculateLocalEnergy(Chosen); 
            if (!Metropolis(EnergyDifference)) /* or Glauber(..,..) */
            {
                Chosen.ToggleSpin(); 
            }
            else
            {
                TotalEnergy += EnergyDifference; 
            }
        }

        public void Sweep() {
            for(int i = 0; i < N; i++) {
                SingleFlip();
            }
        }

        public void Kawasaki()
        {
            Spin Chosen = Spins[r.Next(N*N)];
            Spin Exchange = Chosen.Neighbours[r.Next(4)];
            if (Chosen.Value != Exchange.Value)
            {
                double EnergyDifference = 0.0 - CalculateLocalEnergy(Chosen, Exchange);
                Chosen.ToggleSpin();
                Exchange.ToggleSpin();
                EnergyDifference += CalculateLocalEnergy(Chosen, Exchange);
                if (!Metropolis(EnergyDifference))
                {
                    Chosen.ToggleSpin();
                    Exchange.ToggleSpin();
                }
                else
                {
                    TotalEnergy += EnergyDifference; 
                }
            }
        }

        public bool Metropolis(double DeltaE) {
            bool accept = true;
            if(DeltaE > 0.0) {
                if(r.NextDouble() > Math.Exp(DeltaE * Beta)) {
                    accept = false;
                }
            }
            return accept;
        }

        public bool Glauber(double DeltaE) {
            bool accept = true; 
            if(r.NextDouble() > (1.0 / (1.0 + Math.Exp(DeltaE * Beta)))) {
                accept = false;
            }
            return accept; 
        }
        #endregion
    }
}





