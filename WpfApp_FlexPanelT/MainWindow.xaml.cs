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
            //item.Width = 100;
            //FlexPanel.SetBasis(item, 100);
            item.Delete += Item_Delete;
            item.Edit += Item_Edit;
            this.flexpanel.Children.Add(item);

        }

        FrameworkElement? m_EditSelfObj;

        private void Item_Edit(object sender, RoutedEventArgs e)
        {
            if(sender is FrameworkElement dp)
            {
                m_EditSelfObj = dp;
                if(dp.DataContext is FlexItemVM vm)
                {
                    this.m_MainUI.ItemData = vm;
                }
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

        private void textbox_flexbasis_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                FlexPanel.SetBasis(this.m_EditSelfObj, this.m_MainUI.ItemData.FlexBasis);
            }
        }

        private void checkbox_ishowscrollbar_Checked(object sender, RoutedEventArgs e)
        {
            scollviwer_panel.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scollviwer_panel.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void checkbox_ishowscrollbar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                ShowScrollbar(checkbox.IsChecked == true);
            }
        }

        void ShowScrollbar(bool data)
        {
            if (data)
            {
                scollviwer_panel.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                scollviwer_panel.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                scollviwer_panel.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scollviwer_panel.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ShowScrollbar(checkbox_ishowscrollbar.IsChecked == true);
        }

        private void textbox_minwidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.MinWidth = this.m_MainUI.ItemData.MinWidth;
            }
        }

        private void textbox_maxwidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.MaxWidth = this.m_MainUI.ItemData.MaxWidth;
            }
        }

        private void textbox_width_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.Width = this.m_MainUI.ItemData.Width;
            }
        }

        private void textbox_minheight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.MinHeight = this.m_MainUI.ItemData.MinHeight;
            }
        }

        private void textbox_maxheight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.MaxHeight = this.m_MainUI.ItemData.MaxHeight;
            }
        }

        private void textbox_height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                this.m_EditSelfObj.Height = this.m_MainUI.ItemData.Height;
            }
        }

        private void radiobutton_system_Click(object sender, RoutedEventArgs e)
        {

        }

        private void radiobutton_light_Click(object sender, RoutedEventArgs e)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary themeDict = new ResourceDictionary { Source = new Uri("/Theme/Light.xaml", UriKind.RelativeOrAbsolute) };
            dicts.Clear();
            dicts.Add(themeDict);
        }

        private void radiobutton_dark_Click(object sender, RoutedEventArgs e)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;
            ResourceDictionary themeDict = new ResourceDictionary { Source = new Uri("/Theme/Dark.xaml", UriKind.RelativeOrAbsolute) };
            dicts.Clear();
            dicts.Add(themeDict);
        }

        private void textbox_flexshrink_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.m_EditSelfObj is not null && this.m_MainUI.ItemData is not null)
            {
                FlexPanel.SetShrink(this.m_EditSelfObj, this.m_MainUI.ItemData.FlexShrink);
            }
        }
    }

    public  class MainUI: INotifyPropertyChanged
    {
        //public ObservableCollection<FlexItemVM> Items { get; set; } = [];
        FlexItemVM? m_ItemData;
        public FlexItemVM? ItemData
        {
            get => m_ItemData;
            set { m_ItemData = value; Update(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        void Update([CallerMemberName]string name="")=>this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    //public class ItemData
    //{
    //    public AlignSelf AlignSelf { get; set; }
    //    public double FlexGrow { set; get; }
    //    public double FlexBasis { set; get; }
    //}

}