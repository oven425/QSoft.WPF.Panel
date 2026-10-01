using System;
using System.ComponentModel;
using System.Windows;

//https://w3c.hexschool.com/flexbox/4a029043
namespace QSoft.WPF.Panel
{
    //public enum FlexWrap
    //{
    //    NoWrap,
    //    Wrap,
    //    //WrapReverse
    //}
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
        //public readonly static DependencyProperty FlexWrapProperty = DependencyProperty.Register("FlexWrap", typeof(FlexWrap), typeof(FlexPanel), new FrameworkPropertyMetadata(FlexWrap.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure));
        //[Category("FlexPanel")]
        //public FlexWrap FlexWrap
        //{
        //    set => this.SetValue(FlexWrapProperty, value);
        //    get => (FlexWrap)GetValue(FlexWrapProperty);
        //}

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

        public static readonly DependencyProperty ShrinkProperty = DependencyProperty.RegisterAttached("Shrink", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetShrink(DependencyObject obj) => (double)obj.GetValue(ShrinkProperty);
        public static void SetShrink(DependencyObject obj, double value) => obj.SetValue(ShrinkProperty, value);

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

        // Flex data of one child, gathered by MeasureOverride and reused by ArrangeOverride.
        struct FlexItem
        {
            public bool HasBasis;
            public double Base;     // flex base size (main axis, margin included): Basis, or the desired size when Basis is not set
            public double Min;      // main-axis size limits, see GetSizeLimits
            public double Max;
            public double Chrome;   // margin, border and padding on the main axis, see GetChrome
            public double Grow;
            public double Shrink;
            public double Size;     // resolved main-axis size
            public bool Frozen;
            public double Violation;

            // CSS weights Shrink by the size of the content, so larger items shrink more than smaller ones.
            public double ShrinkWeight => Shrink * Math.Max(Base - Chrome, 0);
        }

        FlexItem[] flexItems = [];
        int measuredCount;

        static bool IsRow(FlexDirection direction)
            => direction == FlexDirection.Row || direction == FlexDirection.RowReverse;

        static Size MakeSize(bool isRow, double main, double cross)
            => isRow ? new Size(main, cross) : new Size(cross, main);

        static double Deflate(double size, double padding)
            => double.IsInfinity(size) ? size : Math.Max(size - padding, 0);

        static double Clamp(double value, double min, double max)
            => Math.Max(min, Math.Min(value, max));

        static double ToFactor(double value)
            => value > 0 && !double.IsInfinity(value) ? value : 0;

        // Same rule FrameworkElement applies to itself: an explicit Width/Height pins the size (within Min/Max).
        // The limits are for the child's slot, so they include its margin like DesiredSize does.
        static void GetSizeLimits(UIElement child, bool horizontal, out double min, out double max)
        {
            min = 0;
            max = double.PositiveInfinity;
            if (child is not FrameworkElement fe) return;

            var size = horizontal ? fe.Width : fe.Height;
            min = horizontal ? fe.MinWidth : fe.MinHeight;
            max = horizontal ? fe.MaxWidth : fe.MaxHeight;
            max = Math.Max(Math.Min(double.IsNaN(size) ? double.PositiveInfinity : size, max), min);
            min = Math.Max(Math.Min(max, double.IsNaN(size) ? 0 : size), min);

            var margin = Sum(fe.Margin, horizontal);
            min = Math.Max(min + margin, 0);
            max = Math.Max(max + margin, min);
        }

        static double Sum(Thickness thickness, bool horizontal)
            => horizontal ? thickness.Left + thickness.Right : thickness.Top + thickness.Bottom;

        static double GetMargin(UIElement child, bool horizontal)
            => child is FrameworkElement fe ? Sum(fe.Margin, horizontal) : 0;

        // The part of the child's size on one axis that is not content: margin, and border and padding of the
        // elements that have them. A box can't get smaller than that, and it doesn't take part in shrinking.
        static double GetChrome(UIElement child, bool horizontal)
        {
            if (child is not FrameworkElement fe) return 0;

            var chrome = Sum(fe.Margin, horizontal);
            switch (fe)
            {
                case System.Windows.Controls.Border border:
                    chrome += Sum(border.BorderThickness, horizontal) + Sum(border.Padding, horizontal);
                    break;
                case System.Windows.Controls.Control control:
                    chrome += Sum(control.BorderThickness, horizontal) + Sum(control.Padding, horizontal);
                    break;
                case System.Windows.Controls.TextBlock textBlock:
                    chrome += Sum(textBlock.Padding, horizontal);
                    break;
            }
            return Math.Max(chrome, 0);
        }

        static void ReadFlexProperties(UIElement child, bool isRow, ref FlexItem item)
        {
            GetSizeLimits(child, isRow, out var min, out var max);
            item.Chrome = GetChrome(child, isRow);
            item.Max = max;
            item.Min = Math.Min(Math.Max(min, item.Chrome), max);
            item.Grow = ToFactor(GetGrow(child));
            item.Shrink = ToFactor(GetShrink(child));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var children = this.InternalChildren;
            var count = children.Count;
            var padding = this.Padding;
            var isRow = IsRow(this.FlexDirection);
            var padMain = isRow ? padding.Left + padding.Right : padding.Top + padding.Bottom;
            var padCross = isRow ? padding.Top + padding.Bottom : padding.Left + padding.Right;
            var availMain = isRow ? availableSize.Width : availableSize.Height;
            var availCross = isRow ? availableSize.Height : availableSize.Width;
            var innerMain = Deflate(availMain, padMain);
            var innerCross = Deflate(availCross, padCross);
            var childConstraint = MakeSize(isRow, innerMain, innerCross);

            if (flexItems.Length < count)
            {
                Array.Resize(ref flexItems, count * 2);
            }
            measuredCount = count;

            var gapTotal = TotalGap();
            var sumHypothetical = 0.0;
            for (int i = 0; i < count; i++)
            {
                var child = children[i];
                ref var item = ref flexItems[i];
                ReadFlexProperties(child, isRow, ref item);

                // Like Width (and CSS flex-basis), Basis is the size of the element without its margin.
                var basis = GetBasis(child);
                var margin = GetMargin(child, isRow);
                if (double.IsPositiveInfinity(basis) && !double.IsInfinity(item.Max))
                {
                    basis = item.Max - margin;
                }
                item.HasBasis = basis > 0 && !double.IsInfinity(basis);
                if (item.HasBasis)
                {
                    item.Base = basis + margin;
                }
                else
                {
                    child.Measure(childConstraint);
                    item.Base = isRow ? child.DesiredSize.Width : child.DesiredSize.Height;
                }
                sumHypothetical += Clamp(item.Base, item.Min, item.Max);
            }

            // Only shrinking is settled here, because a shrunk child has to be measured at its final size.
            // Growing depends on the size the parent finally arranges the panel with, see ArrangeOverride.
            if (sumHypothetical + gapTotal > innerMain)
            {
                ResolveFlexibleLengths(count, innerMain, gapTotal);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    flexItems[i].Size = Clamp(flexItems[i].Base, flexItems[i].Min, flexItems[i].Max);
                }
            }

            var sumSize = 0.0;
            var maxCross = 0.0;
            for (int i = 0; i < count; i++)
            {
                var child = children[i];
                ref var item = ref flexItems[i];
                // FrameworkElement arranges a child at least as large as its unclipped desired size, so a child that
                // gets less room than it asked for must be measured again at that size to report the smaller size.
                if (item.HasBasis || item.Size < item.Base)
                {
                    child.Measure(MakeSize(isRow, item.Size, innerCross));
                }
                var desired = child.DesiredSize;
                sumSize += item.Size;
                maxCross = Math.Max(maxCross, isRow ? desired.Height : desired.Width);
            }

            var desiredMain = Math.Min(sumSize + gapTotal + padMain, availMain);
            var desiredCross = Math.Min(maxCross + padCross, availCross);
            return MakeSize(isRow, desiredMain, desiredCross);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = this.InternalChildren;
            var count = children.Count;
            if (count == 0) return finalSize;
            if (count != measuredCount)
            {
                InvalidateMeasure();
                return finalSize;
            }

            var padding = this.Padding;
            var direction = this.FlexDirection;
            var isRow = IsRow(direction);
            var isReverse = direction == FlexDirection.RowReverse || direction == FlexDirection.ColumnReverse;
            var padStart = isRow ? padding.Left : padding.Top;
            var padEnd = isRow ? padding.Right : padding.Bottom;
            var crossPadStart = isRow ? padding.Top : padding.Left;
            var crossPadEnd = isRow ? padding.Bottom : padding.Right;
            var innerMain = Math.Max((isRow ? finalSize.Width : finalSize.Height) - padStart - padEnd, 0);
            var innerCross = Math.Max((isRow ? finalSize.Height : finalSize.Width) - crossPadStart - crossPadEnd, 0);
            var gap = this.Gap;
            var gapTotal = TotalGap();
            var alignItems = this.AlignItems;

            for (int i = 0; i < count; i++)
            {
                ReadFlexProperties(children[i], isRow, ref flexItems[i]);
            }
            ResolveFlexibleLengths(count, innerMain, gapTotal);

            var freeSpace = innerMain - gapTotal;
            for (int i = 0; i < count; i++)
            {
                freeSpace -= flexItems[i].Size;
            }
            GetJustifyOffsets(freeSpace, count, isReverse, out var offset, out var spacing);

            // Distance from the content box's start edge on the main axis. The start edge is the left/top side,
            // or the right/bottom side for the reversed directions.
            var position = offset;
            for (int i = 0; i < count; i++)
            {
                var child = children[i];
                var main = flexItems[i].Size;
                var mainPos = isReverse ? padStart + innerMain - position - main : padStart + position;

                var align = GetAlignSelf(child) switch
                {
                    AlignSelf.Start => AlignItems.Start,
                    AlignSelf.End => AlignItems.End,
                    AlignSelf.Center => AlignItems.Center,
                    AlignSelf.Stretch => AlignItems.Stretch,
                    _ => alignItems
                };
                double crossSize;
                if (align == AlignItems.Stretch)
                {
                    GetSizeLimits(child, !isRow, out var minCross, out var maxCross);
                    crossSize = Clamp(innerCross, minCross, maxCross);
                }
                else
                {
                    crossSize = isRow ? child.DesiredSize.Height : child.DesiredSize.Width;
                }
                var crossOffset = align switch
                {
                    AlignItems.End => innerCross - crossSize,
                    AlignItems.Center => (innerCross - crossSize) / 2,
                    _ => 0
                };
                var crossPos = crossPadStart + crossOffset;

                child.Arrange(isRow
                    ? new Rect(mainPos, crossPos, main, crossSize)
                    : new Rect(crossPos, mainPos, crossSize, main));
                position += main + gap + spacing;
            }

            return finalSize;
        }

