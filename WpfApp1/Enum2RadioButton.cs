using System.Globalization;
using System.Windows.Data;

namespace QSoft.WPF.ValueConvert
{
    public class Enum2RadioButton<TEnum> : IValueConverter where TEnum : Enum
    {
        public Enum2RadioButton()
        {
            var name = typeof(TEnum).GetEnumNames();
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum AAs
    {
        One, Two, Three
    }

    public class Enum2RadioButton1:Enum2RadioButton<AAs>
    {

    }
        
}

