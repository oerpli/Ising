using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace IsingModern.Ising {
    public partial class IsingModel {
        static private int instanceNumber = 0;
        public int InstanceNumber { get; private set; }
        public int N;
        public int Count { get; private set; }
        public Point[] Points { get; private set; }

        private Random r = new Random();

        public IsingModel(int n) {
            InstanceNumber = instanceNumber++;
            Point[,] points;
            N = n;
            points = new Point[N, N];
            Points = new Point[N * N];
            var r = new Random();
            Count = 0;
            for(int i = 0; i < N; i++) {
                for(int j = 0; j < N; j++) {
                    Points[Count++] = points[i, j] = new Point(r.Next(2) * 2 - 1);
                }
            }
            SetBoundary(true);
            //Current = this;
            InitializeNeighbours(points);
        }
        private void InitializeNeighbours(Point[,] points) {
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

        public void SetBoundary(bool periodic) {
            var r = new Random();
            foreach(var p in Boundary) {
                p.Value = periodic ? r.Next(2)*2-1 : 0;
            }
        }

        private IEnumerable<Point> Boundary {
            get {
                for(int i = 0; i < N; i++) {
                    for(int j = 0; j < N; j++) {
                        if(i == 0 || j == 0 || i == N - 1 || j == N - 1) {
                            yield return Points[N * j + i];
                        }
                    }
                }
            }
        }

        public virtual void Randomize() {
            foreach(var p in Points) {
                if(p.Value != 0) {
                    p.Value = r.NextDouble() > 0.5 ? -1 : 1;
                }
            }
        }



        #region rendering
        //public static IsingModel Current;

        public IEnumerable<SolidColorBrush> RenderColors {
            get {
                foreach(var p in Points) {
                    yield return p.Color;
                }
            }
        }



        public String TestString { get { return "TEST"; } }
        #endregion
    }
}





