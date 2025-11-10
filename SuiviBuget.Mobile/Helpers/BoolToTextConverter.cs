using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuiviBuget.Mobile.Helpers
{
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Oui";
        public string FalseText { get; set; } = "Non";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? TrueText : FalseText;
            }
            return FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return s.Equals(TrueText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
