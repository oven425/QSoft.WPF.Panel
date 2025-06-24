using System.Collections.ObjectModel;
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

namespace QSoft.WPF.ButtonGroupT
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
            this.DataContext = m_MainUI = new MainUI();
        }
    }

    public class MainUI
    {
        public ObservableCollection<Sized> Items { get; set; } = 
            [
                new Sized { Width = 100, Height = 100, Name = "Button1" },
                new Sized { Width = 100, Height = 100, Name = "Button2" },
                new Sized { Width = 100, Height = 100, Name = "Button3" },
                new Sized { Width = 100, Height = 100, Name = "Button4" },
                new Sized { Width = 100, Height = 100, Name = "Button5" }
            ];
    }

    public class  Sized
    {
        public int Width { set; get; }
        public int Height { set; get; }
        public string Name { set; get; }
    }
}