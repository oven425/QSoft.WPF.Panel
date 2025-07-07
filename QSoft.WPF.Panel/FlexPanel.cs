using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

//https://w3c.hexschool.com/flexbox/4a029043
namespace QSoft.WPF.Panel
{
    public enum JustifyContent
    {
        Left,
        Right,
        Center,
        SpaceAround,
        SpaceBetween
    }

    public enum AlignItems
    {
        Top,
        Bottom,
        Center,
        Stretch,
        BaeseLine
    }
    public class FlexPanel: System.Windows.Controls.Panel
    {
        public readonly static DependencyProperty JustifyContentProperty = DependencyProperty.Register("JustifyContent", typeof(JustifyContent), typeof(FlexPanel), new FrameworkPropertyMetadata(JustifyContent.Left, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public JustifyContent JustifyContent
        {
            set=>this.SetValue(JustifyContentProperty, value);
            get=>(JustifyContent)GetValue(JustifyContentProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child?.Measure(availableSize);
                
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var item_w = finalSize.Width / InternalChildren.Count;
            double x = 0;
            foreach (UIElement child in InternalChildren)
            {
                switch(this.JustifyContent)
                {
                    case JustifyContent.Left:
                        {
                            item_w = child.DesiredSize.Width;
                        }
                        break;
                }
                child.Arrange(new Rect()
                {
                    X = x,
                    Y = 0,
                    Height = finalSize.Height,
                    Width = item_w,
                });
                x=x+ item_w;
            }
            return finalSize;
        }
    }
}
