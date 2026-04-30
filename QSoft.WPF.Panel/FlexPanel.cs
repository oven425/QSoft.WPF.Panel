using System;
using System.ComponentModel;
using System.Windows;

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
        public readonly static DependencyProperty JustifyContentProperty = DependencyProperty.Register("JustifyContent", typeof(JustifyContent), typeof(FlexPanel), new FrameworkPropertyMetadata(JustifyContent.Start, FrameworkPropertyMetadataOptions.AffectsArrange));
        [Category("FlexPanel")]
        public JustifyContent JustifyContent
        {
            set => this.SetValue(JustifyContentProperty, value);
            get => (JustifyContent)GetValue(JustifyContentProperty);
        }

        public readonly static DependencyProperty AlignItemsProperty = DependencyProperty.Register("AlignItems", typeof(AlignItems), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignItems.Start, FrameworkPropertyMetadataOptions.AffectsArrange));
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

        public static readonly DependencyProperty AlignSelfProperty = DependencyProperty.RegisterAttached("AlignSelf", typeof(AlignSelf), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignSelf.Auto, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static AlignSelf GetAlignSelf(DependencyObject obj) => (AlignSelf)obj.GetValue(AlignSelfProperty);
        public static void SetAlignSelf(DependencyObject obj, AlignSelf value) => obj.SetValue(AlignSelfProperty, value);

        public static readonly DependencyProperty GrowProperty = DependencyProperty.RegisterAttached("Grow", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetGrow(DependencyObject obj) => (double)obj.GetValue(GrowProperty);
        public static void SetGrow(DependencyObject obj, double value) => obj.SetValue(GrowProperty, value);

        public static readonly DependencyProperty BasisProperty = DependencyProperty.RegisterAttached("Basis", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange|FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetBasis(DependencyObject obj) => (double)obj.GetValue(BasisProperty);
        public static void SetBasis(DependencyObject obj, double value) => obj.SetValue(BasisProperty, value);

        public FlexPanel()
        {
            this.Loaded += FlexPanel_Loaded;
            this.Unloaded += OnUnloaded;
        }

        private void FlexPanel_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement child in this.InternalChildren)
            {
                if (child is FrameworkElement fe)
                {
                    MaxWidthDescriptor.RemoveValueChanged(fe, OnMaxWidthChanged);
                    MaxHeightDescriptor.RemoveValueChanged(fe, OnMaxHeightChanged);
                    MaxWidthDescriptor.AddValueChanged(fe, OnMaxWidthChanged);
                    MaxHeightDescriptor.AddValueChanged(fe, OnMaxHeightChanged);
                }
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (UIElement child in this.InternalChildren)
            {
                if (child is FrameworkElement fe)
                {
                    MaxWidthDescriptor.RemoveValueChanged(fe, OnMaxWidthChanged);
                    MaxHeightDescriptor.RemoveValueChanged(fe, OnMaxHeightChanged);
                }
            }
        }
        static readonly DependencyPropertyDescriptor MaxWidthDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxWidthProperty, typeof(FrameworkElement));
        static readonly DependencyPropertyDescriptor MaxHeightDescriptor = DependencyPropertyDescriptor.FromProperty(FrameworkElement.MaxHeightProperty, typeof(FrameworkElement));
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if(visualAdded is FrameworkElement addfe)
            {
                MaxWidthDescriptor.RemoveValueChanged(addfe, OnMaxWidthChanged);
                MaxHeightDescriptor.RemoveValueChanged(addfe, OnMaxHeightChanged);
                MaxWidthDescriptor.AddValueChanged(addfe, OnMaxWidthChanged);
                MaxHeightDescriptor.AddValueChanged(addfe, OnMaxHeightChanged);
            }
            if (visualRemoved is FrameworkElement removefe)
            {
                MaxWidthDescriptor.RemoveValueChanged(removefe, OnMaxWidthChanged);
                MaxHeightDescriptor.RemoveValueChanged(removefe, OnMaxHeightChanged);
            }
        }

        void OnMaxWidthChanged(object? sender, EventArgs e)
        {
            if (this.FlexDirection != FlexDirection.Row && this.FlexDirection != FlexDirection.RowReverse) return;
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
            if (this.FlexDirection != FlexDirection.Column && this.FlexDirection != FlexDirection.ColumnReverse) return;
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

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            var childrenCount = this.InternalChildren.Count;
            if (childrenCount == 0) return new System.Windows.Size(0, 0);

            var totalGap = TotalGap();
            var desiredSize = new System.Windows.Size(0, 0);
            bool isRow = this.FlexDirection switch
            { 
                FlexDirection.Row => true,
                FlexDirection.RowReverse => true,
                _=>false
            };

            for (int i = 0; i < childrenCount; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                var basis = GetBasis(child);
                child.Measure(availableSize);
                var childDesiredSize = child.DesiredSize;

                if (isRow && basis > 0)
                {
#if NET5_0_OR_GREATER
                    basis = Math.Clamp(basis, child.MinWidth, child.MaxWidth);
#else
                    basis = Math.Max(child.MinWidth, Math.Min(basis, child.MaxWidth));
#endif
                    childDesiredSize.Width = basis;
                }
                else if (!isRow && basis > 0)
                {
#if NET5_0_OR_GREATER
                    basis = Math.Clamp(basis, child.MinHeight, child.MaxHeight);
#else
                     basis = Math.Max(child.MinHeight, Math.Min(basis, child.MaxHeight));
#endif
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
        Rect[] rcs = [];
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            var childrenCount = this.InternalChildren.Count;
            if (childrenCount == 0)
                return base.ArrangeOverride(finalSize);

            var padding = this.Padding;
            var gap = this.Gap;
            var totalgap = TotalGap();
            var direction = this.FlexDirection;


            if (grows.Length < childrenCount)
            {
                Array.Resize(ref grows, childrenCount * 2);
            }
            if(rcs.Length < childrenCount)
            {
                Array.Resize(ref rcs, childrenCount *2);
            }
            bool isclacgrow = false;
            for (int i = 0; i < childrenCount; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                var desiredSize = child.DesiredSize;
                rcs[i].X = 0;
                rcs[i].Y = 0;
                rcs[i].Width = desiredSize.Width;
                rcs[i].Height = desiredSize.Height;
                var basis = GetBasis(child);
                if (basis != 0)
                {
                    switch(direction)
                    {
                        case FlexDirection.Row:
                        case FlexDirection.RowReverse:
#if NET5_0_OR_GREATER
                            basis = Math.Clamp(basis, child.MinWidth, child.MaxWidth);
#else
                            basis = Math.Max(child.MinWidth, Math.Min(basis, child.MaxWidth));
#endif
                            rcs[i].Width = basis;
                            break;
                        case FlexDirection.Column:
                        case FlexDirection.ColumnReverse:
#if NET5_0_OR_GREATER
                            basis = Math.Clamp(basis, child.MinHeight, child.MaxHeight);
#else
                            basis = Math.Max(child.MinHeight, Math.Min(basis, child.MaxHeight));
#endif
                            rcs[i].Height = basis;
                            break;
                    }
                }

                var grow = GetGrow(child);
                isclacgrow = isclacgrow || grow > 0;
                grows[i] = Math.Max(grow, 0);
            }

            
            if (isclacgrow)
            {
                this.CalcGrow(rcs, finalSize, grows, direction, padding, gap, totalgap);
            }
            else
            {
                CalcJustifyContent(rcs, finalSize, this.JustifyContent, direction, padding, gap, totalgap);
            }
            
            CalcAlignItems(rcs, finalSize, direction, padding);

            for (int i = 0; i < childrenCount; i++)
            {
                InternalChildren[i].Arrange(rcs[i]);
            }

            return finalSize;
        }
        void CalcGrow(Rect[] rcs, in System.Windows.Size finalSize, double[] grows, FlexDirection direction,in Thickness padding , double gap, double totalgap)
        {
            var item_w = 0.0;
            var item_h = 0.0;
            double x = padding.Left;
            double y = padding.Top;
            var sum = 0.0;
            var zerogrow_w = 0.0;
            var zerogrow_h = 0.0;
            for(int i=0; i< this.InternalChildren.Count; i++)
            {
                var child = (FrameworkElement)InternalChildren[i];
                var grow = grows[i];
                sum += grow;
                if (grow == 0)
                {
                    zerogrow_w += child.DesiredSize.Width;
                    zerogrow_h += child.DesiredSize.Height;
                }
            }

            switch (direction)
            {
                case FlexDirection.Row:
                    var iw = Math.Max(finalSize.Width - zerogrow_w - totalgap - padding.Left - padding.Right, 0);
                    iw = iw / sum;
                    for(int i=0; i< this.InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_w = grows[i] * iw;
                        if (item_w <= 0)
                        {
                            item_w = child.DesiredSize.Width;
                        }
                        rcs[i].Width = item_w;
                        rcs[i].X = x;
                        x += item_w + gap;
                    }
                    break;
                case FlexDirection.RowReverse:
                    iw = Math.Max(finalSize.Width - zerogrow_w - totalgap - padding.Left - padding.Right, 0);
                    iw = iw / sum;
                    x = finalSize.Width - padding.Right;
                    for (int i = 0; i < this.InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_w = grows[i] * iw;
                        if (item_w <= 0)
                        {
                            item_w = child.DesiredSize.Width;
                        }
                        x -= item_w;
                        rcs[i].X = x;
                        rcs[i].Width = item_w;
                        x -= gap;
                    }
                    break;
                case FlexDirection.Column:
                    var ih = Math.Max(finalSize.Height - zerogrow_h - totalgap - padding.Top - padding.Bottom, 0);
                    ih /= sum;
                    for(int i = 0; i < this.InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_h = grows[i] * ih;
                        if (item_h <= 0)
                        {
                            item_h = child.DesiredSize.Height;
                        }
                        rcs[i].Height = item_h;
                        rcs[i].Y = y;
                        y += item_h + gap;
                    }
                    break;
                case FlexDirection.ColumnReverse:
                    ih = Math.Max(finalSize.Height - zerogrow_h - totalgap - padding.Top - padding.Bottom, 0);
                    ih /= sum;
                    y = finalSize.Height - padding.Bottom;
                    for (int i = 0; i < this.InternalChildren.Count; i++)
                    {
                        var child = (FrameworkElement)InternalChildren[i];
                        item_h = grows[i] * ih;
                        if (item_h <= 0)
                        {
                            item_h = child.DesiredSize.Height;
                        }
                        y -= item_h;
                        rcs[i].Height = item_h;
                        rcs[i].Y = y;
                        y -= gap;
                    }

                    break;
            }
        }

        void CalcAlignItems(Rect[] rcs, in System.Windows.Size finalSize, FlexDirection direction, in Thickness padding)
        {
            for(int i=0; i < this.InternalChildren.Count; i++) 
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
                                    rcs[i].Y = padding.Top;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = padding.Left;
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
                                    rcs[i].Y = finalSize.Height - child.DesiredSize.Height - padding.Bottom;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = finalSize.Width - child.DesiredSize.Width - padding.Right;
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
                                    rcs[i].Y = (finalSize.Height - child.DesiredSize.Height - padding.Top - padding.Bottom) / 2 + padding.Top;
                                    rcs[i].Height = child.DesiredSize.Height;
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = (finalSize.Width - child.DesiredSize.Width - padding.Left - padding.Right) / 2 + padding.Left;
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
                                    rcs[i].Y = padding.Top;
                                    rcs[i].Height = Math.Max(finalSize.Height - padding.Top - padding.Bottom, 0);
                                    break;
                                case FlexDirection.Column:
                                case FlexDirection.ColumnReverse:
                                    rcs[i].X = padding.Left;
                                    rcs[i].Width = Math.Max(finalSize.Width - padding.Left - padding.Right, 0);
                                    break;
                            }

                        }
                        break;
                }
            }
        }

        double TotalGap()
            => this.InternalChildren.Count > 1 
            ? this.Gap * (this.InternalChildren.Count - 1)
            : 0;

        void CalcJustifyContent(Rect[] rcs, in System.Windows.Size finalSize, JustifyContent justify, FlexDirection direction, in Thickness padding, double gap, double totalgap)
        {
            double x = padding.Left;
            double y = padding.Top;
            var totalw = 0.0;
            var totalh = 0.0;
            var totaldsw = 0.0;
            var totaldsh = 0.0;
            for(int i=0; i< this.InternalChildren.Count; i++)
            {
                var child = this.InternalChildren[i];
                totalw += rcs[i].Width;
                totalh += rcs[i].Height;
                totaldsw += child.DesiredSize.Width;
                totaldsh += child.DesiredSize.Height;
            }
            switch (justify)
            {
                case JustifyContent.Start:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            x = padding.Left;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x = x + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = finalSize.Width - padding.Right;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = padding.Top;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y = y + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = finalSize.Height - padding.Bottom;
                            for (int i = 0; i < this.InternalChildren.Count; i++)
                            {
                                y -= rcs[i].Height;
                                rcs[i].Y = y;
                                y -= gap;
                            }
                            break;
                    }
                    break;
                case JustifyContent.End:
                    switch (direction)
                    {
                        case FlexDirection.Row:
                            x = finalSize.Width - padding.Right;
                            for (int i = this.InternalChildren.Count - 1; i >= 0; i--)
                            {
                                x = x - rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = padding.Left;
                            for(int i= this.InternalChildren.Count - 1; i >= 0; i--)
                            {
                                rcs[i].X = x;
                                x += rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = finalSize.Height - padding.Bottom;
                            for(int i = this.InternalChildren.Count - 1; i >= 0; i--)
                            {
                                y -= rcs[i].Height;
                                rcs[i].Y = y;
                                y -= gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = padding.Top;
                            for(int i= this.InternalChildren.Count - 1; i >= 0; i--)
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
                            x = (finalSize.Width - totalw - totalgap - padding.Left - padding.Right) / 2;
                            x = x+padding.Left;
                            for (int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x += rcs[i].Width + gap;    
                            }
                            break;
                        case FlexDirection.RowReverse:
                            x = (finalSize.Width - totalw - totalgap - padding.Left - padding.Right) / 2;
                            x = finalSize.Width - padding.Right - x;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x -= gap;
                            }
                            break;
                        case FlexDirection.Column:
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap - padding.Top - padding.Bottom) / 2);
                            y = y+padding.Top;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y += rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            y = Math.Max(0, (finalSize.Height - totalh - totalgap - padding.Top - padding.Bottom) / 2);
                            y = finalSize.Height - padding.Bottom - y;
                            for (int i = 0; i < this.InternalChildren.Count; i++)
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
                            var remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgap - totalw);
                            var iw = Math.Max(0, remainingSpace / (this.InternalChildren.Count * 2));
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                x += iw;
                                rcs[i].X = x;
                                x += iw + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            remainingSpace = (finalSize.Width - padding.Left - padding.Right - totalgap - totalw);
                            iw = Math.Max(0, remainingSpace / (this.InternalChildren.Count * 2));
                            x = finalSize.Width - padding.Right;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                x -= iw;
                                x -= rcs[i].Width;
                                rcs[i].X = x;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap - totalh);
                            ih = ih < 0 ? 0 : ih /= (this.InternalChildren.Count * 2);
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                y += ih;
                                rcs[i].Y = y;
                                y += ih + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap - totalh);
                            ih = ih < 0 ? 0 : ih /= (this.InternalChildren.Count * 2);
                            y = finalSize.Height - padding.Bottom;
                            for (int i = 0; i < this.InternalChildren.Count; i++)
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
                                var iw = (finalSize.Width - padding.Left - padding.Right - totalgap - totalw);
                                iw = Math.Max(0, iw / (this.InternalChildren.Count + 1));
                                x = x + iw;
                                for(int i=0; i< this.InternalChildren.Count; i++)
                                {
                                    rcs[i].X = x;
                                    x += iw + rcs[i].Width + gap;
                                }
                                break;
                            case FlexDirection.RowReverse:
                                iw = (finalSize.Width - padding.Left - padding.Right - totalgap - totalw);
                                iw = Math.Max(0, iw / (this.InternalChildren.Count + 1));
                                x = finalSize.Width - padding.Right;
                                x = x - iw;
                                for(int i=0; i< this.InternalChildren.Count; i++)
                                {
                                    x -= rcs[i].Width;
                                    rcs[i].X = x;
                                    x = x - iw- gap;
                                }
                                break;
                            case FlexDirection.Column:
                                var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap - totalh);
                                if (ih < 0)
                                {
                                    ih = 0;
                                }
                                else
                                {
                                    ih /= (this.InternalChildren.Count + 1);
                                }
                                y = y + ih;
                                for(int i=0; i< this.InternalChildren.Count; i++)
                                {
                                    rcs[i].Y = y;
                                    y += ih + rcs[i].Height + gap;
                                }
                                break;
                            case FlexDirection.ColumnReverse:
                                ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap - totalh);
                                if (ih < 0)
                                {
                                    ih = 0;
                                }
                                else
                                {
                                    ih /= (this.InternalChildren.Count + 1);
                                }
                                y = finalSize.Height - padding.Bottom;
                                y = y - ih;
                                for (int i = 0; i < this.InternalChildren.Count; i++)
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
                            var iw = (finalSize.Width - padding.Left - padding.Right - totalgap);
                            iw = iw - totaldsw;
                            var childcount = Math.Max(1, this.InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = padding.Left;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].X = x;
                                x = x + iw + rcs[i].Width + gap;
                            }
                            break;
                        case FlexDirection.RowReverse:
                            iw = (finalSize.Width - padding.Left - padding.Right - totalgap);
                            iw = iw - totaldsw;
                            childcount = Math.Max(1, this.InternalChildren.Count - 1);
                            iw = Math.Max(0, iw / childcount);
                            x = finalSize.Width - padding.Right;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                x = x - rcs[i].Width;
                                rcs[i].X = x;
                                x = x - iw - gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, this.InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = padding.Top;
                            for(int i=0; i< this.InternalChildren.Count; i++)
                            {
                                rcs[i].Y = y;
                                y = y + ih + rcs[i].Height + gap;
                            }
                            break;
                        case FlexDirection.ColumnReverse:
                            ih = (finalSize.Height - padding.Top - padding.Bottom - totalgap);
                            ih = ih - totaldsh;
                            childcount = Math.Max(1, this.InternalChildren.Count - 1);
                            ih = ih < 0 ? 0 : ih / childcount;
                            y = finalSize.Height - padding.Bottom;
                            for (int i = 0; i < this.InternalChildren.Count; i++)
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