using MarketPrice.Ui.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPrice.Ui.Converters
{
    /// <summary>
    /// Return a color based on a price state. 
    /// ConverterParameter: "BidBg" |"BidBorder" | "OfferBg" | "OfferBorder" | "BidText" | "OfferText"
    /// Binding path must be the CommodityDisplayModel itself.
    /// </summary>
    public class PriceStateBidBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#21262D");

            if (!m.HasBid)
                return Color.FromArgb("#21262D");

            return m.IsBidImproved
            ? Color.FromArgb("#071F14")   
            : Color.FromArgb("#200D10");  
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

    }

    public class PriceStateBidBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#30363D"); // neutral border

            if (!m.HasBid)
                return Color.FromArgb("#30363D"); // neutral border — no data

            return m.IsBidImproved
                ? Color.FromArgb("#0E3D26")   // green border
                : Color.FromArgb("#4A1520");  // red border
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriceStateBidTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#474D57"); // neutral text

            if (!m.HasBid)
                return Color.FromArgb("#474D57"); // muted text — no data

            return m.IsBidImproved
                ? Color.FromArgb("#0ECB81")   // green text
                : Color.FromArgb("#F6465D");  // red text
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriceStateOfferBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#21262D");

            if (!m.HasOffer)
                return Color.FromArgb("#21262D"); // neutral — no data

            return m.IsOfferImproved
                ? Color.FromArgb("#071F14")   // green bg
                : Color.FromArgb("#200D10");  // red bg
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriceStateOfferBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#30363D");

            if (!m.HasOffer)
                return Color.FromArgb("#30363D"); // neutral border — no data

            return m.IsOfferImproved
                ? Color.FromArgb("#0E3D26")   // green border
                : Color.FromArgb("#4A1520");  // red border
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PriceStateOfferTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CommodityDisplayModel m)
                return Color.FromArgb("#474D57");

            if (!m.HasOffer)
                return Color.FromArgb("#474D57"); // muted text — no data

            return m.IsOfferImproved
                ? Color.FromArgb("#0ECB81")   // green text
                : Color.FromArgb("#F6465D");  // red text
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
