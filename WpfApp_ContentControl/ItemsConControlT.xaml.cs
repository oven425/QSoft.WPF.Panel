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
    /// ItemsConControlT.xaml 的互動邏輯
    /// </summary>
    public partial class ItemsConControlT : UserControl
    {
        public ItemsConControlT()
        {
            InitializeComponent();
            this.DataContext = new ItemsConControlTVM();
        }
    }

    public class ItemsConControlTVM : INotifyPropertyChanged
    {
        public ObservableCollection<object> Items { get; set; } = [new IntVM(), new TextVM(), new GenericVMInt()];
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name)=>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));   
    }

    public class GenericVM<T>
    {

    }

    public class GenericVMInt: GenericVM<int>
    {

    }
    public class GenericVMText : GenericVM<string>
    {

    }
}
