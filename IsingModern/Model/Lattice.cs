using System;
using System.Linq;
using System.Collections.Generic;


namespace IsingModern.Ising {
    public partial class Lattice {
        public int InstanceNumber { get; private set; }
        public int N;
        public int Count { get; private set; }
        public Spin[] Spins { get; private set; }


        public Lattice(int n) {
            {
                J = 1;
                h = 0;
                Beta = 1;
                dynamic = SingleFlip;
                accept = Metropolis;
                accepts = new Dictionary<string, AcceptanceFunction>()
                {
                    {"Metropolis", Metropolis}, 
                    {"Glauber", Glauber}
                };
            }
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
            InitializeNeighbours(points);
            SetBoundary(true);
            //Current = this;
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
            UpdateStats();
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


        internal Spin RandomSpin() {
            return Spins[r.Next(N * N)];
        }

        public virtual void Randomize() {
            foreach(var p in Spins) {
                if(p.Value != 0) {
                    p.Value = r.NextDouble() > 0.5 ? -1 : 1;
                }
            }
            UpdateStats();
        }

        #region Hamiltonian




        #endregion
    }
}





