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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ObjForTest root = new ObjForTest();
            ObjForTest depart = new ObjForTest("Department", null, "");
            ObjForTest c1 = new ObjForTest("li", 45, "M");
            ObjForTest c2 = new ObjForTest("xu", 30, "W");
            ObjForTest c3 = new ObjForTest("zhang", 22, "M");
            ObjForTest cc1 = new ObjForTest("shen", 30, "M");
            ObjForTest cc2 = new ObjForTest("zhao", 18, "W");
            ObjForTest cc3 = new ObjForTest("wang", 32, "M");
            ObjForTest ccc1 = new ObjForTest("qian", 20, "W");
            root.Children.Add(depart);
            depart.Children.Add(c1);
            depart.Children.Add(c2);
            depart.Children.Add(c3);
            c1.Children.Add(cc1);
            c2.Children.Add(cc2);
            c3.Children.Add(cc3);
            cc1.Children.Add(ccc1);
            //this._list.ItemsSource = root.Children;
            this.qtreelistview.ItemsSource = root.Children;
            //ListView list = new ListView();
            //list.View
        }
    }

    public class ObjForTest
    {
        public ObjForTest() { }
        public ObjForTest(string name, int? age, string sex)
        {

            this._sex = sex;
            this._age = age;
            this._name = name;

        }
        private string _name;
        private int? _age;
        private string _sex;


        public string Sex
        {
            get { return this._sex; }
            set { this._sex = value; }
        }
        public int? Age
        {
            get { return this._age; }
            set { this._age = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

            }
        }

        private ObservableCollection<ObjForTest> _children = new ObservableCollection<ObjForTest>();
        public ObservableCollection<ObjForTest> Children
        {
            get { return _children; }
        }



    }



    
}