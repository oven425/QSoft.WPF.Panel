using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainUI m_MainUI;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = m_MainUI = new MainUI()
            {
                VM = new TextVM()
            };
        }

        private void button_text_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM = new TextVM();
        }

        private void button_int_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM =new IntVM();
        }

        private void button_null_Click(object sender, RoutedEventArgs e)
        {
            this.m_MainUI.VM = null;
        }
    }

    public class MainUI : INotifyPropertyChanged
    {
        object m_VM;
        public object VM
        { set { m_VM = value; Update("VM"); } get { return m_VM; } }
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); 
        }
    }

    public class TextVM
    {
        public string Text { set; get; } = DateTime.Now.ToString();
    }

    public class IntVM
    {
        public int Value { set; get; } = DateTime.Now.Minute;
    }
}