using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;



namespace IsingModern.Ising {
    public partial class IsingModel {
        public int N;
        public readonly Point[,] points;
        private Random r = new Random();

        public IsingModel(int n) {
            N = n;
            points = new Point[N, N];
            var r = new Random();
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    points[i, j] = new Point(-1);
                    if(i == 0 || j == 0 || i == N - 1 || j == N - 1) {
                        points[i, j] = new Point(0); ;
                    }
                }
            }
            points[0, 1].Value = 5;
        }

        private void InitializeNeighbours() {
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    Point n, e, s, w;
                    n = points[(i - 1 + N) % N, j];
                    e = points[i, (j + 1) % N];
                    s = points[(i + 1) % N, j];
                    w = points[i, (j - 1 + N) % N];

                    points[i, j].SetNeighbours(n, e, w, s);
                }
            }
        }

        public virtual void Randomize() {
            for(int i = 1; i < N - 1; i++) {
                for(int j = 1; j < N - 1; j++) {
                    points[i, j].Value = r.NextDouble() > 0.5 ? -1 : 1;
                }
            }
        }

        internal IEnumerable<Rectangle> GetRectangles() {
            for(int j = 0; j < N; j++) {
                for(int i = 0; i < N; i++) {
                    yield return points[i, j].Rectangle;
                }
            }
        }
    }
}





