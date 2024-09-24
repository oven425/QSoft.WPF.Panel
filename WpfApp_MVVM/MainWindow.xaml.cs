using CommunityToolkit.Mvvm.ComponentModel;
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
            var pp = new VN1();
            //pp.Aa = new Test();
            var bb = pp.HasError();
            //pp.Data.HasError();
            //pp.HasError();
        }
    }


    public partial class VN : ObservableValidatorEx
    {
        [ObservableProperty]
        [Required]
        Test aa;
        
    }

    public partial class VN1 : ObservableValidatorEx<Test>
    {
        //[ObservableProperty]
        //[Required]
        //Test data;

        //public override bool HasError()
        //{
        //    this.ValidateAllProperties();
        //    //if(Data is ObservableValidatorEx ex)
        //    //{
        //    //    ex.HasError();
        //    //}

        //    return this.HasErrors;
        //}

        //public override bool HasError()
        //{
        //    var a = base.HasError();
        //    var b = true;
        //    if(Data is not null)
        //    {
        //        b = Data.HasError();
        //    }
        //    if (a) return true;
        //    else if (b) return true;
        //    return false;
        //}
    }

    //public partial class NewVN<T> : VN<T> 
    //{
    //    [ObservableProperty]
    //    [Required]
    //    string? name;
    //    //public override bool HasError()
    //    //{
    //    //    var a = base.HasError();
    //    //    var b = true;
    //    //    if (Data is not null)
    //    //    {
    //    //        b = Data.HasError();
    //    //    }
    //    //    if (a) return true;
    //    //    else if (b) return true;
    //    //    return false;
    //    //}
    //}

    public class Test
    {
        [Required]
        public string? Name { set; get; }

        [Required]
        public int Age { set; get; }
    }

    public partial class TestValidate(Test test): ObservableValidatorEx
    {
        [ObservableProperty]
        [Required]
        string? name = test.Name;

        [ObservableProperty]
        [Required]
        int age = test.Age;
    }

    public class ObservableValidatorEx: ObservableValidator
    {
        virtual public bool HasError()
        {
            this.ValidateAllProperties();
            return this.HasErrors;
        }
    }

    public partial class ObservableValidatorEx<T>: ObservableValidatorEx
    {
        [ObservableProperty]
        [Required]
        T? value;
    }
}