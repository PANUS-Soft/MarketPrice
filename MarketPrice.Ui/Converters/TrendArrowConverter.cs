using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Converters
{
    public class TrendArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool up && up ? "▲" : "▼";
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class TrendColorConverter : IValueConverter
    {
        public bool IsOffer { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool trendUp = value is bool b && b;
            if (IsOffer)
                return trendUp ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            else
                return trendUp ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
           => throw new NotImplementedException();
    }
}
