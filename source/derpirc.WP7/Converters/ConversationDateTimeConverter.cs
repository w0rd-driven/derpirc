using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace derpirc.Converters
{
    public class ConversationDateTimeConverter : IValueConverter
    {
        private string _stringDateFormat;
        private string _stringTimeFormat = "h:mm:sst";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime;
            if (value is DateTime)
            {
                dateTime = value as DateTime?;
                //IFormatProvider format = culture.GetFormat(targetType) as IFormatProvider;
                var format = culture.DateTimeFormat;
                format.AMDesignator = format.AMDesignator.ToLower();
                format.PMDesignator = format.PMDesignator.ToLower();
                var firstDayOfWeek = StartOfWeek(format);
                if (dateTime > firstDayOfWeek)
                {
                    if (dateTime < DateTime.Today)
                        _stringDateFormat = "ddd, ";
                }
                else
                    _stringDateFormat = "M/d, ";
                if (parameter == null || !(parameter is string))
                {
                    return dateTime.Value.ToString(_stringDateFormat + _stringTimeFormat, format);
                }
                else
                {
                    var result = dateTime.Value.ToString(parameter as string, format);
                    return result;
                }
            }
            System.Diagnostics.Debug.WriteLine("Failed to convert DateTime to string, value is not a valid DateTime.");
            return value;
        }

        DateTime StartOfWeek(DateTimeFormatInfo format)
        {
            var firstDayOfWeek = format.FirstDayOfWeek;
            return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - firstDayOfWeek));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