        // CSS Flexbox "Resolving Flexible Lengths" for a single line: shares the free space of the main axis
        // between the items (Grow when there is room left, Shrink when the items don't fit) while honouring
        // their min/max sizes. The resulting main sizes are stored in FlexItem.Size.
        void ResolveFlexibleLengths(int count, double innerMain, double gapTotal)
        {
            var sumHypothetical = 0.0;
            for (int i = 0; i < count; i++)
            {
                sumHypothetical += Clamp(flexItems[i].Base, flexItems[i].Min, flexItems[i].Max);
            }
            var useGrow = sumHypothetical + gapTotal < innerMain;

            // Items that can't flex in the current direction are frozen at their hypothetical size.
            var initialFreeSpace = innerMain - gapTotal;
            for (int i = 0; i < count; i++)
            {
                ref var item = ref flexItems[i];
                var hypothetical = Clamp(item.Base, item.Min, item.Max);
                var factor = useGrow ? item.Grow : item.Shrink;
                item.Frozen = factor == 0 || (useGrow ? item.Base > hypothetical : item.Base < hypothetical);
                item.Size = item.Frozen ? hypothetical : item.Base;
                initialFreeSpace -= item.Size;
            }

            while (true)
            {
                var remainingFreeSpace = innerMain - gapTotal;
                var sumFactors = 0.0;
                var sumScaledShrink = 0.0;
                var hasUnfrozen = false;
                for (int i = 0; i < count; i++)
                {
                    ref var item = ref flexItems[i];
                    if (item.Frozen)
                    {
                        remainingFreeSpace -= item.Size;
                    }
                    else
                    {
                        hasUnfrozen = true;
                        remainingFreeSpace -= item.Base;
                        sumFactors += useGrow ? item.Grow : item.Shrink;
                        sumScaledShrink += item.ShrinkWeight;
                    }
                }
                if (!hasUnfrozen) break;

                if (sumFactors < 1)
                {
                    var scaled = initialFreeSpace * sumFactors;
                    if (Math.Abs(scaled) < Math.Abs(remainingFreeSpace))
                    {
                        remainingFreeSpace = scaled;
                    }
                }

                // Larger items shrink more: the share is weighted by Shrink * content size, not by Shrink alone.
                var totalViolation = 0.0;
                for (int i = 0; i < count; i++)
                {
                    ref var item = ref flexItems[i];
                    if (item.Frozen) continue;

                    var size = item.Base;
                    if (useGrow && remainingFreeSpace > 0)
                    {
                        size += remainingFreeSpace * item.Grow / sumFactors;
                    }
                    else if (!useGrow && remainingFreeSpace < 0 && sumScaledShrink > 0)
                    {
                        size += remainingFreeSpace * item.ShrinkWeight / sumScaledShrink;
                    }
                    item.Size = Clamp(size, item.Min, item.Max);
                    item.Violation = item.Size - size;
                    totalViolation += item.Violation;
                }

                // Items that hit their min/max are fixed at that size and the rest is distributed again.
                for (int i = 0; i < count; i++)
                {
                    ref var item = ref flexItems[i];
                    if (!item.Frozen
                        && (totalViolation == 0 || (totalViolation > 0 ? item.Violation > 0 : item.Violation < 0)))
                    {
                        item.Frozen = true;
                    }
                }
            }
        }

