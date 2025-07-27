using System.Globalization;
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            columndef.Width = new GridLength(40);
            this.ellipse.Width = 100;
        }

        private void button_content_Click(object sender, RoutedEventArgs e)
        {
            columndef.Width = GridLength.Auto;
            this.ellipse.Width = 0;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if(columndef.Width != GridLength.Auto)
            {
                var button = sender as Button;
                //button.Margin = new Thickness(-80, 0, 0, 0);
                button.Margin = new Thickness(0, 0, -80, 0);
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (columndef.Width != GridLength.Auto)
            {
                var button = sender as Button;
                button.Margin = new Thickness(0, 0, 0, 0);
            }
        }
    }

    public class TextLengthMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                double offset = Math.Min(text.Length * 6, 60); // 根據文字長度計算左偏移
                return new Thickness(-offset, 0, 0, 0);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}