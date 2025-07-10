using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public readonly static DependencyProperty AlignItemsProperty = DependencyProperty.Register("AlignItems", typeof(AlignItems), typeof(FlexPanel), new FrameworkPropertyMetadata(AlignItems.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));
        [Category("FlexPanel")]
        public AlignItems AlignItems
        {
            set => this.SetValue(AlignItemsProperty, value);
            get => (AlignItems)GetValue(AlignItemsProperty);
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            var height = this.AlignItems switch
            {
                AlignItems.Top => 0,
                AlignItems.Bottom => availableSize.Height,
                AlignItems.Center => availableSize.Height / 2,
                AlignItems.Stretch => availableSize.Height,
                AlignItems.BaeseLine => 0, // TODO: implement
                _ => availableSize.Height,
            };

            foreach (UIElement child in InternalChildren)
            {
                child?.Measure(availableSize);
            }
            return availableSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            //switch (this.JustifyContent)
            //{
            //    case JustifyContent.SpaceAround:
            //    case JustifyContent.SpaceBetween:
            //        {
            //            var item_w = ActualWidth / InternalChildren.Count;
            //            double x = 0;
            //            foreach (UIElement child in InternalChildren)
            //            {
            //                dc.DrawLine(new Pen(Brushes.Red, 1), new Point(x, 0), new Point(x, ActualHeight));
            //                x += item_w;
            //            }

            //        }
            //        break;
            //}
        }

        protected override Size ArrangeOverride(Size finalSize)
        {

            Dictionary<UIElement, Rect> rc = [];
            foreach (UIElement child in InternalChildren)
            {
                rc.Add(child, new());
            }
            CalacJustifyContent(rc, finalSize);
            CalacAlignItems(rc, finalSize);
            foreach (var oo in rc)
            {
                oo.Key.Arrange(oo.Value);
                System.Diagnostics.Trace.WriteLine(oo.Value);
            }



            return finalSize;
        }

        void CalacAlignItems(Dictionary<UIElement, Rect> els, Size finalSize)
        {
            double y = 0;
            
            switch (this.AlignItems)
            {
                case AlignItems.Top:
                    y = 0;
                    break;
                case AlignItems.Bottom:
                    y = finalSize.Height;
                    break;
                case AlignItems.Center:
                    y = finalSize.Height / 2;
                    break;
                case AlignItems.Stretch:
                    y = 0; // stretch is handled in ArrangeOverride
                    break;
                case AlignItems.BaeseLine:
                    y = 0; // TODO: implement
                    break;
            }
            
            foreach (var oo in els)
            {
                var rc = oo.Value;
                rc.Y = y;
                switch (this.AlignItems)
                {
                    case AlignItems.Top:
                        {
                            rc.Y = 0;
                            rc.Height = oo.Key.DesiredSize.Height;
                        }
                        break;
                    case AlignItems.Bottom:
                        {
                            rc.Y = finalSize.Height - oo.Key.DesiredSize.Height;
                            rc.Height = oo.Key.DesiredSize.Height;
                        }
                        break;
                    case AlignItems.Center:
                        {
                            rc.Y = (finalSize.Height - oo.Key.DesiredSize.Height) / 2;
                            rc.Height = oo.Key.DesiredSize.Height;
                        }
                        break;
                    case AlignItems.Stretch:
                        {
                            rc.Y = 0;
                            rc.Height = finalSize.Height;
                        }
                        break;
                }
                
                els[oo.Key] = rc;
            }
        }

        void CalacJustifyContent(Dictionary<UIElement, Rect> els, Size finalSize)
        {
            var item_w = finalSize.Width / InternalChildren.Count;
            double x = 0;
            switch (this.JustifyContent)
            {
                case JustifyContent.Left:
                    {
                        //foreach (UIElement child in InternalChildren)
                        //{
                        //    item_w = child.DesiredSize.Width;
                          //    {
                        //        X = x,
                        //        Y = 0,
                        //        Height = finalSize.Height,
                        //        Width = item_w,
                        //    });
                        //    x = x + item_w;
                        //}
                        foreach (var oo in els)
                        {
                            item_w = oo.Key.DesiredSize.Width;
                            els[oo.Key] = new Rect()
                            {
                                X = x,
                                Y = 0,
                                Height = finalSize.Height,
                                Width = item_w,
                            };
                            x = x + item_w;
                        }

                    }
                    break;
                case JustifyContent.Right:
                    {
                        //x = finalSize.Width;
                        //for (int i = InternalChildren.Count - 1; i >= 0; i--)
                        //{
                        //    UIElement child = InternalChildren[i];
                        //    item_w = child.DesiredSize.Width;
                        //    x = x - item_w;
                        //    child.Arrange(new Rect()
                        //    {
                        //        X = x,
                        //        Y = 0,
                        //        Height = finalSize.Height,
                        //        Width = item_w,
                        //    });
                        //}

                        x = finalSize.Width;
                        for (int i = els.Count - 1; i >= 0; i--)
                        {
                            UIElement child = els.ElementAt(i).Key;
                            item_w = child.DesiredSize.Width;
                            x = x - item_w;

                            els[child] = new Rect()
                            {
                                X = x,
                                Y = 0,
                                Height = finalSize.Height,
                                Width = item_w,
                            };
                        }
                    }
                    break;
                case JustifyContent.Center:
                    {
                        var totalw = 0.0;
                        foreach (UIElement oo in this.InternalChildren)
                        {
                            totalw += oo.DesiredSize.Width;
                        }
                        x = (finalSize.Width - totalw) / 2;
                        foreach (UIElement child in InternalChildren)
                        {
                            item_w = child.DesiredSize.Width;
                            child.Arrange(new Rect()
                            {
                                X = x,
                                Y = 0,
                                Height = finalSize.Height,
                                Width = item_w,
                            });
                            x += item_w;
                        }
                    }
                    break;
                case JustifyContent.SpaceAround:
                    {
                        var iw = finalSize.Width / InternalChildren.Count;
                        foreach (UIElement child in InternalChildren)
                        {
                            item_w = child.DesiredSize.Width;
                            if (iw < child.DesiredSize.Width)
                            {
                                iw = child.DesiredSize.Width;
                            }
                            child.Arrange(new Rect()
                            {
                                X = x + (iw - child.DesiredSize.Width) / 2,
                                Y = 0,
                                Height = finalSize.Height,
                                Width = item_w,
                            });
                            x += iw;
                        }
                    }
                    break;
                case JustifyContent.SpaceBetween:
                    {
                        var iw = finalSize.Width / InternalChildren.Count;
                        for (int i = 0; i < InternalChildren.Count; i++)
                        {
                            var child = InternalChildren[i];

                            item_w = child.DesiredSize.Width;

                            if (item_w > iw)
                            {
                                item_w = iw;
                            }
                            Rect rc;
                            if (i == 0)
                            {
                                rc = new Rect()
                                {
                                    X = 0,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                            }
                            else if (i == InternalChildren.Count - 1)
                            {
                                rc = new Rect()
                                {
                                    X = finalSize.Width - item_w,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                            }
                            else
                            {
                                rc = new Rect()
                                {
                                    X = x + (iw - item_w) / 2,
                                    Y = 0,
                                    Height = finalSize.Height,
                                    Width = item_w,
                                };
                            }
                            child.Arrange(rc);
                            System.Diagnostics.Debug.WriteLine($"[{i}] {rc}");
                            x += iw;
                        }
                    }
                    break;

            }
        }
    }
}
