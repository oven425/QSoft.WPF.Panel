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
        public readonly static DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(FlexPanel), new PropertyMetadata(new Thickness()));
        [Category("FlexPanel")]
        public Thickness BorderThickness
        {
            set => this.SetValue(BorderThicknessProperty, value);
            get => (Thickness)GetValue(BorderThicknessProperty);
        }

        public readonly static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FlexPanel), new PropertyMetadata(new CornerRadius()));
        [Category("FlexPanel")]
        public CornerRadius CornerRadius
        {
            set => this.SetValue(CornerRadiusProperty, value);
            get => (CornerRadius)GetValue(CornerRadiusProperty);
        }

        public readonly static DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(FlexPanel), new PropertyMetadata(null));
        [Category("FlexPanel")]
        public Brush BorderBrush
        {
            set => this.SetValue(BorderBrushProperty, value);
            get => (Brush)GetValue(BorderBrushProperty);
        }

        public readonly static DependencyProperty JustifyContentProperty = DependencyProperty.Register("JustifyContent", typeof(JustifyContent), typeof(FlexPanel), new FrameworkPropertyMetadata(JustifyContent.SpaceBetween, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public JustifyContent JustifyContent
        {
            set => this.SetValue(JustifyContentProperty, value);
            get => (JustifyContent)GetValue(JustifyContentProperty);
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

        public static readonly DependencyProperty AlignSelfProperty = DependencyProperty.RegisterAttached("AlignSelf", typeof(AlignSelf), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignSelf.Auto, FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure |
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static AlignSelf GetAlignSelf(DependencyObject obj) => (AlignSelf)obj.GetValue(AlignSelfProperty);
        public static void SetAlignSelf(DependencyObject obj, AlignSelf value) => obj.SetValue(AlignSelfProperty, value);


        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);
            // 取得目前的邊框厚度、圓角和渲染尺寸
            Thickness thickness = this.BorderThickness;
            CornerRadius cornerRadius = this.CornerRadius;

            // RenderSize 是控制項最終被分配到的繪圖區域大小
            Rect renderRect = new Rect(new Point(0, 0), this.RenderSize);

            // 1. 繪製背景
            if (this.Background != null)
            {
                // 使用 DrawRoundedRectangle 繪製背景，Pen 設為 null 表示不繪製邊框
                //dc.DrawRoundedRectangle(this.Background, null, renderRect, cornerRadius);
            }

            // 2. 繪製邊框
            // 檢查是否有邊框畫刷，且邊框厚度不為零
            //if (this.BorderBrush != null && !thickness.Equals(new Thickness()))
            {
                // 使用最穩健的方法：建立一個代表邊框的 Geometry 並繪製它

                // 定義外部矩形的邊界
                Rect outerRect = renderRect;

                // 根據邊框厚度計算內部矩形的邊界
                // 內部矩形向內縮排
                Rect innerRect = new Rect(
                    outerRect.Left + thickness.Left,
                    outerRect.Top + thickness.Top,
                    Math.Max(0.0, outerRect.Width - thickness.Left - thickness.Right),
                    Math.Max(0.0, outerRect.Height - thickness.Top - thickness.Bottom)
                );

                // 計算內部圓角半徑
                // 內部圓角需要根據外部圓角和邊框厚度進行縮減
                CornerRadius innerCornerRadius = new CornerRadius(
                    Math.Max(0.0, cornerRadius.TopLeft - Math.Max(thickness.Left, thickness.Top) / 2.0),
                    Math.Max(0.0, cornerRadius.TopRight - Math.Max(thickness.Right, thickness.Top) / 2.0),
                    Math.Max(0.0, cornerRadius.BottomRight - Math.Max(thickness.Right, thickness.Bottom) / 2.0),
                    Math.Max(0.0, cornerRadius.BottomLeft - Math.Max(thickness.Left, thickness.Bottom) / 2.0)
                );

                // 建立外部和內部幾何圖形
                // RoundedRectangleGeometry 是描述圓角矩形的 Geometry
                var outerGeometry = CreateRoundedRectangleGeometry(outerRect, cornerRadius);
                var innerGeometry = CreateRoundedRectangleGeometry(innerRect, innerCornerRadius);

                // 使用 CombinedGeometry 將兩個幾何圖形組合起來
                // GeometryCombineMode.Exclude 會從第一個圖形中減去第二個圖形
                // 這樣就得到了中空的邊框形狀
                var borderGeometry = new CombinedGeometry(
                    GeometryCombineMode.Exclude,
                    outerGeometry,
                    innerGeometry
                );

                // 最後，使用 DrawGeometry 繪製這個計算出來的邊框形狀
                // Pen 設為 null，因為我們是直接填充(Fill)這個 Geometry
                dc.DrawGeometry(this.BorderBrush, null, borderGeometry);
                dc.DrawGeometry(this.Background, null, innerGeometry);
            }
        }

        PathGeometry CreateRoundedRectangleGeometry(Rect rect, CornerRadius cornerRadius)
        {
            // 建立 PathGeometry 和 PathFigure
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            geometry.Figures.Add(figure);

            // 校正 CornerRadius，確保半徑不會大於矩形尺寸的一半
            double left = rect.Left;
            double top = rect.Top;
            double right = rect.Right;
            double bottom = rect.Bottom;

            // 左上角 (Top-Left)
            double radiusX_TL = Math.Min(cornerRadius.TopLeft, (right - left) / 2.0);
            double radiusY_TL = Math.Min(cornerRadius.TopLeft, (bottom - top) / 2.0);

            // 右上角 (Top-Right)
            double radiusX_TR = Math.Min(cornerRadius.TopRight, (right - left) / 2.0);
            double radiusY_TR = Math.Min(cornerRadius.TopRight, (bottom - top) / 2.0);

            // 右下角 (Bottom-Right)
            double radiusX_BR = Math.Min(cornerRadius.BottomRight, (right - left) / 2.0);
            double radiusY_BR = Math.Min(cornerRadius.BottomRight, (bottom - top) / 2.0);

            // 左下角 (Bottom-Left)
            double radiusX_BL = Math.Min(cornerRadius.BottomLeft, (right - left) / 2.0);
            double radiusY_BL = Math.Min(cornerRadius.BottomLeft, (bottom - top) / 2.0);

            // 設定 PathFigure 的起始點 (上邊緣，左上圓角之後)
            figure.StartPoint = new Point(left + radiusX_TL, top);

            // 1. 上邊緣 (Top Edge)
            figure.Segments.Add(new LineSegment(new Point(right - radiusX_TR, top), true));

            // 2. 右上角 (Top-Right Corner)
            figure.Segments.Add(new ArcSegment(
                new Point(right, top + radiusY_TR),
                new Size(radiusX_TR, radiusY_TR),
                90, // 旋轉角度
                false, // isLargeArc
                SweepDirection.Clockwise,
                true));

            // 3. 右邊緣 (Right Edge)
            figure.Segments.Add(new LineSegment(new Point(right, bottom - radiusY_BR), true));

            // 4. 右下角 (Bottom-Right Corner)
            figure.Segments.Add(new ArcSegment(
                new Point(right - radiusX_BR, bottom),
                new Size(radiusX_BR, radiusY_BR),
                90,
                false,
                SweepDirection.Clockwise,
                true));

            // 5. 下邊緣 (Bottom Edge)
            figure.Segments.Add(new LineSegment(new Point(left + radiusX_BL, bottom), true));

            // 6. 左下角 (Bottom-Left Corner)
            figure.Segments.Add(new ArcSegment(
                new Point(left, bottom - radiusY_BL),
                new Size(radiusX_BL, radiusY_BL),
                90,
                false,
                SweepDirection.Clockwise,
                true));

            // 7. 左邊緣 (Left Edge)
            figure.Segments.Add(new LineSegment(new Point(left, top + radiusY_TL), true));

            // 8. 左上角 (Top-Left Corner)
            figure.Segments.Add(new ArcSegment(
                figure.StartPoint, // 回到起點
                new Size(radiusX_TL, radiusY_TL),
                90,
                false,
                SweepDirection.Clockwise,
                true));

            // 關閉圖形
            figure.IsClosed = true;

            return geometry;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
                return base.MeasureOverride(availableSize);
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
            sz.Width = sz.Width + this.Padding.Left + this.Padding.Right+this.BorderThickness.Left+this.BorderThickness.Right;
            sz.Height = sz.Height + this.Padding.Top + this.Padding.Bottom+this.BorderThickness.Top+this.BorderThickness.Bottom;
            if (sz.Width > availableSize.Width)
            {
                sz.Width = availableSize.Width;
            }
            if (sz.Height > availableSize.Height)
            {
                sz.Height = availableSize.Height;
            }
            System.Diagnostics.Debug.WriteLine($"{this.Name} MeasureOverride: {sz}");
            return sz;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return base.ArrangeOverride(finalSize);

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
                                    rc.Y = this.Padding.Top+this.BorderThickness.Top;
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
                                    rc.Y = finalSize.Height - oo.Key.DesiredSize.Height - this.Padding.Bottom - this.BorderThickness.Bottom;
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
                                    rc.Height = finalSize.Height - this.Padding.Top - this.Padding.Bottom;
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
            double x = this.Padding.Left+this.BorderThickness.Left;
            double y = this.Padding.Top+this.BorderThickness.Top;

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
                            x = finalSize.Width - this.Padding.Right - this.BorderThickness.Right;
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
                                    Y = y,
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