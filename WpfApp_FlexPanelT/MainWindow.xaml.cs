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

namespace WpfApp_FlexPanelT
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
            this.DataContext = this.m_MainUI = new MainUI();
        }

        private void button_addflex_Click(object sender, RoutedEventArgs e)
        {
            var item = new FlexItem(new FlexItemVM()
            { Name = $"index:{this.flexpanel.Children.Count}" });
            item.Delete += Item_Delete;

            this.flexpanel.Children.Add(item);

        }

        private void Item_Delete(object sender, RoutedEventArgs e)
        {
            var item = sender as FlexItem;
            this.flexpanel.Children.Remove(item);
        }
    }

    public  class MainUI
    {
        public ObservableCollection<FlexItemVM> Items { get; set; } = new ObservableCollection<FlexItemVM>();
    }

}