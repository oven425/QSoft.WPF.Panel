using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QSoft.WPF.Behaviors
{
    public class RoutedEventIsHandledBehavior: Behavior<TextBox>
    {
        readonly public static DependencyProperty IsHandledProperty = DependencyProperty.Register("IsHandled", typeof(bool), typeof(RoutedEventIsHandledBehavior));
        public bool IsHandled { set => SetValue(IsHandledProperty, value); get => (bool)GetValue(IsHandledProperty); }
        private MethodInfo? eventHandlerMethodInfo;
        protected override void OnAttached()
        {
            Type targetType = this.AssociatedObject.GetType();
            var eventInfo = targetType.GetEvent("PreviewKeyDown");
            this.eventHandlerMethodInfo = typeof(RoutedEventIsHandledBehavior).GetMethod("OnEventImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            eventInfo?.AddEventHandler(this.AssociatedObject, Delegate.CreateDelegate(eventInfo.EventHandlerType, this, this.eventHandlerMethodInfo));
            

            //this.AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            //this.AssociatedObject.AddHandler
            base.OnAttached();
        }

        private void OnEventImpl(object sender, RoutedEventArgs eventArgs)
        {
            eventArgs.Handled = IsHandled;
            //var rr = eventArgs as RoutedEventArgs;
            //if(rr is not null)
            //{
            //    rr.Handled = IsHandled;
            //}
        }

        private void AssociatedObject_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = IsHandled;
            //throw new NotImplementedException();
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;

            base.OnDetaching();
        }
    }
}
