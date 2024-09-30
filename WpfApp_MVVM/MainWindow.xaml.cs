using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

namespace WpfApp_MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.DataContext = new VN1<Test>();
            //this.DataContext = new VN2();
            this.DataContext = new VN_VM();
        }

        private void radiobutton_1_Click(object sender, RoutedEventArgs e)
        {
            if(this.DataContext is VN1<Test> vn1)
            {
                vn1.Data = new Test() { Age = DateTime.Now.Second, Name = DateTime.Now.Second.ToString() };
            }
            else if(this.DataContext is VN_VM vn_vm)
            {
                vn_vm.VN1 = new VN1<Test>()
                {
                    Data = new Test() { Age = DateTime.Now.Second, Name = DateTime.Now.Second.ToString() }
                };
            }
        }
    }


    public enum VNStates
    {
        one, two, three
    }
    public partial class VN_VM:ObservableObject
    {
        [ObservableProperty]
        VN1<Test> vN1;

        [ObservableProperty]
        VN2 vN2;
    }

    public partial class VN1<T> : ObservableObject
    {
        [ObservableProperty]
        T? data;
    }

    public partial class VN2: ObservableValidator
    {
        [Required]
        public string? Name { set; get; }

        [Required]
        public int Age { set; get; }

        [RelayCommand]
        void Save()
        {
            this.ValidateAllProperties();
        }
        [RelayCommand]
        void Cancel()
        {

        }
    }


    public class Test
    {
        public string? Name { set; get; }

        public int Age { set; get; }
    }

    public class TestEdit(Test? data): ObservableValidator
    {
        [Required]
        public string? Name { set; get; } = data?.Name ?? "";

        [Required]
        public int Age { set; get; }= data?.Age ?? 0;
    }

    
}