        // Where the first item starts (measured from the main-start edge of the content box) and the extra
        // space between two items. Like CSS, SpaceBetween falls back to start when nothing is left over, while
        // SpaceAround/SpaceEvenly fall back to "safe center": the content is pinned to the left/top edge, which
        // is the main-end side of the reversed directions, and overflows on the other side.
        void GetJustifyOffsets(double freeSpace, int count, bool isReverse, out double offset, out double spacing)
        {
            offset = 0;
            spacing = 0;
            switch (this.JustifyContent)
            {
                case JustifyContent.End:
                    offset = freeSpace;
                    break;
                case JustifyContent.Center:
                    offset = freeSpace / 2;
                    break;
                case JustifyContent.SpaceBetween:
                    if (freeSpace > 0 && count > 1)
                    {
                        spacing = freeSpace / (count - 1);
                    }
                    break;
                case JustifyContent.SpaceAround:
                    if (freeSpace > 0)
                    {
                        spacing = freeSpace / count;
                        offset = spacing / 2;
                    }
                    else if (isReverse)
                    {
                        offset = freeSpace;
                    }
                    break;
                case JustifyContent.SpaceEvenly:
                    if (freeSpace > 0)
                    {
                        spacing = freeSpace / (count + 1);
                        offset = spacing;
                    }
                    else if (isReverse)
                    {
                        offset = freeSpace;
                    }
                    break;
            }
        }

        double TotalGap()
            => this.InternalChildren.Count > 1
            ? this.Gap * (this.InternalChildren.Count - 1)
            : 0;
    }
}