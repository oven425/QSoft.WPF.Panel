﻿using System;
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

        public static readonly DependencyProperty BasisProperty = DependencyProperty.RegisterAttached("Basis", typeof(double), typeof(FlexPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static double GetBasis(DependencyObject obj) => (double)obj.GetValue(BasisProperty);
        public static void SetBasis(DependencyObject obj, double value) => obj.SetValue(BasisProperty, value);


        protected override Size MeasureOverride(Size availableSize)
        {
            // 如果沒有子元素，直接返回基礎實現或0
            if (InternalChildren.Count == 0)
            {
                return new Size(0, 0);
            }

            // 初始化這個 Panel 期望的尺寸
            var desiredSize = new Size(0, 0);
            // 複製一份可用的空間，因為我們會在迴圈中修改它
            var remainingSize = availableSize;

            bool isRow = this.FlexDirection == FlexDirection.Row;

            // --- 調整可用空間，先減去 Padding 和 Border ---
            // 這樣傳遞給子元素的才是真正可用的空間
            remainingSize.Width = Math.Max(0.0, remainingSize.Width - (this.Padding.Left + this.Padding.Right));
            remainingSize.Height = Math.Max(0.0, remainingSize.Height - (this.Padding.Top + this.Padding.Bottom));

            // --- 單次迴圈完成所有測量與計算 ---
            foreach (UIElement child in InternalChildren)
            {
                if (child == null) continue;

                // 核心修正：用剩餘的空間去測量子元素
                child.Measure(remainingSize);

                var childDesiredSize = child.DesiredSize;

                if (isRow)
                {
                    // Row 方向：寬度累加，高度取最大值
                    desiredSize.Width += childDesiredSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, childDesiredSize.Height);

                    // 更新剩餘可用寬度
                    remainingSize.Width -= childDesiredSize.Width;
                }
                else // Column 方向
                {
                    // Column 方向：寬度取最大值，高度累加
                    desiredSize.Width = Math.Max(desiredSize.Width, childDesiredSize.Width);
                    desiredSize.Height += childDesiredSize.Height;

                    // 更新剩餘可用高度
                    remainingSize.Height -= childDesiredSize.Height;
                }
            }

            // --- 加上 Gap 的空間 ---
            var totalGap = TotalGap(); // 假設 TotalGap() 的邏輯是正確的
            if (isRow)
            {
                desiredSize.Width += totalGap;
            }
            else
            {
                desiredSize.Height += totalGap;
            }

            // --- 最後加上 Padding 和 Border 的空間 ---
            desiredSize.Width += this.Padding.Left + this.Padding.Right;
            desiredSize.Height += this.Padding.Top + this.Padding.Bottom;

            // --- 約束最終尺寸不超過父容器提供的可用空間 ---
            // 這一部分邏輯保持不變，是正確的
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
                if(basis != 0)
                {
                    if(this.FlexDirection == FlexDirection.Row)
                    {
                        rcc.Width = basis;
                    }
                    else
                    {
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
                            //foreach (var oo in els.Select(x => x.Key))
                            //{
                            //    item_w = oo.DesiredSize.Width;
                            //    els[oo] = new Rect()
                            //    {
                            //        X = x,
                            //        Y = y,
                            //        Height = finalSize.Height,
                            //        Width = item_w,
                            //    };
                            //    x = x + item_w + this.Gap;
                            //}
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                var rcc = els[oo];
                                rcc.X = x;
                                els[oo] = rcc;
                                x = x + rcc.Width + this.Gap;
                            }
                            //for(int i=0; i<els.Count; i++)
                            //{
                            //    var kv = els.ElementAt(i);
                            //    var rcc = kv.Value;
                            //    rcc.X = x;
                            //    els[kv.Key] = rcc;
                            //    x = x + rcc.Width + this.Gap;
                            //}
                            using (var enumerator = els.GetEnumerator())
                            {
                                double currentX = 0;

                                while (enumerator.MoveNext())
                                {
                                    var kvp = enumerator.Current;
                                    Rect rcc = els[kvp.Key];
                                    rcc.X = currentX;
                                    currentX = currentX + rcc.Width + this.Gap;
                                    els[kvp.Key] = rcc;
                                }
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
                            var totalgapw = this.TotalGap();
                            var totalw = els.Keys.Sum(x => x.DesiredSize.Width);
                            var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw - totalw);
                            if (iw < 0)
                            {
                                iw = 0;
                            }
                            else
                            {
                                iw = iw / (InternalChildren.Count*2);
                            }
                            
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                x = x + iw;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = finalSize.Height,
                                    Width = oo.DesiredSize.Width,
                                };
                                x += iw+oo.DesiredSize.Width + this.Gap;
                            }
                            break;
                        case FlexDirection.Column:
                            var totalgaph = this.TotalGap();
                            var totalh = els.Keys.Sum(x => x.DesiredSize.Height);
                            var ih = (finalSize.Height - this.Padding.Top - this.Padding.Bottom - totalgaph - totalh);
                            if(ih<0)
                            {
                                ih = 0;
                            }
                            else
                            {
                                ih = ih / (InternalChildren.Count*2);
                            }
                            foreach (var oo in els.Select(x => x.Key))
                            {
                                y= y + ih;
                                els[oo] = new Rect()
                                {
                                    X = x,
                                    Y = y,
                                    Height = oo.DesiredSize.Height,
                                    Width = finalSize.Width,
                                };
                                y += ih+oo.DesiredSize.Height + this.Gap;
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
                                var totalw = els.Keys.Sum(x => x.DesiredSize.Width);
                                var iw = (finalSize.Width - this.Padding.Left - this.Padding.Right - totalgapw - totalw);
                                if (iw < 0)
                                {
                                    iw = 0;
                                }
                                else
                                {
                                    iw = iw / (InternalChildren.Count +1);
                                }
                                x = x + iw;
                                foreach (var oo in els.Select(x => x.Key))
                                {
                                    els[oo] = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = finalSize.Height,
                                        Width = oo.DesiredSize.Width,
                                    };
                                    x += iw+oo.DesiredSize.Width + this.Gap;
                                }
                                break;
                            case FlexDirection.Column:
                                var totalgaph = this.TotalGap();
                                var totalh = els.Keys.Sum(x => x.DesiredSize.Height);
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
                                    els[oo] = new Rect()
                                    {
                                        X = x,
                                        Y = y,
                                        Height = oo.DesiredSize.Height,
                                        Width = finalSize.Width,
                                    };
                                    y += ih + oo.DesiredSize.Height + this.Gap;
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
                                y += ih + this.Gap;
                            }
                            break;

                    }

                    break;

            }
        }
    }
}