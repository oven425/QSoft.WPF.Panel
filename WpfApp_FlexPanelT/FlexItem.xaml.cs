using QSoft.WPF.Panel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_FlexPanelT
{
    /// <summary>
    /// FlexItem.xaml 的互動邏輯
    /// </summary>
    public partial class FlexItem : UserControl
    {
        public  static readonly RoutedEvent DeleteEvent = EventManager.RegisterRoutedEvent("Delete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlexItem));
        public event RoutedEventHandler Delete
        {             
            add { AddHandler(DeleteEvent, value); }
            remove { RemoveHandler(DeleteEvent, value); }
        }
        public static readonly RoutedEvent EditEvent = EventManager.RegisterRoutedEvent("Edit", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlexItem));
        public event RoutedEventHandler Edit
        {
            add { AddHandler(EditEvent, value); }
            remove { RemoveHandler(EditEvent, value); }
        }
        public FlexItem(FlexItemVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DeleteEvent));
        }

        private void button_edit_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(EditEvent));
        }
    }

    public class FlexItemVM : INotifyPropertyChanged
    {
        string m_Name = string.Empty;
        public string Name
        {
            get => m_Name; set
            {
                m_Name = value; Update();
            }
        }
        public AlignSelf AlignSelf { get; set; }
        public double FlexGrow { set; get; }
        public double FlexBasis { set; get; }
        public double FlexShrink { set; get; }
        public double MinWidth { set; get; }
        public double MaxWidth { set; get; } = double.PositiveInfinity;
        public double Width { set; get; } = double.NaN;

        public double MinHeight { set; get; }
        public double MaxHeight { set; get; } = double.PositiveInfinity;
        public double Height { set; get; } = double.NaN;
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update([CallerMemberName] string name="")=>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
