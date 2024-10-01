using QSoft.WPF.ValueConvert;
using System;
using System.Collections.Generic;
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

namespace WpfAppNET472
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainUI();
        }
    }

    public class MainUI : INotifyPropertyChanged
    {
        TTT m_TTT;
        public TTT TTT
        {
            set { m_TTT = value; Update("TTT"); }
            get { return m_TTT; }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public enum TTT
    {
        One = 100,
        Two = 101, 
        Three,
        Four,
        Five
    };

    public class TTT2: Enum2RadioButton<TTT>
    {

    }
}
