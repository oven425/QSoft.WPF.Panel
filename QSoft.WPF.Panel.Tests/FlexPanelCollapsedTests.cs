using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace QSoft.WPF.Panel.Tests
{
    [TestClass]
    public sealed class FlexPanelCollapsedTests
    {
        [TestMethod]
        public void RowGap_IgnoresCollapsedChild()
        {
            RunOnSta(() =>
            {
                var panel = CreatePanel(FlexDirection.Row, gap: 10);
                var first = CreateChild(20, 10);
                var collapsed = CreateChild(30, 10, Visibility.Collapsed);
                var third = CreateChild(20, 10);

                panel.Children.Add(first);
                panel.Children.Add(collapsed);
                panel.Children.Add(third);

                Layout(panel, new Size(200, 100), new Size(200, 100));

                AssertRect(first, 0, 0, 20, 10);
                AssertRect(third, 30, 0, 20, 10);
                AssertRect(collapsed, 0, 0, 0, 0);
            });
        }

        [TestMethod]
        public void JustifyContentSpaceBetween_CountsOnlyVisibleChildren()
        {
            RunOnSta(() =>
            {
                var panel = CreatePanel(FlexDirection.Row, justifyContent: JustifyContent.SpaceBetween);
                var first = CreateChild(20, 10);
                var collapsed = CreateChild(20, 10, Visibility.Collapsed);
                var third = CreateChild(20, 10);

                panel.Children.Add(first);
                panel.Children.Add(collapsed);
                panel.Children.Add(third);

                Layout(panel, new Size(100, 40), new Size(100, 40));

                AssertRect(first, 0, 0, 20, 10);
                AssertRect(third, 80, 0, 20, 10);
                AssertRect(collapsed, 0, 0, 0, 0);
            });
        }

        [TestMethod]
        public void Wrap_IgnoresCollapsedChildWhenCollectingLines()
        {
            RunOnSta(() =>
            {
                var panel = CreatePanel(FlexDirection.Row, flexWrap: FlexWrap.Wrap, gap: 10, rowGap: 5, alignItems: AlignItems.Start);
                var first = CreateChild(30, 10);
                var collapsed = CreateChild(30, 10, Visibility.Collapsed);
                var third = CreateChild(30, 10);
                var fourth = CreateChild(30, 10);

                panel.Children.Add(first);
                panel.Children.Add(collapsed);
                panel.Children.Add(third);
                panel.Children.Add(fourth);

                Layout(panel, new Size(75, 100), new Size(75, 100));

                AssertRect(first, 0, 0, 30, 10);
                AssertRect(third, 40, 0, 30, 10);
                AssertRect(fourth, 0, 52.5, 30, 10);
                AssertRect(collapsed, 0, 0, 0, 0);
            });
        }

        [TestMethod]
        public void RowReverse_IgnoresCollapsedChildInReverseFlow()
        {
            RunOnSta(() =>
            {
                var panel = CreatePanel(FlexDirection.RowReverse, gap: 10);
                var first = CreateChild(20, 10);
                var collapsed = CreateChild(20, 10, Visibility.Collapsed);
                var third = CreateChild(20, 10);

                panel.Children.Add(first);
                panel.Children.Add(collapsed);
                panel.Children.Add(third);

                Layout(panel, new Size(100, 40), new Size(100, 40));

                AssertRect(first, 80, 0, 20, 10);
                AssertRect(third, 50, 0, 20, 10);
                AssertRect(collapsed, 0, 0, 0, 0);
            });
        }

        [TestMethod]
        public void AlignContentSpaceBetween_IgnoresCollapsedLineContribution()
        {
            RunOnSta(() =>
            {
                var panel = CreatePanel(FlexDirection.Row, flexWrap: FlexWrap.Wrap, alignContent: AlignContent.SpaceBetween, alignItems: AlignItems.Start);
                var first = CreateChild(30, 10);
                var collapsed = CreateChild(30, 10, Visibility.Collapsed);
                var third = CreateChild(30, 10);
                var fourth = CreateChild(30, 10);

                panel.Children.Add(first);
                panel.Children.Add(collapsed);
                panel.Children.Add(third);
                panel.Children.Add(fourth);

                Layout(panel, new Size(70, 100), new Size(70, 100));

                AssertRect(first, 0, 0, 30, 10);
                AssertRect(third, 30, 0, 30, 10);
                AssertRect(fourth, 0, 90, 30, 10);
                AssertRect(collapsed, 0, 0, 0, 0);
            });
        }

        static FlexPanel CreatePanel(
            FlexDirection direction,
            double gap = 0,
            double rowGap = double.NaN,
            double columnGap = double.NaN,
            FlexWrap flexWrap = FlexWrap.NoWrap,
            JustifyContent justifyContent = JustifyContent.Start,
            AlignItems alignItems = AlignItems.Stretch,
            AlignContent alignContent = AlignContent.Stretch)
        {
            return new FlexPanel
            {
                FlexDirection = direction,
                Gap = gap,
                RowGap = rowGap,
                ColumnGap = columnGap,
                FlexWrap = flexWrap,
                JustifyContent = justifyContent,
                AlignItems = alignItems,
                AlignContent = alignContent
            };
        }

        static Border CreateChild(double width, double height, Visibility visibility = Visibility.Visible)
        {
            return new Border
            {
                Width = width,
                Height = height,
                Visibility = visibility
            };
        }

        static void Layout(FlexPanel panel, Size availableSize, Size finalSize)
        {
            panel.Measure(availableSize);
            panel.Arrange(new Rect(new Point(), finalSize));
            panel.UpdateLayout();
        }

        static void AssertRect(FrameworkElement element, double x, double y, double width, double height)
        {
            var rect = LayoutInformation.GetLayoutSlot(element);
            Assert.AreEqual(x, rect.X, 0.001, $"Unexpected X for {element}.");
            Assert.AreEqual(y, rect.Y, 0.001, $"Unexpected Y for {element}.");
            Assert.AreEqual(width, rect.Width, 0.001, $"Unexpected Width for {element}.");
            Assert.AreEqual(height, rect.Height, 0.001, $"Unexpected Height for {element}.");
        }

        static void RunOnSta(Action action)
        {
            Exception? error = null;
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (error is not null)
            {
                throw new AssertFailedException($"STA test failed: {error}");
            }
        }
    }
}
