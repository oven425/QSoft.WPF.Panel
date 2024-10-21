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
            this.DataContext = new MainUI();
        }

    }

    public partial class MainUI:ObservableObject
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        //[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string firstName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        //[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string lastName = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        bool isEnableButton;

        [RelayCommand(CanExecute = nameof(CanSaveExecute), IncludeCancelCommand = true)]
        private Task SaveAsync(CancellationToken cancelToken)
        {
            // Code to save the user details
            return Task.CompletedTask;
        }


        private bool CanSaveExecute()
            => this.IsEnableButton;


        //private bool CanSaveExecute()
        //    => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);
    }
}