using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows;

using IsingModern.Ising;

namespace IsingModern.ViewModel {
    class IsingRenderModel : FrameworkElement {
        private IsingModel model;
        private int viewsize = 600;// this should be dynamic, yo.
        private double ratio;

        public IsingRenderModel(int n = 50) {
            model = new IsingModel(n);
            ratio = 600 / n;
        }

        protected override void OnRender(DrawingContext dc) {
            var pen = new Pen();
            int counter = 0;
            foreach(var x in model.Points) {
                var rect = new Rect((counter % model.N) * ratio, (counter / model.N) * ratio, ratio, ratio);
                dc.DrawRectangle(x.Color, pen, rect);
                counter++;
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);

            // work out which "cell" was clicked on and change
            // appropriate spin state value in Boolean array

            InvalidateVisual(); // force OnRender() call
        }
    }
}
