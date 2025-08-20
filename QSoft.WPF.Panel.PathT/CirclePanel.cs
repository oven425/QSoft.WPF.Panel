using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QSoft.WPF.Panel
{
    //https://www.jointjs.com/demos/circular-layout
    public class CirclePanel: System.Windows.Controls.Panel
    {
        
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
            var angele = 360.0/ this.Children.Count;
            var center_x = finalSize.Width / 2;
            var center_y = finalSize.Height / 2;

            var pt = FromAngle(angele, center_x, center_y);
        }

        protected override void OnRender(DrawingContext dc)
        {

            base.OnRender(dc);
        }


        Point FromAngle(double angle, double center_x, double center_y)
        {
            var raduis = Math.PI / 180 * (angle - 90);
            var y = Math.Sin(raduis) * 100;
            var x = Math.Cos(raduis) * 100;
            var pt_x = x + center_x;
            var pt_y = y + center_y;
            return new Point((int)pt_x, (int)pt_y);
        }
    }
}
