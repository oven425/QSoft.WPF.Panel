using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

namespace WpfApp_ContentControl
{
    /// <summary>
    /// ContentView.xaml 的互動邏輯
    /// </summary>
    public partial class ContentView : UserControl
    {
        public ContentView()
        {
            InitializeComponent();
            this.DataContext = m_MainUI = new ContentViewVM();
        }
        ContentViewVM m_MainUI;
        private void button_text_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM = new TextVM();
        }

        private void button_int_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM = new IntVM();
        }

        private void button_null_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM = null;
        }
    }

    public class ContentViewVM : INotifyPropertyChanged
    {
        object m_VM;
        public object VM
        { set { m_VM = value; Update("VM"); } get { return m_VM; } }
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name)=>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    }

}
