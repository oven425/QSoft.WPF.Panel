using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                vm.IsSet2 = !vm.IsSet2;
            }
        }
    }

    public partial class MainWindowVM : BaseVM, INotifyPropertyChanged
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("SetText")]
        bool isSet = false;

        [RelayCommand]
        void Set(bool ss)
        {
            //base.OnPropertyChanged(nameof(SetText));
        }

        public string SetText
        {
            get => IsSet ? "Set" : "Unset";
        }

        bool m_IsSet2 = false;
        public bool IsSet2
        {
            set
                            {
                if (m_IsSet2 != value)
                {
                    m_IsSet2 = value;
                    OnPropertyChanged();
                }
            }
            get => m_IsSet2;
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        public override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            string str = propertyName;
            //base.SetProperty(ref str, true);

            //base.OnPropertyChanged(propertyName);
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class BaseVM:ObservableObject
    {


        public virtual new void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
        }
    }
}