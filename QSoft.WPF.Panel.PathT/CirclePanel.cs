using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            foreach (FrameworkElement oo in this.InternalChildren)
            {
                oo.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }
        Point m_CenterXy = new Point(0, 0);
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            var angele_ = 360.0/ this.Children.Count;
            var center_x = finalSize.Width / 2;
            var center_y = finalSize.Height / 2;
            m_CenterXy = new Point(center_x, center_y);

            var angele = 0.0;
            foreach (FrameworkElement oo in this.InternalChildren)
            {
                var pt = FromAngle(angele, m_CenterXy, finalSize.Width/2, finalSize.Height / 2);
                oo.Arrange(new Rect(pt.X, pt.Y, oo.DesiredSize.Width, oo.DesiredSize.Height));
                angele = angele + angele_;
            }

            return base.ArrangeOverride(finalSize);

        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            var angele_ = 360.0 / this.Children.Count;
            for(int i=0; i < this.Children.Count; i++)
            {
                var angele = angele_ * i;
                var pt = FromAngle(angele, m_CenterXy, this.ActualWidth / 2, this.ActualHeight / 2);

                dc.DrawLine(new Pen(Brushes.Green, 1), m_CenterXy, pt);
                dc.DrawEllipse(null, new Pen(Brushes.Red, 1), m_CenterXy, this.ActualWidth / 2, this.ActualHeight / 2);
            }
        }


        Point FromAngle(double angle, Point center, double w, double h)
        {
            var raduis = Math.PI / 180 * (angle - 90);
            var y = Math.Sin(raduis) * h;
            var x = Math.Cos(raduis) * w;
            var pt_x = x + center.X;
            var pt_y = y + center.Y;
            return new Point((int)pt_x, (int)pt_y);
        }
    }
}
