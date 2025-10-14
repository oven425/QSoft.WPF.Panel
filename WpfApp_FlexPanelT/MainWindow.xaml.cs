using QSoft.WPF.Panel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
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
            { Name = $"Index:{this.flexpanel.Children.Count}" });
            item.Width = 100;
            item.Delete += Item_Delete;
            item.Edit += Item_Edit;
            this.flexpanel.Children.Add(item);

        }

        DependencyObject? m_EditSelfObj;

        private void Item_Edit(object sender, RoutedEventArgs e)
        {
            if(sender is DependencyObject dp)
            {
                m_EditSelfObj = dp;
                var item = new ItemData
                {
                    AlignSelf = FlexPanel.GetAlignSelf(dp),
                    FlexGrow = FlexPanel.GetGrow(dp)
                };
                this.m_MainUI.ItemData = item;
                this.tabcontrol.SelectedIndex = 1;
            }
        }

        private void Item_Delete(object sender, RoutedEventArgs e)
        {
            var item = sender as FlexItem;
            this.flexpanel.Children.Remove(item);
            int index = 0;
            foreach(var oo in this.flexpanel.Children)
            {
                if(oo is FlexItem fm && fm.DataContext is FlexItemVM vm)
                {
                    vm.Name = $"Index:{index++}";
                }
            }
        }

        private void combobox_alighself_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                FlexPanel.SetAlignSelf(this.m_EditSelfObj, this.m_MainUI.ItemData.AlignSelf);
            }
        }

        private void textbox_flexgrow_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                FlexPanel.SetGrow(this.m_EditSelfObj, this.m_MainUI.ItemData.FlexGrow);
            }
        }
    }

    public  class MainUI: INotifyPropertyChanged
    {
        public ObservableCollection<FlexItemVM> Items { get; set; } = [];
        ItemData? m_ItemData;
        public ItemData? ItemData
        {
            get => m_ItemData;
            set { m_ItemData = value; Update(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update([CallerMemberName]string name="")=>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ItemData
    {
        public AlignSelf AlignSelf { get; set; }
        public double FlexGrow { set; get; }
    }

}