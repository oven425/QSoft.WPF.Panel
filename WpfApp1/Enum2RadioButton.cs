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
    public class Enum2RadioButton<TEnum> : IValueConverter where TEnum : struct,Enum
    {
        public Enum2RadioButtonMatches Match { set; get; } = Enum2RadioButtonMatches.Index;
        string[] m_Names = Enum.GetNames<TEnum>();
        TEnum[] enums = Enum.GetValues<TEnum>();
        Dictionary<string, TEnum> m_String2Enum = new Dictionary<string, TEnum>();
        public Enum2RadioButton()
        {
            var names = Enum.GetNames<TEnum>();
            var enums = Enum.GetValues<TEnum>();
            var ints = Enum.GetValuesAsUnderlyingType<TEnum>().OfType<int>().ToArray();
            
            
            for(int i=0; i<names.Length; i++)
            {
                m_String2Enum[ints[i].ToString()] = enums[i];
            }
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var enumValue = (TEnum)value;
            var str = parameter as string;
            if (str is null)
            {
                return false;
            }
            if(m_String2Enum.TryGetValue(str, out var result))
            {
                var bb = Enum.Equals(enumValue, result);
                return bb;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vv = Enum.GetValues(typeof(TEnum)).GetValue(0);
            switch (this.Match)
            {
                case Enum2RadioButtonMatches.Name:
                    {

                    }
                    break;
                case Enum2RadioButtonMatches.Value:
                    {

                    }
                    break;
                case Enum2RadioButtonMatches.Index:
                    {
                        if(int.TryParse(parameter as string, out var index))
                        {

                            var ints = Enum.GetValuesAsUnderlyingType(typeof(TEnum));
                        }
                    }
                    break;
            }
            return vv;
        }
    }


        
}

