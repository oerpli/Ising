using System;
using System.Collections.Generic;


namespace IsingModern.Ising {
    public partial class Lattice {
        public int InstanceNumber { get; private set; }
        public int N;
        public int Count { get; private set; }
        public Spin[] Spins { get; private set; }

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
        }
    }
}





