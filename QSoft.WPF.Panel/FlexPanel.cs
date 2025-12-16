using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        SpaceBetween,
        SpaceEvenly
    }

    public enum AlignItems
    {
        Start,
        End,
        Center,
        Stretch,
        //BaeseLine
    }

    public enum AlignSelf
    {
        Auto,
        Start,
        End,
        Center,
        Stretch,
        //BaeseLine
    }

    public class FlexPanel : System.Windows.Controls.Panel
    {
        public readonly static DependencyProperty JustifyContentProperty = DependencyProperty.Register("JustifyContent", typeof(JustifyContent), typeof(FlexPanel), new FrameworkPropertyMetadata(JustifyContent.Start, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public JustifyContent JustifyContent
        {
            set => this.SetValue(JustifyContentProperty, value);
            get => (JustifyContent)GetValue(JustifyContentProperty);
        }

        public readonly static DependencyProperty AlignItemsProperty = DependencyProperty.Register("AlignItems", typeof(AlignItems), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignItems.Start, FrameworkPropertyMetadataOptions.AffectsMeasure));
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

        public static readonly DependencyProperty AlignSelfProperty = DependencyProperty.RegisterAttached("AlignSelf", typeof(AlignSelf), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignSelf.Auto, FrameworkPropertyMetadataOptions.AffectsParentArrange|FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static AlignSelf GetAlignSelf(DependencyObject obj) => (AlignSelf)obj.GetValue(AlignSelfProperty);
        public static void SetAlignSelf(DependencyObject obj, AlignSelf value) => obj.SetValue(AlignSelfProperty, value);

        public static readonly DependencyProperty GrowProperty = DependencyProperty.RegisterAttached("Grow", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetGrow(DependencyObject obj) => (double)obj.GetValue(GrowProperty);
        public static void SetGrow(DependencyObject obj, double value) => obj.SetValue(GrowProperty, value);

        public static readonly DependencyProperty BasisProperty = DependencyProperty.RegisterAttached("Basis", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange|FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetBasis(DependencyObject obj) => (double)obj.GetValue(BasisProperty);
        public static void SetBasis(DependencyObject obj, double value) => obj.SetValue(BasisProperty, value);

        //public static readonly DependencyProperty ShrinkProperty = DependencyProperty.RegisterAttached("Shrink", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //public static double GetShrink(DependencyObject obj) => (double)obj.GetValue(ShrinkProperty);
        //public static void SetShrink(DependencyObject obj, double value) => obj.SetValue(ShrinkProperty, value);
        static readonly DependencyPropertyDescriptor MaxWidthDesciptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxWidthProperty, typeof(FrameworkElement));
        static readonly DependencyPropertyDescriptor MaxHeightDesciptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxHeightProperty, typeof(FrameworkElement));
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if(visualAdded is FrameworkElement addfe)
            {
                MaxWidthDesciptor.AddValueChanged(addfe, OnMaxWidthChanged);
                MaxHeightDesciptor.AddValueChanged(addfe, OnMaxHeightChanged);
            }
            if (visualRemoved is FrameworkElement removefe)
            {
                MaxWidthDesciptor.RemoveValueChanged(removefe, OnMaxWidthChanged);
                MaxHeightDesciptor.RemoveValueChanged(removefe, OnMaxHeightChanged);
            }
        }


        void OnMaxWidthChanged(object? sender, EventArgs e)
        {
            if (this.FlexDirection != FlexDirection.Row) return;
            if (sender is FrameworkElement fe)
            {
                if(fe.MaxWidth != double.PositiveInfinity && FlexPanel.GetBasis(fe) > 0)
                {
                    if(fe.MaxWidth != fe.ActualWidth)
                    {
                        this.InvalidateMeasure();
                    }
                }
            }
        }

        void OnMaxHeightChanged(object? sender, EventArgs e)
        {
            if (this.FlexDirection != FlexDirection.Column) return;
            if (sender is FrameworkElement fe)
            {
                if (fe.MaxHeight != double.PositiveInfinity && FlexPanel.GetBasis(fe) > 0)
                {
                    if (fe.MaxHeight != fe.ActualHeight)
                    {
                        this.InvalidateMeasure();
                    }
                }
            }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
            {
                return new Size(0, 0);
            }
            var totalGap = TotalGap();


            var desiredSize = new Size(0, 0);
            var remainingSize = availableSize;

            bool isRow = this.FlexDirection == FlexDirection.Row;
            remainingSize.Width = Math.Max(0.0, remainingSize.Width - (this.Padding.Left + this.Padding.Right));
            remainingSize.Height = Math.Max(0.0, remainingSize.Height - (this.Padding.Top + this.Padding.Bottom));
            foreach (FrameworkElement child in InternalChildren)
            {
                if (child == null) continue;
                Size sz = new(remainingSize.Width, remainingSize.Height);
                var basis = GetBasis(child);

                child.Measure(remainingSize);

                var childDesiredSize = child.DesiredSize;

                if (isRow && basis > 0)
                {
                    if(basis > child.MaxWidth)
                    {
                        basis = child.MaxWidth;
                    }
                    else if(basis < child.MinWidth)
                    {
                        basis = child.MinWidth;
                    }
                    childDesiredSize.Width = basis;
                }
                else if (!isRow && basis > 0)
                {
                    if (basis > child.MaxHeight)
                    {
                        basis = child.MaxHeight;
                    }
                    else if (basis < child.MinHeight)
                    {
                        basis = child.MinHeight;
                    }
                    childDesiredSize.Height = basis;
                }
                if (isRow)
                {
                    desiredSize.Width += childDesiredSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);
                    remainingSize.Width = Math.Max(0.0, remainingSize.Width - childDesiredSize.Width);
                }
                else
                {
                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                    desiredSize.Height += childDesiredSize.Height;
                    remainingSize.Height = Math.Max(0.0, remainingSize.Height - childDesiredSize.Height);
                }
            }

            
            if (isRow)
            {
                desiredSize.Width += totalGap;
            }
            else
            {
                desiredSize.Height += totalGap;
            }

            desiredSize.Width += this.Padding.Left + this.Padding.Right;
            desiredSize.Height += this.Padding.Top + this.Padding.Bottom;

            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"{this.Name} MeasureOverride: {desiredSize}");
#endif

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return base.ArrangeOverride(finalSize);

            System.Diagnostics.Debug.WriteLine($"{this.Name} ArrangeOverride: {finalSize}");

            Dictionary<FrameworkElement, Rect> rc = [];
            foreach (FrameworkElement child in InternalChildren)
            {
                Rect rcc = new(0, 0, child.DesiredSize.Width, child.DesiredSize.Height);
                var basis = GetBasis(child);
                if (basis != 0)
                {
                    if (this.FlexDirection == FlexDirection.Row)
                    {
                        if (basis > child.MaxWidth)
                        {
                            basis = child.MaxWidth;
                        }
                        else if (basis < child.MinWidth)
                        {
                            basis = child.MinWidth;
                        }
                        rcc.Width = basis;
                    }
                    else
                    {
                        if (basis > child.MaxHeight)
                        {
                            basis = child.MaxHeight;
                        }
                        else if (basis < child.MinHeight)
                        {
                            basis = child.MinHeight;
                        }
                        rcc.Height = basis;
                    }
                }

                rc.Add(child, rcc);
            }

            Dictionary<FrameworkElement, double> grows = [];
            foreach (FrameworkElement child in InternalChildren)
            {
                grows.Add(child, Math.Max(GetGrow(child), 0));
            }
            if (grows.Values.Any(static x => x >0))
            {
                this.CalacGrow(rc, finalSize, grows);
            }
            else
            {
                CalacJustifyContent(rc, finalSize);
            }
            CalacAlignItems(rc, finalSize);
            foreach (var oo in rc)
            {
                oo.Key.Arrange(oo.Value);
                System.Diagnostics.Debug.WriteLine(oo.Value);
            }
            return finalSize;
        }
        void CalacGrow(Dictionary<FrameworkElement, Rect> els, Size finalSize, Dictionary<FrameworkElement, double> grows)
        {
            var item_w = 0.0;
            var item_h = 0.0;
            double x = this.Padding.Left;
            double y = this.Padding.Top;
            var child_zerogrow = grows.Where(x => x.Value == 0);
            var sum = grows.Values.Sum();
            var totalgap = TotalGap();
            switch (this.FlexDirection)
            {
                case FlexDirection.Row:
                    var zerogrow_w = child_zerogrow.Sum(x=>x.Key.DesiredSize.Width);
                    var iw = Math.Max(finalSize.Width - zerogrow_w - totalgap - this.Padding.Left - this.Padding.Right, 0);
                    
                    iw = iw / sum;
                    foreach(var oo in els.Select((x,idx) => x.Key))
                    {
                        item_w = grows[oo] * iw;
                        if(item_w <= 0)
                        {
                            item_w = oo.DesiredSize.Width;
                        }
                        els[oo] = new Rect()
                        {
                            X = x,
                            Y = y,
                            Height = oo.DesiredSize.Height,
                            Width = item_w,
                        };
                        x += item_w + this.Gap;
                    }
                    break;
                case FlexDirection.Column:
                    var zerogrow_h = child_zerogrow.Sum(x => x.Key.DesiredSize.Height);
                    
                    var ih = Math.Max(finalSize.Height - zerogrow_h - totalgap - this.Padding.Top - this.Padding.Bottom, 0);

                    ih = ih / sum;
                    foreach (var oo in els.Select((x, idx) => x.Key))
                    {
                        item_h = grows[oo] * ih;
                        if (item_h <= 0)
                        {
                            item_h = oo.DesiredSize.Height;
                        }
                        els[oo] = new Rect()
                        {
                            X = x,
                            Y = y,
                            Height = item_h,
                            Width = oo.DesiredSize.Width,
                        };
                        y += item_h + this.Gap;
                    }
                    break;
            }
        }

        void CalacAlignItems(Dictionary<FrameworkElement, Rect> els, Size finalSize)
        {
            foreach (var oo in els)
            {
                var alignitem = GetAlignSelf(oo.Key) switch
                {
                    AlignSelf.Stretch => AlignItems.Stretch,
                    AlignSelf.Center => AlignItems.Center,
                    AlignSelf.Start => AlignItems.Start,
                    AlignSelf.End => AlignItems.End,
                    _ => this.AlignItems
                };
                var rc = oo.Value;
                switch (alignitem)
                {
                    case AlignItems.Start:
                        {
                            switch (this.FlexDirection)
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
                            switch (this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = finalSize.Height - oo.Key.DesiredSize.Height - this.Padding.Bottom;
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
                            switch (this.FlexDirection)
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
                            switch (this.FlexDirection)
                            {
                                case FlexDirection.Row:
                                    rc.Y = this.Padding.Top;
                                    rc.Height = Math.Max(finalSize.Height - this.Padding.Top - this.Padding.Bottom, 0);
                                    break;
                                case FlexDirection.Column:
                                    rc.X = this.Padding.Left;
                                    rc.Width = Math.Max(finalSize.Width - this.Padding.Left - this.Padding.Right, 0);
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
            var totalgap = 0.0;
            if(this.InternalChildren.Count >1)
            {
                totalgap = this.Gap * (InternalChildren.Count - 1);
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
                            using (var enumerator = els.GetEnumerator())
                            {
                                x = this.Padding.Left;
                                while (enumerator.MoveNext())
                                {
                                    var kvp = enumerator.Current;
                                    Rect rcc = els[kvp.Key];
                                    rcc.X = x;
                                    x = x + rcc.Width + this.Gap;
                                    els[kvp.Key] = rcc;
                                }
                            }
                            break;
                        case FlexDirection.Column:
                            using (var enumerator = els.GetEnumerator())
                            {
                                y = this.Padding.Top;
                                while (enumerator.MoveNext())
                                {
                                    var kvp = enumerator.Current;
                                    Rect rcc = els[kvp.Key];
                                    rcc.Y = y;
                                    y = y + rcc.Height + this.Gap;
                                    els[kvp.Key] = rcc;
                                }
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
                                Rect rcc = els[child];
                                item_w = rcc.Width;
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
                                Rect rcc = els[child];
                                item_h = rcc.Height;
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
                            var totalw = els.Values.Sum(x => x.Width);
                            var totalgap = this.TotalGap();
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                Rect rcc = els[oo];
                                item_w = rcc.Width;
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
                            var totalh = els.Values.Sum(x => x.Height);
                            var totalgap_h = this.Gap * (els.Count - 1);
                            if (totalgap_h <= 0)
                            {
                                totalgap_h = 0;
                            }
                            y = (finalSize.Height - totalh - totalgap_h) / 2;
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                Rect rcc = els[oo];
                                item_h = rcc.Height;
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
                            var totalgapw = this.TotalGap();
                            var totalw = els.Values.Sum(x => x.Width);
                            var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw - totalw);
                            if (iw < 0)
                            {
                                iw = 0;
                            }
                            else
                            {
                                iw = iw / (InternalChildren.Count * 2);
                            }

                            foreach (var oo in els.Select(x => x.Key))
                            {
                                x = x + iw;
                                item_w = els[oo].Width;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                                x += iw + item_w + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var totalh = els.Values.Sum(x => x.Height);
                            var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph - totalh);
                            if (ih < 0)
                            {
                                ih = 0;
                            }
                            else
                            {
                                ih = ih / (InternalChildren.Count * 2);
                            }
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                y = y + ih;
                                item_h = els[oo].Height;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = item_h,
                                    Width = finalSize.Width,
                                };
                                y += ih + item_h + this.Gap;
                            }
                            break;
                    }
                    break;
                case JustifyContent.SpaceEvenly:
                    {
                        switch(this.FlexDirection)
                        {
                            case FlexDirection.Row:
                                var totalgapw = this.TotalGap();
                                var totalw = els.Values.Sum(x => x.Width);
                                var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw - totalw);
                                if (iw < 0)
                                {
                                    iw = 0;
                                }
                                else
                                {
                                    iw = iw / (InternalChildren.Count + 1);
                                }
                                x = x + iw;
                                foreach (var oo in els.Select(x => x.Key))
                                {
                                    item_w = els[oo].Width;
                                    els[oo] = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = finalSize.Height,
                                        Width = item_w,
                                    };
                                    x += iw + item_w + this.Gap;
                                }
                                break;
                            case FlexDirection.Column:
                                var totalgaph = this.TotalGap();
                                var totalh = els.Values.Sum(x => x.Height);
                                var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph - totalh);
                                if (ih < 0)
                                {
                                    ih = 0;
                                }
                                else
                                {
                                    ih = ih / (InternalChildren.Count + 1);
                                }
                                y = y + ih;
                                foreach (var oo in els.Select(x => x.Key))
                                {
                                    item_h = els[oo].Height;
                                    els[oo] = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = item_h,
                                        Width = finalSize.Width,
                                    };
                                    y += ih + item_h + this.Gap;
                                }
                                break;
                        }
                    }
                    break;
                case JustifyContent.SpaceBetween:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.TotalGap();
                            var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw);
                            if (iw < 0)
                            {
                                iw = 0;
                            }
                            else
                            {
                                iw = iw / InternalChildren.Count;
                            }
                            for (int i = 0; i < els.Count; i++)
                            {
                                var child = els.ElementAt(i).Key;

                                item_w = els[child].Width;
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
                                x += iw + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph);
                            if (ih < 0)
                            {
                                ih = 0;
                            }
                            else
                            {
                                ih = ih / InternalChildren.Count;
                            }
                            for (int i = 0; i < els.Count; i++)
                            {
                                var child = els.ElementAt(i).Key;

                                item_h = els[child].Height;
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
                                y += ih + this.Gap;
                            }
                            break;

                    }

                    break;

            }
        }
    }
}