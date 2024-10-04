using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QSoft.WPF.MVVM.Region
{
    public class ContentControlRegion:ContentControl
    {
        static Dictionary<string, ContentControl> m_Dic = [];
        public static readonly DependencyProperty Property = 
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(ContentControlRegion), new PropertyMetadata(NamePropertyChange));
        static void NamePropertyChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_Dic[(string)e.NewValue] = (ContentControl)sender;
        }

        static public ContentControl? Get(string name)
        {
            if(m_Dic.TryGetValue(name, out ContentControl? contentControl)) { return contentControl; }
            return null;
        }

        public static string GetName(ContentControl target)
        {
            return (string)target.GetValue(NameProperty);
        }
        public static void SetName(ContentControl target, string value)
        {
            target.SetValue(NameProperty, value);
        }
    }


    public class NavgationContenControl(string regionanme)
    {
        public void Show(object? obj)
        {
            var cc = ContentControlRegion.Get(regionanme);
            if (cc is null) return;
            cc.Content = obj;
        }
    }
}
