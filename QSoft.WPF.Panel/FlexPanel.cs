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
                //rc.Add(addfe, new Rect());
                //grows.Add(addfe, 0);
            }
            if (visualRemoved is FrameworkElement removefe)
            {
                MaxWidthDesciptor.RemoveValueChanged(removefe, OnMaxWidthChanged);
                MaxHeightDesciptor.RemoveValueChanged(removefe, OnMaxHeightChanged);
                //rc.Remove(removefe);
                //grows.Remove(removefe);
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

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
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


            return desiredSize;
        }

        double[] grows = [];
        //readonly Dictionary<FrameworkElement, double> grows = [];
        //readonly Dictionary<FrameworkElement, Rect> rc = [];
        Rect[] rcs = [];
        protected override Size ArrangeOverride(Size finalSize)
        {
            var childrenCount = this.InternalChildren.Count;
            if (childrenCount == 0)
                return base.ArrangeOverride(finalSize);

            var padding = this.Padding;
            var gap = this.Gap;
            var direction = this.FlexDirection;

            var shrinkmode = direction switch
            { 
                FlexDirection.Row => this.DesiredSize.Width > finalSize.Width,
                FlexDirection.Column => this.DesiredSize.Height > finalSize.Height,
                _=>false
            };


            if (grows.Length < childrenCount)
            {
                Array.Resize(ref grows, childrenCount * 2);
            }
            if(rcs.Length < childrenCount)
            {
                Array.Resize(ref rcs, childrenCount *2);
            }

            for (int i = 0; i < childrenCount; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                Rect rcc = new(0, 0, child.DesiredSize.Width, child.DesiredSize.Height);
                var shrink = GetShrink(child);
                var basis = GetBasis(child);
                if (basis != 0)
                {
                    switch(direction)
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
                            rcs[i].Width = basis;
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
                            rcs[i].Height = basis;
                            break;
                    }
                }
                rcs[i] = rcc;
                //rc[child] = rcc;
            }
            
            

            bool isclacgrow = false;
            for (int i = 0; i < childrenCount; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                var grow = GetGrow(child);
                if(!isclacgrow && grow > 0)
                {
                    isclacgrow = true;
                }
                grows[i] = Math.Max(grow, 0);
                //grows[child] = Math.Max(grow, 0);
            }

            
            if (isclacgrow)
            {
                this.CalacGrow(rcs, finalSize, grows, direction, padding, gap);
            }
            else
            {
                CalacJustifyContent(rcs, finalSize, this.JustifyContent, direction, padding, gap);
                
            }
            
            CalacAlignItems(rcs, finalSize, direction);
            //for (int i = 0; i < InternalChildren.Count; i++)
            //{
            //    var child = (FrameworkElement)InternalChildren[i];
            //    rc[child] = rcs[i];
            //}
            for (int i = 0; i < childrenCount; i++)
            {
                InternalChildren[i].Arrange(rcs[i]);
            }

            return finalSize;
        }
        void CalacGrow(/*Dictionary<FrameworkElement, Rect> els, */Rect[] rcs, Size finalSize, double[] grows, FlexDirection direction,Thickness padding , double gap)
        {
            var item_w = 0.0;
            var item_h = 0.0;
            double x = padding.Left;
            double y = padding.Top;
            var sum = 0.0;
            var zerogrow_w = 0.0;
            var zerogrow_h = 0.0;
            for(int i=0; i< InternalChildren.Count; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                var grow = grows[i];
                sum = sum + grow;
                if (grow == 0)
                {
                    zerogrow_w = zerogrow_w + child.DesiredSize.Width;
                    zerogrow_h = zerogrow_h + child.DesiredSize.Height;
                }
            }

            var totalgap = TotalGap();
            switch (direction)
            {
                case FlexDirection.Row:
                    var iw = Math.Max(finalSize.Width - zerogrow_w - totalgap - padding.Left - padding.Right, 0);
                    iw = iw / sum;
                    //foreach (var oo in els)
                    //{
                    //    item_w = grows[oo.Key] * iw;
                    //    if (item_w <= 0)
                    //    {
                    //        item_w = oo.Key.DesiredSize.Width;
                    //    }
                    //    var rcc = oo.Value;
                    //    rcc.Width = item_w;
                    //    rcc.X = x;
                    //    els[oo.Key] = rcc;
                    //    x += item_w + gap;
                    //}
                    for(int i=0; i< InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_w = grows[i] * iw;
                        if (item_w <= 0)
                        {
                            item_w = child.DesiredSize.Width;
                        }
                        //var rcc = els[child];
                        rcs[i].Width = item_w;
                        rcs[i].X = x;
                        //els[child] = rcc;
                        x += item_w + gap;
                    }
                    break;
                case FlexDirection.Column:
                    var ih = Math.Max(finalSize.Height - zerogrow_h - totalgap - padding.Top - padding.Bottom, 0);
                    ih /= sum;
                    //foreach (var oo in els)
                    //{
                    //    item_h = grows[oo.Key] * ih;
                    //    if (item_h <= 0)
                    //    {
                    //        item_h = oo.Key.DesiredSize.Height;
                    //    }
                    //    var rcc = oo.Value;
                    //    rcc.Height = item_h;
                    //    rcc.Y = y;
                    //    els[oo.Key] = rcc;
                    //    y += item_h + gap;
                    //}
                    for(int i = 0; i < InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_h = grows[i] * ih;
                        if (item_h <= 0)
                        {
                            item_h = child.DesiredSize.Height;
                        }
                        //var rcc = els[child];
                        rcs[i].Height = item_h;
                        rcs[i].Y = y;
                        //els[child] = rcc;
                        y += item_h + gap;
                    }
                    break;
            }
        }

        void CalacAlignItems(Rect[] rcs, Size finalSize, FlexDirection direction)
        {
            for(int i=0; i < InternalChildren.Count; i++) 
            {
                var child = (FrameworkElement)InternalChildren[i];
                
                var alignitem = GetAlignSelf(child) switch
                {
                    AlignSelf.Stretch => AlignItems.Stretch,
                    AlignSelf.Center => AlignItems.Center,
                    AlignSelf.Start => AlignItems.Start,
                    AlignSelf.End => AlignItems.End,
                    _ => this.AlignItems
                };
                switch (alignitem)
                {
                    case AlignItems.Start:
                        {
                            switch (direction)
                            {
                                case FlexDirection.Row:
                                case FlexDirection.RowReverse:
                                    rcs[i].Y = this.Padding.Top;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = this.Padding.Left;
                                    rcs[i].Width = child.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.End:
                        {
                            switch (direction)
                            {
                                case FlexDirection.Row:
                                case FlexDirection.RowReverse:
                                    rcs[i].Y = finalSize.Height - child.DesiredSize.Height - this.Padding.Bottom;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = finalSize.Width - child.DesiredSize.Width - this.Padding.Right;
                                    rcs[i].Width = child.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.Center:
                        {
                            switch (direction)
                            {
                                case FlexDirection.Row:
                                case FlexDirection.RowReverse:
                                    rcs[i].Y = (finalSize.Height - child.DesiredSize.Height) / 2;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = (finalSize.Width - child.DesiredSize.Width) / 2;
                                    rcs[i].Width = child.DesiredSize.Width;
                                    break;
                            }
                        }
                        break;
                    case AlignItems.Stretch:
                        {
                            switch (direction)
                            {
                                case FlexDirection.Row:
                                case FlexDirection.RowReverse:
                                    rcs[i].Y = this.Padding.Top;
                                    rcs[i].Height = Math.Max(finalSize.Height - this.Padding.Top - this.Padding.Bottom, 0);
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = this.Padding.Left;
                                    rcs[i].Width = Math.Max(finalSize.Width - this.Padding.Left - this.Padding.Right, 0);
                                    break;
                            }

                        }
                        break;
                }
                //els[child] = rc;
            }
        }

        double TotalGap()
            => this.InternalChildren.Count > 1 
            ? this.Gap * (InternalChildren.Count - 1)
            : 0;

        void CalacJustifyContent(/*Dictionary<FrameworkElement, Rect> els,*/ Rect[] rcs, Size finalSize, JustifyContent justify, FlexDirection direction, Thickness padding, double gap)
        {
            double x = padding.Left;
            double y = padding.Top;
            var totalw = 0.0;
            var totalh = 0.0;
            var totaldsw = 0.0;
            var totaldsh = 0.0;
            //foreach (var oo in els)
            //{
            //    totalw = totalw + oo.Value.Width;
            //    totalh = totalh + oo.Value.Height;
            //    totaldsw = totaldsw + oo.Key.DesiredSize.Width;
            //    totaldsh = totaldsh + oo.Key.DesiredSize.Height;
            //}
            for(int i=0; i< InternalChildren.Count; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                totalw = totalw + rcs[i].Width;
                totalh = totalh + rcs[i].Height;
                totaldsw = totaldsw + child.DesiredSize.Width;
                totaldsh = totaldsh + child.DesiredSize.Height;
            }
            switch (justify)
            {
                case JustifyContent.Start:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            x = padding.Left;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x = x + rcc.Width + gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x = x + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = finalSize.Width - padding.Right;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    x -= rcc.Width;
                            //    rcc.X = x;
                            //    x -= gap;
                            //    els[oo.Key] = rcc;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = padding.Top;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    rcc.Y = y;
                            //    y = y + rcc.Height + gap;
                            //    els[oo.Key] = rcc;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y = y + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = finalSize.Height - padding.Bottom;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    y -= rcc.Height;
                            //    rcc.Y = y;
                            //    y += -gap;
                            //    els[oo.Key] = rcc;
                            //}
                            for (int i = 0; i < InternalChildren.Count; i++)
                            {
                                y -= rcs[i].Height;
                                rcs[i].Y = y;
                                y += -gap;
                            }
                            break;
                    }
                    break;
                case JustifyContent.End:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            x = finalSize.Width - padding.Right;
                            //for (int i = InternalChildren.Count-1; i >=0; i--)
                            //{
                            //    var child = (FrameworkElement)InternalChildren[i];
                            //    Rect rcc = els[child];
                            //    x = x - rcc.Width;
                            //    rcc.X = x;
                            //    els[child] = rcc;
                            //    x -= gap;
                            //}
                            for (int i = InternalChildren.Count - 1; i >= 0; i--)
                            {
                                x = x - rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = padding.Left;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = (FrameworkElement)InternalChildren[i];
                            //    Rect rcc = els[child];
                            //    rcc.X = x;
                            //    els[child] = rcc;
                            //    x += rcc.Width + gap;
                            //}
                            for(int i= InternalChildren.Count - 1; i >= 0; i--)
                            {
                                rcs[i].X = x;
                                x += rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = finalSize.Height - padding.Bottom;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = (FrameworkElement)InternalChildren[i];
                            //    Rect rcc = els[child];
                            //    y -= rcc.Height;
                            //    rcc.Y = y;
                            //    els[child] = rcc;
                            //    y -= gap;
                            //}
                            for(int i = InternalChildren.Count - 1; i >= 0; i--)
                            {
                                y -= rcs[i].Height;
                                rcs[i].Y = y;
                                y -= gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = padding.Top;
                            //for (int i = els.Count - 1; i >= 0; i--)
                            //{
                            //    var child = (FrameworkElement)InternalChildren[i];
                            //    Rect rcc = els[child];
                            //    rcc.Y = y;
                            //    els[child] = rcc;
                            //    y = y + rcc.Height + gap;
                            //}
                            for(int i= InternalChildren.Count - 1; i >= 0; i--)
                            {
                                rcs[i].Y = y;
                                y = y + rcs[i].Height + gap;
                            }
                            break;
                    }

                    break;
                case JustifyContent.Center:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            var totalgap = this.TotalGap();
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x += rcc.Width + gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x += rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            totalgap = this.TotalGap();
                            x = (finalSize.Width - totalw - totalgap) / 2;
                            x = finalSize.Width - x;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    x -= rcc.Width;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x -= gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.Column:
                            totalgap = this.TotalGap();
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap) / 2);
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y += rcc.Height + gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y += rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            totalgap = this.TotalGap();
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap) / 2);
                            y = finalSize.Height - y;
                            //foreach (var oo in els)
                            //{
                            //    Rect rcc = oo.Value;
                            //    y -= rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y -= gap;
                            //}
                            for (int i = 0; i < InternalChildren.Count; i++)
                            {
                                y -= rcs[i].Height;
                                rcs[i].Y = y;
                                y -= gap;
                            }
                            break;
                    }
                    break;              
                case JustifyContent.SpaceAround:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.TotalGap();
                            var remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                            var iw = Math.Max(0, remainingSpace / (InternalChildren.Count * 2));
                            //foreach (var oo in els)
                            //{
                            //    x += iw;
                            //    var rcc = oo.Value;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x += iw + rcc.Width + gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                x += iw;
                                rcs[i].X = x;
                                x += iw + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            totalgapw = this.TotalGap();
                            remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                            iw = Math.Max(0, remainingSpace / (InternalChildren.Count * 2));
                            x = finalSize.Width - padding.Right;
                            //foreach (var oo in els)
                            //{
                            //    x -= iw;
                            //    var rcc = oo.Value;
                            //    x -= rcc.Width;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x = x - iw - gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                x -= iw;
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                            ih = ih < 0 ? 0 : ih /= (InternalChildren.Count * 2);
                            //foreach (var oo in els)
                            //{
                            //    y += ih;
                            //    var rcc = oo.Value;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y += ih + rcc.Height + gap;
                            //}
                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                y += ih;
                                rcs[i].Y = y;
                                y += ih + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            totalgaph = this.TotalGap();
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph - totalh);
                            ih = ih < 0 ? 0 : ih /= (InternalChildren.Count * 2);
                            y = finalSize.Height - padding.Bottom;
                            //foreach (var oo in els)
                            //{
                            //    var rcc = oo.Value;
                            //    y = y - ih - rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y -= ih - gap;
                            //}

                            for (int i = 0; i < InternalChildren.Count; i++)
                            {
                                y = y - ih - rcs[i].Height;
                                rcs[i].Y = y;
                                y -= ih - gap;
                            }
                            break;
                    }
                    break;
                case JustifyContent.SpaceEvenly:
                    {
                        switch(direction)
                        {
                            case FlexDirection.Row:
                                var totalgapw = this.TotalGap();
                                var iw = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                                iw = Math.Max(0, iw / (InternalChildren.Count + 1));
                                x = x + iw;
                                //foreach (var oo in els)
                                //{
                                //    var rcc = oo.Value;
                                //    rcc.X = x;
                                //    els[oo.Key] = rcc;
                                //    x += iw + rcc.Width + gap;
                                //}

                                for(int i=0; i< InternalChildren.Count; i++)
                                {
                                    rcs[i].X = x;
                                    x += iw + rcs[i].Width + gap;
                                }
                                break;
                            case FlexDirection.RowReverse:
                                totalgapw = this.TotalGap();
                                iw = (finalSize.Width - padding.Left - padding.Right - totalgapw - totalw);
                                iw = Math.Max(0, iw / (InternalChildren.Count + 1));
                                x = finalSize.Width - padding.Right;
                                x = x - iw;
                                //foreach (var oo in els)
                                //{
                                //    var rcc = oo.Value;
                                //    x -= rcc.Width;
                                //    rcc.X = x;
                                //    els[oo.Key] = rcc;
                                //    x -= iw - gap;
                                //}

                                for(int i=0; i< InternalChildren.Count; i++)
                                {
                                    x -= rcs[i].Width;
                                    rcs[i].X = x;
                                    x -= iw - gap;
                                }
                                break;
                            case FlexDirection.Column:
                                var totalgaph = this.TotalGap();
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
                                //foreach (var oo in els)
                                //{
                                //    var rcc = oo.Value;
                                //    rcc.Y = y;
                                //    els[oo.Key] = rcc;
                                //    y += ih + rcc.Height + gap;
                                //}

                                for(int i=0; i< InternalChildren.Count; i++)
                                {
                                    rcs[i].Y = y;
                                    y += ih + rcs[i].Height + gap;
                                }
                                break;
                            case FlexDirection.ColumnReverse:
                                totalgaph = this.TotalGap();
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
                                //foreach (var oo in els)
                                //{
                                //    var rcc = oo.Value;
                                //    y = y - rcc.Height;
                                //    rcc.Y = y;
                                //    els[oo.Key] = rcc;
                                //    y = y - ih - gap;
                                //}

                                for (int i = 0; i < InternalChildren.Count; i++)
                                {
                                    y = y - rcs[i].Height;
                                    rcs[i].Y = y;
                                    y = y - ih - gap;
                                }
                                break;
                        }
                    }
                    break;
                case JustifyContent.SpaceBetween:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            var totalgapw = this.TotalGap();
                            var iw = (finalSize.Width - padding.Left - padding.Right - totalgapw);
                            iw = iw - totaldsw;
                            var childcount = Math.Max(1, InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = padding.Left;
                            //foreach (var oo in els)
                            //{
                            //    var rcc = oo.Value;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x = x + iw + rcc.Width + gap;
                            //}

                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x = x + iw + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            totalgapw = this.TotalGap();
                            iw = (finalSize.Width - padding.Left - padding.Right - totalgapw);
                            iw = iw - totaldsw;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = finalSize.Width - padding.Right;
                            //foreach (var oo in els)
                            //{
                            //    var rcc = oo.Value;
                            //    x = x - rcc.Width;
                            //    rcc.X = x;
                            //    els[oo.Key] = rcc;
                            //    x = x - iw - gap;
                            //}

                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                x = x - rcs[i].Width;
                                rcs[i].X = x;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = padding.Top;
                            //foreach (var oo in els)
                            //{
                            //    var rcc = oo.Value;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y = y + ih + rcc.Height + gap;
                            //}

                            for(int i=0; i< InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y = y + ih + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            totalgaph = this.TotalGap();
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgaph);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = finalSize.Height - padding.Bottom;
                            //foreach (var oo in els)
                            //{
                            //    var rcc = oo.Value;
                            //    y = y - rcc.Height;
                            //    rcc.Y = y;
                            //    els[oo.Key] = rcc;
                            //    y = y - ih - gap;
                            //}

                            for (int i = 0; i < InternalChildren.Count; i++)
                            {
                                y = y - rcs[i].Height;
                                rcs[i].Y = y;
                                y = y - ih - gap;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}