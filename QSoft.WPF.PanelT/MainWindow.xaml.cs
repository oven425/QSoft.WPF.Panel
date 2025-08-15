using QSoft.WPF.Panel;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace QSoft.WPF.PanelT
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //Point relativePoint = button.TranslatePoint(new Point(0, 0), flexpanel);
            //System.Diagnostics.Trace.WriteLine($"{relativePoint} {button.ActualWidth} {button.ActualHeight}");
        }

        private void combobox_alignself_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var Combobox = sender as ComboBox;
            Panel.FlexPanel.SetAlignSelf(Combobox, (AlignSelf)Combobox.SelectedItem);
        }

        private void ItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var mainui = this.DataContext as MainUI;
        }

        private void listbox_fix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bb = sender as ListBox;
            var bf = bb.SelectedValue;
        }
    }

    public class MainUI:INotifyPropertyChanged
    {
        public ObservableCollection<QSoft.WPF.Panel.JustifyContent> JustifyContents { get; set; } =
        [
            QSoft.WPF.Panel.JustifyContent.Start,
            QSoft.WPF.Panel.JustifyContent.End,
            QSoft.WPF.Panel.JustifyContent.Center,
            QSoft.WPF.Panel.JustifyContent.SpaceAround,
            QSoft.WPF.Panel.JustifyContent.SpaceBetween,
            QSoft.WPF.Panel.JustifyContent.SpaceEvenly,
        ];

        public ObservableCollection<QSoft.WPF.Panel.AlignItems> AlignItems { get; set; } =
        [
            QSoft.WPF.Panel.AlignItems.Start,
            QSoft.WPF.Panel.AlignItems.End,
            QSoft.WPF.Panel.AlignItems.Center,
            QSoft.WPF.Panel.AlignItems.Stretch,
            //QSoft.WPF.Panel.AlignItems.BaeseLine
        ];
        public ObservableCollection<QSoft.WPF.Panel.FlexDirection> FlexDirections { get; set; } =
        [
            QSoft.WPF.Panel.FlexDirection.Row,
            QSoft.WPF.Panel.FlexDirection.Column,
        ];

        public ObservableCollection<QSoft.WPF.Panel.AlignSelf> AlignSelf { get; set; } =
        [
            QSoft.WPF.Panel.AlignSelf.Auto,
            QSoft.WPF.Panel.AlignSelf.Start,
            QSoft.WPF.Panel.AlignSelf.End,
            QSoft.WPF.Panel.AlignSelf.Center,
            QSoft.WPF.Panel.AlignSelf.Stretch,
        ];

        string m_RadioButton = "RadioButton4";
        public string RadioButton
        {
            get => m_RadioButton;
            set
            {
                if (m_RadioButton != value)
                {
                    m_RadioButton = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RadioButton)));
                }
            }
        }

        public ObservableCollection<string> RadioButtons { get; set; } =
        [
            "RadioButton1",
            "RadioButton2",
            "RadioButton3",
            "RadioButton4"
        ];

        int m_Fix = 2;
        public int Fix
        {
            get => m_Fix;
            set
            {
                if (m_Fix != value)
                {
                    m_Fix = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fix)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            this.Loaded += (s, e) =>
            {
                var v = PresentationSource.FromVisual(this);
                if(v is not null)
                {
                    Matrix m = v.CompositionTarget.TransformToDevice;
                    ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                    if (dpiTransform.CanFreeze)
                        dpiTransform.Freeze();
                    this.LayoutTransform = dpiTransform;
                }
                
            };
        }
    }
}