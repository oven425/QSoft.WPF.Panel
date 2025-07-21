using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

//https://w3c.hexschool.com/flexbox/4a029043
namespace QSoft.WPF.Panel
{
    public enum FlexDirection
    {
        Row,
        //RowReverse,
        Column,
        //ColumnReverse
    }
    public enum JustifyContent
    {
        Start,
        End,
        Center,
        SpaceAround,
        SpaceBetween
    }

    public enum AlignItems
    {
        Start,
        End,
        Center,
        Stretch,
        //BaeseLine
    }
    public class FlexPanel: System.Windows.Controls.Panel
    {
        public readonly static DependencyProperty JustifyContentProperty = DependencyProperty.Register("JustifyContent", typeof(JustifyContent), typeof(FlexPanel), new FrameworkPropertyMetadata(JustifyContent.SpaceBetween, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public JustifyContent JustifyContent
        {
            set=>this.SetValue(JustifyContentProperty, value);
            get=>(JustifyContent)GetValue(JustifyContentProperty);
        }

        public readonly static DependencyProperty AlignItemsProperty = DependencyProperty.Register("AlignItems", typeof(AlignItems), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignItems.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public AlignItems AlignItems
        {
            set => this.SetValue(AlignItemsProperty, value);
            get => (AlignItems)GetValue(AlignItemsProperty);
        }

        public readonly static DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(FlexPanel), new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public Thickness Padding
        {
            set => this.SetValue(PaddingProperty, value);
            get => (Thickness)GetValue(PaddingProperty);
        }

        public readonly static DependencyProperty GapProperty = DependencyProperty.Register("Gap", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public double Gap
        {
            set => this.SetValue(GapProperty, value);
            get => (double)GetValue(GapProperty);
        }

        public readonly static DependencyProperty FlexDirectionProperty = DependencyProperty.Register("FlexDirection", typeof(FlexDirection), typeof(FlexPanel), new FrameworkPropertyMetadata(FlexDirection.Row, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public FlexDirection FlexDirection
        {
            set => this.SetValue(FlexDirectionProperty, value);
            get => (FlexDirection)GetValue(FlexDirectionProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {

            foreach (UIElement child in InternalChildren)
            {
                child?.Measure(availableSize);
            }
            //return availableSize;
            var ll = InternalChildren.OfType<FrameworkElement>().ToList();
            var totalgap = TotalGap();
            var sz = new Size(0, 0);
            switch (this.FlexDirection)
            {
                case FlexDirection.Row:
                    sz.Width = ll.Sum(x => x.DesiredSize.Width) + totalgap;
                    sz.Height = ll.Max(x => x.DesiredSize.Height);
                    break;
                case FlexDirection.Column:
                    sz.Width = ll.Max(x => x.DesiredSize.Width);
                    sz.Height = ll.Sum(x => x.DesiredSize.Height) + totalgap;
                    break;
            }
            sz.Width = sz.Width + this.Padding.Left + this.Padding.Right;
            sz.Height = sz.Height + this.Padding.Top + this.Padding.Bottom;
            if(sz.Width >availableSize.Width)
            {
                sz.Width = availableSize.Width;
            }
            if(sz.Height > availableSize.Height)
            {
                sz.Height = availableSize.Height;
            }
            System.Diagnostics.Debug.WriteLine($"{this.Name} MeasureOverride: {sz}");
            return sz;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            System.Diagnostics.Debug.WriteLine($"{this.Name} ArrangeOverride: {finalSize}");

            Dictionary<FrameworkElement, Rect> rc = [];
            foreach (FrameworkElement child in InternalChildren)
            {
                rc.Add(child, new(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
            }
            CalacJustifyContent(rc, finalSize);
            CalacAlignItems(rc, finalSize);
            foreach (var oo in rc)
            {
                oo.Key.Arrange(oo.Value);
                System.Diagnostics.Debug.WriteLine(oo.Value);
            }
            return finalSize;
        }

        void CalacAlignItems(Dictionary<FrameworkElement, Rect> els, Size finalSize)
        {            
            foreach (var oo in els)
            {
                var rc = oo.Value;
                switch (this.AlignItems)
                {
                    case AlignItems.Start:
                        {
                            switch(this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = this.Padding.Top;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                    rc.X = this.Padding.Left;
                                    rc.Width = oo.Key.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.End:
                        {
                            switch(this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = finalSize.Height - oo.Key.DesiredSize.Height;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                    rc.X = finalSize.Width - oo.Key.DesiredSize.Width - this.Padding.Right;
                                    rc.Width = oo.Key.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.Center:
                        {
                            switch(this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = (finalSize.Height - oo.Key.DesiredSize.Height) / 2;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                    rc.X = (finalSize.Width - oo.Key.DesiredSize.Width) / 2;
                                    rc.Width = oo.Key.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.Stretch:
                        {
                            switch(this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = this.Padding.Top;
                                    rc.Height = finalSize.Height - this.Padding.Top-this.Padding.Bottom;
                                    break;
                                case FlexDirection.Column:
                                    rc.X = this.Padding.Left;
                                    rc.Width = finalSize.Width - this.Padding.Left - this.Padding.Right;
                                    break;
                            }
                            
                        }
                        break;
                }
                els[oo.Key] = rc;
            }
        }

        double TotalGap()
        {
            var totalgap = this.Gap * (InternalChildren.Count - 1);
            if (totalgap <= 0)
            {
                totalgap = 0;
            }
            return totalgap;
        }

        void CalacJustifyContent(Dictionary<FrameworkElement, Rect> els, Size finalSize)
        {
            var item_w = 0.0;
            var item_h = 0.0;
            double x = this.Padding.Left;
            double y = this.Padding.Top;
            
            switch (this.JustifyContent)
            {
                case JustifyContent.Start:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_w = oo.DesiredSize.Width;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                                x = x + item_w + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_h = oo.DesiredSize.Height;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = item_h,
                                    Width = finalSize.Width,
                                };
                                y = y + item_h + this.Gap;
                            }
                            break;
                    }
                    break;
                case JustifyContent.End:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            x = finalSize.Width - this.Padding.Right;
                            for (int i = els.Count - 1; i >= 0; i--)
                            {
                                var child = els.ElementAt(i).Key;
                                item_w = child.DesiredSize.Width;
                                x = x - item_w;

                                els[child] = new Rect()
                                {
                                    X = x,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                                x -= this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = finalSize.Height - this.Padding.Bottom;
                            for (int i = els.Count - 1; i >= 0; i--)
                            {
                                var child = els.ElementAt(i).Key;
                                item_h = child.DesiredSize.Height;
                                y = y - item_h;

                                els[child] = new Rect()
                                {
                                    X = 0,
                                    Y = y,
                                    Height = item_h,
                                    Width = child.DesiredSize.Width,
                                };
                                y -= this.Gap;
                            }
                            break;
                    }

                    break;
                case JustifyContent.Center:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            var totalw = els.Keys.Sum(x => x.DesiredSize.Width);
                            var totalgap = this.Gap * (els.Count - 1);
                            if (totalgap <= 0)
                            {
                                totalgap = 0;
                            }
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_w = oo.DesiredSize.Width;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                                x += item_w + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalh = els.Keys.Sum(x => x.DesiredSize.Height);
                            var totalgap_h = this.Gap * (els.Count - 1);
                            if (totalgap_h <= 0)
                            {
                                totalgap_h = 0;
                            }
                            y = (finalSize.Height - totalh - totalgap_h) / 2;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_h = oo.DesiredSize.Height;
                                els[oo] = new Rect()
                                {
                                    X = 0,
                                    Y = y,
                                    Height = item_h,
                                    Width = finalSize.Width,
                                };
                                y += item_h + this.Gap;
                            }
                            break;
                    }

                    break;
                case JustifyContent.SpaceAround:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.Gap * (els.Count - 1);
                            if (totalgapw <= 0)
                            {
                                totalgapw = 0;
                            }
                            var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw) / InternalChildren.Count;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_w = oo.DesiredSize.Width;

                                if (iw > item_w)
                                {
                                    item_w = iw;
                                }
                                els[oo] = new Rect()
                                {
                                    X = x + (iw - item_w) / 2,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                                x += iw + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.Gap * (els.Count - 1);
                            if (totalgaph <= 0)
                            {
                                totalgaph = 0;
                            }
                            var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph) / InternalChildren.Count;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                item_h = oo.DesiredSize.Height;

                                if (ih > item_h)
                                {
                                    item_h = ih;
                                }
                                els[oo] = new Rect()
                                {
                                    X = 0,
                                    Y = y + (ih - item_h) / 2,
                                    Height = item_h,
                                    Width = finalSize.Width,
                                };
                                y += ih + this.Gap;
                            }
                            break;
                    }

                    break;
                case JustifyContent.SpaceBetween:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.Gap * (els.Count - 1);
                            if (totalgapw <= 0)
                            {
                                totalgapw = 0;
                            }
                            var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw) / InternalChildren.Count;
                            for (int i = 0; i < els.Count; i++)
                            {
                                var child = els.ElementAt(i).Key;

                                item_w = child.DesiredSize.Width;
                                if (double.IsNaN(child.Width))
                                {
                                    //item_w = iw;
                                }
                                else if (item_w > iw)
                                {
                                    item_w = iw;
                                }
                                Rect rc;
                                if (i == 0)
                                {
                                    rc = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = finalSize.Height,
                                        Width = item_w,
                                    };
                                }
                                else if (i == InternalChildren.Count - 1)
                                {
                                    rc = new Rect()
                                    {
                                        X = x + (iw - item_w),
                                        Y = y,
                                        Height = finalSize.Height,
                                        Width = item_w,
                                    };
                                }
                                else
                                {
                                    rc = new Rect()
                                    {
                                        X = x + (iw - item_w) / 2,
                                        Y = y,
                                        Height = finalSize.Height,
                                        Width = item_w,
                                    };
                                }
                                els[child] = rc;
                                x += iw+this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.Gap * (els.Count - 1);
                            if (totalgaph <= 0)
                            {
                                totalgaph = 0;
                            }
                            var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph) / InternalChildren.Count;
                            for (int i = 0; i < els.Count; i++)
                            {
                                var child = els.ElementAt(i).Key;

                                item_h = child.DesiredSize.Height;
                                if (double.IsNaN(child.Height))
                                {
                                    //item_h = ih;
                                }
                                else if (item_w > ih)
                                {
                                    item_h = ih;
                                }
                                Rect rc;
                                if (i == 0)
                                {
                                    rc = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = item_h,
                                        Width = finalSize.Width,
                                    };
                                }
                                else if (i == InternalChildren.Count - 1)
                                {
                                    rc = new Rect()
                                    {
                                        X = x,
                                        Y = y + (ih - item_h),
                                        Height = item_h,
                                        Width = finalSize.Width,
                                    };
                                }
                                else
                                {
                                    rc = new Rect()
                                    {
                                        X = x,
                                        Y = y + (ih - item_h) / 2,
                                        Height = item_h,
                                        Width = finalSize.Width,
                                    };
                                }
                                els[child] = rc;
                                y += ih+this.Gap;
                            }
                            break;

                    }

                    break;

            }
        }
    }
}
