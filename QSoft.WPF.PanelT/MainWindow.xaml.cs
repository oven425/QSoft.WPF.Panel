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

namespace QSoft.WPF.PanelT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainUI();
        }
    }

    public class MainUI
    {
        public ObservableCollection<QSoft.WPF.Panel.JustifyContent> JustifyContents { get; set; } =
        [
            QSoft.WPF.Panel.JustifyContent.Left,
            QSoft.WPF.Panel.JustifyContent.Right,
            QSoft.WPF.Panel.JustifyContent.Center,
            QSoft.WPF.Panel.JustifyContent.SpaceAround,
            QSoft.WPF.Panel.JustifyContent.SpaceBetween
        ];
    }
}