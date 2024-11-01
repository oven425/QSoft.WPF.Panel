using System.Globalization;
using System.Windows.Data;
using System.Linq;

namespace QSoft.WPF.ValueConvert
{
    public enum Enum2RadioButtonMatches
    {
        Name,
#if NET8_0_OR_GREATER
        Index
#endif
    }
    public class Enum2RadioButton<TEnum> : IValueConverter where TEnum : struct, Enum
    {
        //public TEnum Default { set; get; }
        public Enum2RadioButtonMatches Match { set; get; } = Enum2RadioButtonMatches.Name;
        //Dictionary<string, TEnum>? m_String2Enum;
        public Enum2RadioButton()
        {
            var names = Enum.GetNames(typeof(TEnum));
            var enums = Enum.GetValues(typeof(TEnum)).OfType<TEnum>();
            //this.Default = enums.First();
            //var tt = Enum.GetUnderlyingType(typeof(TEnum));


            //var ints = Enum.GetValuesAsUnderlyingType<TEnum>().OfType<int>().Select(x => x.ToString());
            //m_String2Enum = Match switch
            //{
            //    Enum2RadioButtonMatches.Index => ints.Zip(enums).ToDictionary(x => x.First, y => y.Second),
            //    Enum2RadioButtonMatches.Name => names.Zip(enums).ToDictionary(x => x.First, y => y.Second),
            //    Enum2RadioButtonMatches.Value => enums.Zip(enums).ToDictionary(x => x.First.ToString(), y => y.Second),
            //    _ => []
            //};


        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(this.Match == Enum2RadioButtonMatches.Name)
            {
                var src = value.ToString();
                if (parameter is string str)
                {
                    return src == str;
                }
                
            }
#if NET8_0_OR_GREATER
            else if(this.Match== Enum2RadioButtonMatches.Index)
            {
                var tt = Enum.GetUnderlyingType(typeof(TEnum));
            }
#endif

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(Match == Enum2RadioButtonMatches.Name)
            {
                var names = Enum.GetNames(typeof(TEnum));
                var enums = Enum.GetValues(typeof(TEnum));
                if (names is not null && enums is not null)
                {
                    bool bb = (bool)value;
                    var inff = Array.IndexOf(names, parameter.ToString());
                    if(inff != -1)
                    {
                        return enums.GetValue(inff);
                    }
                    
                }
            }
#if NET8_0_OR_GREATER
            else if (this.Match == Enum2RadioButtonMatches.Index)
            {

            }
#endif
            return null;
        }
    }



}

