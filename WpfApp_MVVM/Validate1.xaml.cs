using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

namespace WpfApp_MVVM
{
    /// <summary>
    /// Validate1.xaml 的互動邏輯
    /// </summary>
    public partial class Validate1 : UserControl
    {
        public Validate1()
        {
            InitializeComponent();
            this.DataContext = new Validate1VM();
        }
    }

    public partial class Validate1VM :  ObservableValidator
    {
        [Required]
        [ListCount<int>]
        public ObservableCollection<int> Datas { set; get; } = [];

        [Required]
        [ObservableProperty]
        string? name;

        [Required]
        [ObservableProperty]
        int? age;

        [RelayCommand]
        void Save()
        {
            this.ValidateAllProperties();
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ListCountAttribute<T> : ValidationAttribute
    { 
        public override bool IsValid(object? value)
        {
            if(value is IEnumerable<T> ee)
            {
                return ee.Any();
            }
            return false;
        }
    }

    public class ObservableValidatorEx: ObservableValidator
    {
        public void ValidateAll()
        {
            this.ValidateAllProperties();
        }
    }

}
