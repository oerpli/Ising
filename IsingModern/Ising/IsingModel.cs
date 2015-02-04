using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Ising.Ising {
    public partial class IsingModel {
        public int N;
        public int[,] model;
        private Random r = new Random();

        public IsingModel(int n) {
            N = n;
            model = new int[N, N];
            var r = new Random();
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    model[i, j] = r.NextDouble() > 0.5 ? -1 : 1;
                    if(i == 0 || j == 0 || i == N - 1 || j == N - 1) {
                        model[i, j] = 0;
                    }
                }
            }
        }

        public virtual void DoStuff() {
            for(int i = 1; i < N - 1; i++) {
                for(int j = 1; j < N - 1; j++) {
                    model[i, j] = r.NextDouble() > 0.5 ? -1 : 1;
                }
            }
        }
    }
}





