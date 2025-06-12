using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace WpfApp_MVVM_Multi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowVM vm)
            {
                vm.IsSet = !vm.IsSet;
            }
        }
    }

    public partial class MainWindowVM:ObservableObject
    {
        [ObservableProperty]
        bool isSet = false;

        [RelayCommand]
        void Set(bool ss)
        {

        }
    }
}