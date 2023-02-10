using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ExtraFunctions.ExConverter
{
    /// <summary>
    /// Used With WPF Binding Converter.
    /// Converts Double To String And Back
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts Double To String
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value.ToString();

        /// <summary>
        /// Converts String To Double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double d))
                return d;
            return DependencyProperty.UnsetValue;
        }
    }

    /// <summary>
    /// Used With WPF Binding Converter.
    /// Converts DateTime To String And Back
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime To String
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            ((DateTime)value).ToShortDateString();

        /// <summary>
        /// Converts String To DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse(value.ToString(), out DateTime resultDateTime))
                return resultDateTime;
            return DependencyProperty.UnsetValue;
        }
    }
}
