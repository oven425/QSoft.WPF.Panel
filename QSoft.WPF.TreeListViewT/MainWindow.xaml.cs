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

namespace QSoft.WPF.TreeListViewT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new TreeListView2VM();
        }
    }

    public class TreeListView2VM
    {
        public ObservableCollection<Group2> TreeDs { set; get; } = [];
        public TreeListView2VM()
        {
            this.TreeDs.Add(new Group2()
            {
                GroupName = "Group1",
                Items =
                [
                    new(){ItemName = "Item1"},
                    new(){ItemName = "Item2"}
                ]
            });
        }
    }

    public class Group2
    {
        public string GroupName { set; get; }
        public ObservableCollection<Item2> Items { set; get; } = [];
    }

    public class Item2
    {
        public string ItemName { set; get; }
    }
}