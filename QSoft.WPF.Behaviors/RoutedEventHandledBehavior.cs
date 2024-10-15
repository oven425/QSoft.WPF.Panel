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
    public class RoutedEventIsHandledBehavior: Behavior<UIElement>
    {
        readonly public static DependencyProperty IsHandledProperty = DependencyProperty.Register("IsHandled", typeof(bool), typeof(RoutedEventIsHandledBehavior));
        readonly public static DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(RoutedEventIsHandledBehavior));
        public bool IsHandled { set => SetValue(IsHandledProperty, value); get => (bool)GetValue(IsHandledProperty); }
        public string EventName { set => SetValue(EventNameProperty, value); get => (string)GetValue(EventNameProperty); }
        EventInfo? m_EventInfo;
        Delegate ? m_Delegate;
        protected override void OnAttached()
        {
            Type targetType = this.AssociatedObject.GetType();
            m_EventInfo = targetType.GetEvent(EventName);
            if (m_EventInfo is null) return;
            if (m_EventInfo.EventHandlerType is null) return;
            var eventHandlerMethodInfo = typeof(RoutedEventIsHandledBehavior).GetMethod("OnEventImpl", BindingFlags.NonPublic | BindingFlags.Instance);
            if (eventHandlerMethodInfo is null) return;
            m_Delegate = Delegate.CreateDelegate(m_EventInfo.EventHandlerType, this, eventHandlerMethodInfo);
            m_EventInfo.AddEventHandler(this.AssociatedObject, m_Delegate);
            
            base.OnAttached();
        }

        private void OnEventImpl(object sender, RoutedEventArgs eventArgs)
        {
            eventArgs.Handled = IsHandled;
        }

        protected override void OnDetaching()
        {
            m_EventInfo?.RemoveEventHandler(this.AssociatedObject, m_Delegate);
            base.OnDetaching();
        }
    }
}
