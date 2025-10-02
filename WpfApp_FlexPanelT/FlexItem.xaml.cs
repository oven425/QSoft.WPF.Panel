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
        public FlexItem(FlexItemVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
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
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update([CallerMemberName] string name="")=>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
