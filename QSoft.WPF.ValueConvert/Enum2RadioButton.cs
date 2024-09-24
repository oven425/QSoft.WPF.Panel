using System.Globalization;
using System.Windows.Data;
using System.Linq;

namespace QSoft.WPF.ValueConvert
{
    public enum Enum2RadioButtonMatches
    {
        Name,
        Value,
        Index
    }
    public class Enum2RadioButton<TEnum> : IValueConverter where TEnum : struct, Enum
    {
        public TEnum Default { set; get; }
        public Enum2RadioButtonMatches Match { set; get; } = Enum2RadioButtonMatches.Index;
        readonly Dictionary<string, TEnum> m_String2Enum;
        public Enum2RadioButton()
        {
            var names = Enum.GetNames<TEnum>();
            var enums = Enum.GetValues<TEnum>();
            this.Default = enums.FirstOrDefault();
            var ints = Enum.GetValuesAsUnderlyingType<TEnum>().OfType<int>().Select(x => x.ToString());
            m_String2Enum = Match switch
            {
                Enum2RadioButtonMatches.Index => ints.Zip(enums).ToDictionary(x => x.First, y => y.Second),
                Enum2RadioButtonMatches.Name => names.Zip(enums).ToDictionary(x => x.First, y => y.Second),
                Enum2RadioButtonMatches.Value => enums.Zip(enums).ToDictionary(x => x.First.ToString(), y => y.Second),
                _ => []
            };

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var enumValue = (TEnum)value;
            var str = parameter as string;
            if (str is null)
            {
                return false;
            }
            if (m_String2Enum.TryGetValue(str, out var result))
            {
                var bb = Enum.Equals(enumValue, result);
                return bb;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vv = this.Default;
            if (parameter is string str)
            {
                vv = this.m_String2Enum[str];
            }
            return vv;
        }
    }



}

