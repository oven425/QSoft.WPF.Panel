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
using static WpfApp_TreeView.TreeD_Select;

namespace WpfApp_TreeView
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
        MainUI m_MainUI;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.m_MainUI == null)
            {
                this.DataContext = this.m_MainUI = new MainUI();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }

    public class MainUI
    {
        public ObservableCollection<TreeD> TreeDs { set; get; } = []; 
        public MainUI()
        {
            TreeDs.Add(new TreeD_Select()
            {
                Name = "List",
                SelectDatas =
                [
                    new()
                    {
                        Name = "AAAA", 
                        TreeDs = 
                        [
                            new TreeD_Change(){Name="Change1"},
                            new TreeD_Change(){Name="Change2"},
                            new TreeD_Select()
                            {
                                Name="List11",
                                SelectDatas = 
                                [
                                    new()
                                    {
                                        Name = "List1_A",
                                        TreeDs = 
                                        [
                                            new TreeD_Change(){Name = "Change1"}
                                        ]
                                    }
                                ]
                            }
                        ]
                    },
                    new(){Name = "BBBB"},
                    new(){Name = "CCCC"}
                ]
            });
        }
    }

    public class TreeD
    {
        public string Name { set; get; }
    }

    

    public class TreeD_Enable : TreeD
    {
        public class EnableD
        {
            public bool IsEnable { set; get; }
        }
        public ObservableCollection<EnableD> Items { set; get; }
    }

    public class TreeD_Change:TreeD
    {
        public string ChangeID { set; get; }
    }


    public class SelectData
    {
        public string Name { set; get; }
        public ObservableCollection<TreeD> TreeDs { get; set; }
    }

    public class TreeD_Select : TreeD
    {
        
        public ObservableCollection<SelectData> SelectDatas { get; set; }
    }
}