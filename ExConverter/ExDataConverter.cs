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
    public class DoubleToStringConverter : IValueConverter
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
    [ValueConversion(typeof(DateTime?), typeof(string))]
    public class DateToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime To String
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? ((DateTime)value).ToShortDateString() : "";
        }
            
        /// <summary>
        /// Converts String To DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture = null)
        {
            string[] fmts;
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
                fmts = new string[] { "dd/MM/yyyy", "dd/MM/yy", "dd-MM-yyyy", "dd-MM-yy", "MM/dd/yyyy", "MM/dd/yy", "MM-dd-yyyy", "MM-dd-yy", "yyyy/MM/dd", "yyyy-MM-dd", "d/M/yyyy", "d/M/yy", "d-M-yyyy", "d-M-yy", "M/d/yyyy", "M/d/yy", "M-d-yyyy", "M-d-yy", "d/MM/yyyy", "d/MM/yy", "d-MM-yyyy", "d-MM-yy", "MM/d/yyyy", "MM/d/yy", "MM-d-yyyy", "MM-d-yy", "dd/M/yyyy", "dd/M/yy", "dd-M-yyyy", "dd-M-yy", "M/dd/yyyy", "M/dd/yy", "M-dd-yyyy", "M-dd-yy" };
            }
            else
                fmts = culture.DateTimeFormat.GetAllDateTimePatterns();
            if (DateTime.TryParseExact(value.ToString(),fmts, culture,
                DateTimeStyles.AssumeLocal, out DateTime resultDateTime))
                return resultDateTime;
            return null;
        }
    }

    /// <summary>
    /// Used With WPF Binding Converter.
    /// Converts String.NullOrEmpty To Visibility
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converst String.NullOrEmpty To Visibity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>If Null Or Empty Returns Visible Else Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Collapsed : Visibility.Visible;
        }
        /// <summary>
        /// Convert Back Is Not Supported
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Used With WPF Binding Converter.
    /// Converts String.NullOrEmpty To Visibility
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringNullOrEmptyToVisibilityConverterInverted : IValueConverter
    {
        /// <summary>
        /// Converst String.NullOrEmpty To Visibity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>If Null Or Empty Returns Collapsed Else Visible</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                ? Visibility.Visible : Visibility.Collapsed;
        }
        /// <summary>
        /// Convert Back Is Not Supported
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// Used With WPF Binding Converter.
    /// Inverts A Boolean Value
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(bool))]
    public class BooleanInverter : IValueConverter
    {
        /// <summary>
        /// Inverts The Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        /// <summary>
        /// Reverts The Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }
    }

    /// <summary>
    /// Used With WPF Binding Converter.
    /// Converts Bool To Any Value You Spesify
    /// </summary>
    [ValueConversion(typeof(bool), typeof(object))]
    public class BoolToValueConverter<T> : IValueConverter
    {
        /// <summary>
        /// The Value When False
        /// </summary>
        public T FalseValue { get; set; }
        /// <summary>
        /// Value When True
        /// </summary>
        public T TrueValue { get; set; }

        /// <summary>
        /// Converts Bool To Any Value You Spesified
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts Your Spesified Value Back Into A Bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
}
