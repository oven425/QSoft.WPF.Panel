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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var vv = comboBox.SelectedValue;
            var citem
                 = comboBox.SelectedItem as ComboBoxItem;
            var fr = citem.Content.GetType();
        }

        private void button_addflex_Click(object sender, RoutedEventArgs e)
        {
            this.flexpanel.Children.Add(new FlexItem(new FlexItemVM()
            { Name = $"index:{this.flexpanel.Children.Count}" }));
        }
    }

}