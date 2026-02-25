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
        RowReverse,
        Column,
        ColumnReverse
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

        public static readonly DependencyProperty ShrinkProperty = DependencyProperty.RegisterAttached("Shrink", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetShrink(DependencyObject obj) => (double)obj.GetValue(ShrinkProperty);
        public static void SetShrink(DependencyObject obj, double value) => obj.SetValue(ShrinkProperty, value);
        static readonly DependencyPropertyDescriptor MaxWidthDesciptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxWidthProperty, typeof(FrameworkElement));
        static readonly DependencyPropertyDescriptor MaxHeightDesciptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxHeightProperty, typeof(FrameworkElement));
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if(visualAdded is FrameworkElement addfe)
            {
                MaxWidthDesciptor.AddValueChanged(addfe, OnMaxWidthChanged);
                MaxHeightDesciptor.AddValueChanged(addfe, OnMaxHeightChanged);
                rc.Add(addfe, new Rect());
                grows.Add(addfe, 0);
            }
            if (visualRemoved is FrameworkElement removefe)
            {
                MaxWidthDesciptor.RemoveValueChanged(removefe, OnMaxWidthChanged);
                MaxHeightDesciptor.RemoveValueChanged(removefe, OnMaxHeightChanged);
                rc.Remove(removefe);
                grows.Remove(removefe);
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
            if (InternalChildren.Count == 0) return new Size(0, 0);

            var totalGap = TotalGap();
            var desiredSize = new Size(0, 0);
            bool isRow = this.FlexDirection switch
            { 
                FlexDirection.Row => true,
                FlexDirection.RowReverse => true,
                FlexDirection.Column => false,
                _=>false
            };

            foreach (FrameworkElement child in InternalChildren)
            {
                if (child == null) continue;
                var basis = GetBasis(child);
                child.Measure(availableSize);
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
                }
                else
                {
                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                    desiredSize.Height += childDesiredSize.Height;
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
        readonly Dictionary<FrameworkElement, double> grows = [];
        readonly Dictionary<FrameworkElement, Rect> rc = [];
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return base.ArrangeOverride(finalSize);

            var shrinkmode = this.FlexDirection switch
            { 
                FlexDirection.Row => this.DesiredSize.Width > finalSize.Width,
                FlexDirection.Column => this.DesiredSize.Height > finalSize.Height,
                _=>false
            };

            System.Diagnostics.Debug.WriteLine($"{this.Name} ArrangeOverride: {finalSize}");

            //rc.Clear();
            foreach (FrameworkElement child in InternalChildren)
            {
                Rect rcc = new(0, 0, child.DesiredSize.Width, child.DesiredSize.Height);
                var shrink = GetShrink(child);
                var basis = GetBasis(child);
                if (basis != 0)
                {
                    switch(this.FlexDirection)
                    {
                        case FlexDirection.Row:
                        case FlexDirection.RowReverse:
                            if (basis > child.MaxWidth)
                            {
                                basis = child.MaxWidth;
                            }
                            else if (basis < child.MinWidth)
                            {
                                basis = child.MinWidth;
                            }
                            rcc.Width = basis;
                            break;
                        case FlexDirection.Column:
                            if (basis > child.MaxHeight)
                            {
                                basis = child.MaxHeight;
                            }
                            else if (basis < child.MinHeight)
                            {
                                basis = child.MinHeight;
                            }
                            rcc.Height = basis;
                            break;
                    }
                }

                //rc.Add(child, rcc);
                rc[child] = rcc;
            }

            bool isclacgrow = false;
            //grows.Clear();
            foreach (FrameworkElement child in InternalChildren)
            {
                //grows.Add(child, Math.Max(GetGrow(child), 0));
                var grow = GetGrow(child);
                if(!isclacgrow && grow > 0)
                {
                    isclacgrow = true;
                }
                grows[child] = Math.Max(grow, 0);
            }
            //if (grows.Values.Any(static x => x >0))
            if(isclacgrow)
            {
                this.CalacGrow(rc, finalSize, grows, this.Gap);
            }
            else
            {
                CalacJustifyContent(rc, finalSize, this.JustifyContent, this.Padding, this.Gap);
            }
            CalacAlignItems(rc, finalSize);
            foreach (var oo in rc)
            {
                oo.Key.Arrange(oo.Value);
                System.Diagnostics.Debug.WriteLine(oo.Value);
            }
            return finalSize;
        }
        void CalacGrow(Dictionary<FrameworkElement, Rect> els, Size finalSize, Dictionary<FrameworkElement, double> grows, double gap)
        {
            var item_w = 0.0;
            var item_h = 0.0;
            double x = this.Padding.Left;
            double y = this.Padding.Top;
            //var child_zerogrow = grows.Where(x => x.Value == 0);
            var sum = 0.0;
            var zerogrow_w = 0.0;
            var zerogrow_h = 0.0;
            foreach (var oo in grows)
            {
                sum = sum + oo.Value;
                if(oo.Value == 0)
                {
                    zerogrow_w = zerogrow_w + oo.Key.DesiredSize.Width;
                    zerogrow_h = zerogrow_h + oo.Key.DesiredSize.Height;
                }
            }
            var totalgap = TotalGap();
            switch (this.FlexDirection)
            {
                case FlexDirection.Row:
                    //var zerogrow_w = child_zerogrow.Sum(x=>x.Key.DesiredSize.Width);
                    var iw = Math.Max(finalSize.Width - zerogrow_w - totalgap - this.Padding.Left - this.Padding.Right, 0);
                    iw = iw / sum;
                    //foreach(var oo in els.Select((x,idx) => x.Key))
                    //{
                    //    item_w = grows[oo] * iw;
                    //    if(item_w <= 0)
                    //    {
                    //        item_w = oo.DesiredSize.Width;
                    //    }
                    //    els[oo] = new Rect()
                    //    {
                    //        X = x,
                    //        Y = y,
                    //        Height = oo.DesiredSize.Height,
                    //        Width = item_w,
                    //    };
                    //    x += item_w + this.Gap;
                    //}

                    foreach (var oo in els)
                    {
                        item_w = grows[oo.Key] * iw;
                        if (item_w <= 0)
                        {
                            item_w = oo.Key.DesiredSize.Width;
                        }
                        var rcc = oo.Value;
                        rcc.Width = item_w;
                        rcc.X = x;
                        els[oo.Key] = rcc;
                        x += item_w + gap;
                    }
                    break;
                case FlexDirection.Column:
                    //var zerogrow_h = child_zerogrow.Sum(x => x.Key.DesiredSize.Height);
                    
                    var ih = Math.Max(finalSize.Height - zerogrow_h - totalgap - this.Padding.Top - this.Padding.Bottom, 0);

                    ih /= sum;
                    //foreach (var oo in els.Select((x, idx) => x.Key))
                    //{
                    //    item_h = grows[oo] * ih;
                    //    if (item_h <= 0)
                    //    {
                    //        item_h = oo.DesiredSize.Height;
                    //    }
                    //    els[oo] = new Rect()
                    //    {
                    //        X = x,
                    //        Y = y,
                    //        Height = item_h,
                    //        Width = oo.DesiredSize.Width,
                    //    };
                    //    y += item_h + this.Gap;
                    //}

                    foreach (var oo in els)
                    {
                        item_h = grows[oo.Key] * ih;
                        if (item_h <= 0)
                        {
                            item_h = oo.Key.DesiredSize.Height;
                        }
                        //els[oo] = new Rect()
                        //{
                        //    X = x,
                        //    Y = y,
                        //    Height = item_h,
                        //    Width = oo.DesiredSize.Width,
                        //};
                        var rcc = oo.Value;
                        rcc.Height = item_h;
                        rcc.Y = y;
                        els[oo.Key] = rcc;
                        y += item_h + gap;
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
                                case FlexDirection.RowReverse:
                                    rc.Y = this.Padding.Top;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
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
                                case FlexDirection.RowReverse:
                                    rc.Y = finalSize.Height - oo.Key.DesiredSize.Height - this.Padding.Bottom;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
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
                                case FlexDirection.RowReverse:
                                    rc.Y = (finalSize.Height - oo.Key.DesiredSize.Height) / 2;
                                    rc.Height = oo.Key.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
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
                                case FlexDirection.RowReverse:
                                    rc.Y = this.Padding.Top;
                                    rc.Height = Math.Max(finalSize.Height - this.Padding.Top - this.Padding.Bottom, 0);
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
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
            => this.InternalChildren.Count > 1 
            ? this.Gap * (InternalChildren.Count - 1)
            : 0;

        void CalacJustifyContent(Dictionary<FrameworkElement, Rect> els, Size finalSize, JustifyContent justify, Thickness padding, double gap)
        {
            double x = padding.Left;
            double y = padding.Top;
            var totalw = 0.0;
            var totalh = 0.0;
            var totaldsw = 0.0;
            var totaldsh = 0.0;
            foreach (var oo in els)
            {
                totalw = totalw + oo.Value.Width;
                totalh = totalh + oo.Value.Height;
                totaldsw = totaldsw + oo.Key.DesiredSize.Width;
                totaldsh = totaldsh + oo.Key.DesiredSize.Height;
            }
            switch (justify)
            {
                case JustifyContent.Start:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            //using (var enumerator = els.GetEnumerator())
                            //{
                            //    x = this.Padding.Left;
                            //    while (enumerator.MoveNext())
                            //    {
                            //        var kvp = enumerator.Current;
                            //        Rect rcc = els[kvp.Key];
                            //        rcc.X = x;
                            //        x = x + rcc.Width + this.Gap;
                            //        els[kvp.Key] = rcc;
                            //    }
                            //}
                            x = padding.Left;
                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x = x + rcc.Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = finalSize.Width - padding.Right;
                            //using (var enumerator = els.GetEnumerator())
                            //{
                            //    while (enumerator.MoveNext())
                            //    {
                            //        var kvp = enumerator.Current;
                            //        Rect rcc = els[kvp.Key];
                            //        x -= rcc.Width;
                            //        rcc.X = x;
                            //        x -= this.Gap;
                            //        els[kvp.Key] = rcc;
                            //    }
                            //}
                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                x -= rcc.Width;
                                rcc.X = x;
                                x -= gap;
                                els[oo.Key] = rcc;
                            }
                            break;
                        case FlexDirection.Column:
                            //using (var enumerator = els.GetEnumerator())
                            //{
                            //    y = this.Padding.Top;
                            //    while (enumerator.MoveNext())
                            //    {
                            //        var kvp = enumerator.Current;
                            //        Rect rcc = els[kvp.Key];
                            //        rcc.Y = y;
                            //        y = y + rcc.Height + this.Gap;
                            //        els[kvp.Key] = rcc;
                            //    }
                            //}
                            y = padding.Top;
                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                rcc.Y = y;
                                y = y + rcc.Height + gap;
                                els[oo.Key] = rcc;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            //using (var enumerator = els.GetEnumerator())
                            //{
                            //    y = finalSize.Height - this.Padding.Bottom;
                            //    while (enumerator.MoveNext())
                            //    {
                            //        var kvp = enumerator.Current;
                            //        Rect rcc = els[kvp.Key];
                            //        y -= rcc.Height;
                            //        rcc.Y = y;
                            //        y += -this.Gap;
                            //        els[kvp.Key] = rcc;
                            //    }
                            //}

                            y = finalSize.Height - padding.Bottom;
                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                y -= rcc.Height;
                                rcc.Y = y;
                                y += -gap;
                                els[oo.Key] = rcc;
                            }
                            break;
                    }
                    break;
                case JustifyContent.End:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            x = finalSize.Width - padding.Right;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = els.ElementAt(i).Key;
                            //    Rect rcc = els[child];
                            //    x = x - rcc.Width;
                            //    rcc.X = x;
                            //    els[child] = rcc;
                            //    x -= this.Gap;
                            //}
                            for (int i = InternalChildren.Count-1; i >=0; i--)
                            {
                                var child = (FrameworkElement)InternalChildren[i];
                                Rect rcc = els[child];
                                x = x - rcc.Width;
                                rcc.X = x;
                                els[child] = rcc;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = padding.Left;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = els.ElementAt(i).Key;
                            //    Rect rcc = els[child];
                            //    rcc.X = x;
                            //    els[child] = rcc;
                            //    x += rcc.Width + this.Gap;
                            //}
                            for (int i = els.Count - 1; i >= 0; i--)
                            {
                                var child = (FrameworkElement)InternalChildren[i];
                                Rect rcc = els[child];
                                rcc.X = x;
                                els[child] = rcc;
                                x += rcc.Width + gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = finalSize.Height - padding.Bottom;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = els.ElementAt(i).Key;
                            //    Rect rcc = els[child];
                            //    y -= rcc.Height;
                            //    rcc.Y = y;
                            //    els[child] = rcc;
                            //    y -= this.Gap;
                            //}
                            for (int i = els.Count - 1; i >= 0; i--)
                            {
                                var child = (FrameworkElement)InternalChildren[i];
                                Rect rcc = els[child];
                                y -= rcc.Height;
                                rcc.Y = y;
                                els[child] = rcc;
                                y -= gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = padding.Top;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = els.ElementAt(i).Key;
                            //    Rect rcc = els[child];
                            //    rcc.Y = y;
                            //    els[child] = rcc;
                            //    y = y + rcc.Height + this.Gap;
                            //}
                            for (int i = els.Count - 1; i >= 0; i--)
                            {
                                var child = (FrameworkElement)InternalChildren[i];
                                Rect rcc = els[child];
                                rcc.Y = y;
                                els[child] = rcc;
                                y = y + rcc.Height + gap;
                            }
                            break;
                    }

                    break;
                case JustifyContent.Center:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            //var totalw = els.Values.Sum(x => x.Width);
                            var totalgap = this.TotalGap();
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    Rect rcc = els[oo];
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x += rcc.Width + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x += rcc.Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            //totalw = els.Values.Sum(x => x.Width);
                            totalgap = this.TotalGap();
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            x = finalSize.Width - x;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    Rect rcc = els[oo];
                            //    x -= rcc.Width;
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x -= this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                x -= rcc.Width;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.Column:
                            //var totalh = els.Values.Sum(x => x.Height);
                            totalgap = this.TotalGap();
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap) / 2);
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    Rect rcc = els[oo];
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y += rcc.Height + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y += rcc.Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            //totalh = els.Values.Sum(x => x.Height);
                            totalgap = this.TotalGap();
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap) / 2);
                            y = finalSize.Height - y;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    Rect rcc = els[oo];
                            //    y -= rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y -= this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                Rect rcc = oo.Value;
                                y -= rcc.Height;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y -= gap;
                            }
                            break;
                    }
                    break;              
                case JustifyContent.SpaceAround:
                    switch (this.FlexDirection)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.TotalGap();
                            //var totalw = els.Values.Sum(x => x.Width);
                            var remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                            var iw = Math.Max(0, remainingSpace / (InternalChildren.Count * 2));

                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    x += iw;
                            //    var rcc = els[oo];
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x += iw + rcc.Width + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                x += iw;
                                var rcc = oo.Value;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x += iw + rcc.Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            totalgapw = this.TotalGap();
                            //totalw = els.Values.Sum(x => x.Width);
                            remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                            iw = Math.Max(0, remainingSpace / (InternalChildren.Count * 2));
                            x = finalSize.Width - padding.Right;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    x -= iw;
                            //    var rcc = els[oo];
                            //    x -= rcc.Width;
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x = x - iw - this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                x -= iw;
                                var rcc = oo.Value;
                                x -= rcc.Width;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            //var totalh = els.Values.Sum(x => x.Height);
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                            ih = ih < 0 ? 0 : ih /= (InternalChildren.Count * 2);
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    y += ih;
                            //    var rcc = els[oo];
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y += ih + rcc.Height + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                y += ih;
                                var rcc = oo.Value;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y += ih + rcc.Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            totalgaph = this.TotalGap();
                            //totalh = els.Values.Sum(x => x.Height);
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                            ih = ih < 0 ? 0 : ih /= (InternalChildren.Count * 2);
                            y = finalSize.Height - padding.Bottom;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    var rcc = els[oo];
                            //    y = y - ih - rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y -= ih -this.Gap;
                            //}
                            foreach (var oo in els)
                            {
                                var rcc = oo.Value;
                                y = y - ih - rcc.Height;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y -= ih - gap;
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
                                //var totalw = els.Values.Sum(x => x.Width);
                                var iw = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                                iw = Math.Max(0, iw / (InternalChildren.Count + 1));
                                x = x + iw;
                                //foreach (var oo in els.Select(x => x.Key))
                                //{
                                //    var rcc = els[oo];
                                //    rcc.X = x;
                                //    els[oo] = rcc;
                                //    x += iw + rcc.Width + this.Gap;
                                //}

                                foreach (var oo in els)
                                {
                                    var rcc = oo.Value;
                                    rcc.X = x;
                                    els[oo.Key] = rcc;
                                    x += iw + rcc.Width + gap;
                                }
                                break;
                            case FlexDirection.RowReverse:
                                totalgapw = this.TotalGap();
                                //totalw = els.Values.Sum(x => x.Width);
                                iw = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                                iw = Math.Max(0, iw / (InternalChildren.Count + 1));
                                x = finalSize.Width - padding.Right;
                                x = x - iw;
                                //foreach (var oo in els.Select(x => x.Key))
                                //{
                                //    var rcc = els[oo];
                                //    x -= rcc.Width;
                                //    rcc.X = x;
                                //    els[oo] = rcc;
                                //    x -= iw  - this.Gap;
                                //}

                                foreach (var oo in els)
                                {
                                    var rcc = oo.Value;
                                    x -= rcc.Width;
                                    rcc.X = x;
                                    els[oo.Key] = rcc;
                                    x -= iw - gap;
                                }
                                break;
                            case FlexDirection.Column:
                                var totalgaph = this.TotalGap();
                                //var totalh = els.Values.Sum(x => x.Height);
                                var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                                if (ih < 0)
                                {
                                    ih = 0;
                                }
                                else
                                {
                                    ih /= (InternalChildren.Count + 1);
                                }
                                y = y + ih;
                                //foreach (var oo in els.Select(x => x.Key))
                                //{
                                //    var rcc = els[oo];
                                //    rcc.Y = y;
                                //    els[oo] = rcc;
                                //    y += ih + rcc.Height + this.Gap;
                                //}

                                foreach (var oo in els)
                                {
                                    var rcc = oo.Value;
                                    rcc.Y = y;
                                    els[oo.Key] = rcc;
                                    y += ih + rcc.Height + gap;
                                }
                                break;
                            case FlexDirection.ColumnReverse:
                                totalgaph = this.TotalGap();
                                //totalh = els.Values.Sum(x => x.Height);
                                ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                                if (ih < 0)
                                {
                                    ih = 0;
                                }
                                else
                                {
                                    ih /= (InternalChildren.Count + 1);
                                }
                                y = finalSize.Height - padding.Bottom;
                                y = y - ih;
                                //foreach (var oo in els.Select(x => x.Key))
                                //{
                                //    var rcc = els[oo];
                                //    y = y - rcc.Height;
                                //    rcc.Y = y;
                                //    els[oo] = rcc;
                                //    y = y - ih - this.Gap;
                                //}

                                foreach (var oo in els)
                                {
                                    var rcc = oo.Value;
                                    y = y - rcc.Height;
                                    rcc.Y = y;
                                    els[oo.Key] = rcc;
                                    y = y - ih - gap;
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
                            var iw = (finalSize.Width - padding.Left - padding.Right - totalgapw);
                            //iw = iw - els.Sum(x => x.Key.DesiredSize.Width);
                            iw = iw - totaldsw;
                            var childcount = Math.Max(1, InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = padding.Left;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    var rcc = els[oo];
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x = x + iw + rcc.Width + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                var rcc = oo.Value;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x = x + iw + rcc.Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            totalgapw = this.TotalGap();
                            iw = (finalSize.Width - padding.Left - padding.Right - totalgapw);
                            //iw = iw - els.Sum(x => x.Key.DesiredSize.Width);
                            iw = iw - totaldsw;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = finalSize.Width - padding.Right;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    var rcc = els[oo];
                            //    x = x - rcc.Width;
                            //    rcc.X = x;
                            //    els[oo] = rcc;
                            //    x = x -iw- this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                var rcc = oo.Value;
                                x = x - rcc.Width;
                                rcc.X = x;
                                els[oo.Key] = rcc;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph);
                            //ih = ih - els.Sum(x => x.Key.DesiredSize.Height);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = padding.Top;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    var rcc = els[oo];
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y = y + ih + rcc.Height + this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                var rcc = oo.Value;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y = y + ih + rcc.Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            totalgaph = this.TotalGap();
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph);
                            //ih = ih - els.Sum(x => x.Key.DesiredSize.Height);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = finalSize.Height - padding.Bottom;
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    var rcc = els[oo];
                            //    y = y - rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo] = rcc;
                            //    y = y - ih - this.Gap;
                            //}

                            foreach (var oo in els)
                            {
                                var rcc = oo.Value;
                                y = y - rcc.Height;
                                rcc.Y = y;
                                els[oo.Key] = rcc;
                                y = y - ih - gap;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}