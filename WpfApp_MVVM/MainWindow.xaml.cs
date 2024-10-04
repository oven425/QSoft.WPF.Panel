using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
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
            var dic = new Dictionary<string, People>();
            dic["1"] = new People("1", 11);
            dic["2"] = new People("2", 22);
            dic["3"] = new People("3", 33);
            dic["4"] = new People("4", 44);
            this.DataContext = m_MainUI = new MainUI<People>(dic);
        }
        MainUI<People> m_MainUI;

    }

    public class vvvs:DataTemplateSelector
    {
        public DataTemplate Show { set; get; }
        public DataTemplate Modify { set; get; }
        public DataTemplate Create { set; get; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is null) return base.SelectTemplate(item, container);
            //var tt = item.GetType().BaseType;
            //if (tt.IsGenericType)
            //{
            //    var bbb = tt.GetGenericTypeDefinition() == typeof(ParameterVM_New<>);
            //    if(bbb)
            //    {
            //        return this.Create;
            //    }
            //}

            var int11 = item switch
            {
                
                object=>10,
                _=>-1
            };
            return this.Show;
            return base.SelectTemplate(item, container);
        }
    }

    public partial class MainUI<TParameter>(Dictionary<string, TParameter> parameters) : ObservableObject
    {
        public ObservableCollection<string> ParametersNames { set; get; }= new ObservableCollection<string>(parameters.Keys);
        [ObservableProperty]
        string? parametersName= parameters.Keys.FirstOrDefault();
        [ObservableProperty]
        object? vM = parameters.Values.FirstOrDefault();

        [RelayCommand]
        void Save()
        {

        }
        [RelayCommand]
        void Cancel()
        {
            this.VM = null;
        }
        [RelayCommand]
        void New()
        {
            var item = Ioc.Default.GetService<ParameterVM_New<TParameter>>();
            var tt = item.GetType().BaseType;
            if(tt.IsGenericType)
            {
                var bbb = tt.GetGenericTypeDefinition() == typeof(ParameterVM_New<>);
                bbb = false;
            }
            this.VM = Ioc.Default.GetService<ParameterVM_New<TParameter>>();
        }

        [RelayCommand]
        void Modify()
        {
            if (this.ParametersName is null) return;
            if (parameters.TryGetValue(this.ParametersName, out var ddd))
            {
                var aa = Ioc.Default.GetService<ParameterVM_Modify<TParameter>>();
                aa.FromParameter(ddd);
                this.VM = aa;

            }
            
        }

    }


    public class ObservableValidatorEx: ObservableValidator
    {
        public void ValidateAll()
        {
            this.ValidateAllProperties();
        }
    }

    public abstract class ParameterVM_New<T> : ObservableValidatorEx
    {
        public abstract T? ToParameter();
    }
    public abstract class ParameterVM_Modify<T> : ObservableValidatorEx
    {
        public abstract void FromParameter(T parameter);
        public abstract T? ToParameter();
    }

    public partial class ParameterVM_Modify_People : ParameterVM_Modify<People>
    {
        [Required]
        [ObservableProperty]
        string? name;
        [Required]
        [ObservableProperty]
        int age;

        public override void FromParameter(People parameter)
        {
            this.Name = parameter.Name;
            this.Age = parameter.Age;
        }

        public override People? ToParameter() => new(this.Name ?? "", this.Age);
    }


    public partial class ParameterVM_New_People : ParameterVM_New<People>
    {
        [Required]
        [ObservableProperty]
        string? itemName;
        [Required]
        [ObservableProperty]
        string? name;
        [Required]
        [ObservableProperty]
        int age;

        public override People? ToParameter()=> new(this.Name??"", this.Age);
    }

    public record People(string Name, int Age);
}