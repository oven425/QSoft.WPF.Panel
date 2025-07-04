using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace WpfApp_TestObejctBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainVM()
            {
                SubVM = new SubVM()
            };
        }
    }

    public partial class MainVM:ObservableObject
    {
        [ObservableProperty]
        object subVM;
    }

    public partial class SubVM: ObservableObject
    {
        [ObservableProperty]
        string _name = "SubVM";

        [RelayCommand(CanExecute = nameof(CanGreetUser))]
        void TT(ObservableCollection<int> user)
        {
            System.Diagnostics.Trace.WriteLine("AAA");
        }
        private bool CanGreetUser(ObservableCollection<int> user)
        {
            return user.Count>0;
        }
        [RelayCommand]
        void Add()
        {
            Numbers.Add(0);
        }

        public ObservableCollection<int> Numbers { get; } = [];
    }
}