using System.Collections.Generic;
using System.Linq;
using IsingModern.ViewPages;


namespace IsingModern.Model {
    public sealed partial class Lattice {
        public int N;
        private int Count { get; set; }
        public Spin[] Spins { get; private set; }

        public static int ZeroSpins { get; set; }


        public Lattice(int n, double averageMagnetization) {
            {
                Coupling = 1;
                Field = 0;
                Beta = 1;
                Dynamic = SingleFlip;
                Accept = MetropolisCached;
                Accepts = new Dictionary<string, AcceptanceFunction>()
                {
                    {"Metropolis", MetropolisCached}, 
                    {"Glauber", GlauberCached}
                };
                Dynamics = new Dictionary<string, DynamicsAlgorithm>()
                {
                    {"SingleFlip", SingleFlip}, 
                    {"Kawasaki", Kawasaki}
                };
            }
            N = n;
            var spin2DArray = new Spin[N, N]; //initalize with 2D array to make initalization easier 
            Spins = new Spin[N * N];
            Count = 0;
            double seed = 1 - 0.5 * (averageMagnetization + 1);
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    var r = Rnd.NextDouble();
                    var val = r < seed ? -1 : 1;
                    Spins[Count] = spin2DArray[i, j] = new Spin(val, Count);
                    Count++;
                }
            }
            InitializeNeighbours(spin2DArray, N);
            SetBoundary(true);
            ZeroSpins = Spins.Aggregate(0, (sum, spin) => sum + (spin.Value == 0 ? 1 : 0));
        }


        public void ScaleLattice(bool down) {
            int count = 0;
            int newN = down ? N / 2 : N * 2;
            var spins2D = new Spin[newN, newN];
            var spins = new Spin[newN * newN];
            for(var x = 0; x < newN; x++) {
                for(var y = 0; y < newN; y++) {
                    spins[count] = spins2D[x, y] = new Spin(GetScaledValue(x, y, down), count);
                    count++;
                }
            }
            InitializeNeighbours(spins2D, newN);
            this.N = newN;
            this.Count = count;
            this.Spins = spins;
            ZeroSpins = Spins.Aggregate(0, (sum, spin) => sum + (spin.Value == 0 ? 1 : 0));
            IsingRender.Current.UpdateHelpLines();
            this.UpdateStats();
        }

        private int GetSpinValue(int x, int y) {
            return Spins[x * N + y].Value;
        }

        private int GetScaledValue(int x, int y, bool down) {
            if(down) { //when downscaling use average of the previous 4 cells
                var a = new int[4];
                a[0] = GetSpinValue(x * 2, y * 2);
                a[1] = GetSpinValue(x * 2, y * 2 + 1);
                a[2] = GetSpinValue(x * 2 + 1, y * 2);
                a[3] = GetSpinValue(x * 2 + 1, y * 2 + 1);

                var countVal = new int[3]; //count occurence of all three possible values
                for(var v = 0; v < 3; v++) {
                    countVal[v] = a.Aggregate(0, (c, val) => c + (val == v - 1 ? 1 : 0));
                    if(countVal[v] > 2) return v - 1;
                    if(countVal[v] == 2 && v == 1) return 0; //favor boundaries over non boundaries
                }
                if(countVal[0] < 2) return 1;
                if(countVal[2] < 2) return -1;
                return Rnd.NextDouble() < 0.5 ? -1 : 1; //spin 1 and -1 have 2 -- return random value
            } else { //when upscaling give the values on the corresponding position
                return GetSpinValue(x / 2, y / 2);
            }

        }

        private void InitializeNeighbours(Spin[,] points, int N) {
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    var n = points[(i - 1 + N) % N, j];
                    var e = points[i, (j + 1) % N];
                    var s = points[(i + 1) % N, j];
                    var w = points[i, (j - 1 + N) % N];
                    points[i, j].SetNeighbours(n, e, w, s);
                }
            }
        }

        public void SetBoundary(bool periodic) {
            foreach(var p in Boundary) {
                if(p.Value == 0 && periodic) ZeroSpins++;
                if(p.Value != 0 && !periodic) ZeroSpins--;
                p.Value = periodic ? Rnd.Next(2) * 2 - 1 : 0;
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


        private Spin RandomSpin() {
            return Spins[Rnd.Next(N * N)];
        }

        public void Randomize() {
            foreach(var p in Spins) {
                if(p.Value != 0) {
                    p.Value = Rnd.NextDouble() > 0.5 ? -1 : 1;
                }
            }
            UpdateStats();
        }

        #region Hamiltonian




        #endregion
    }
}





