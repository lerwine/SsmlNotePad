using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="SayAs"/> values to display text.
    /// </summary>
    [ValueConversion(typeof(SayAs), typeof(string))]
    public class SayAsToStringConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Converts a <seealso cref="SayAs"/> value to a <seealso cref="string"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="SayAs"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="SayAs"/> value converted to a <seealso cref="string"/> value.</returns>
        public string Convert(SayAs? value, object parameter, CultureInfo culture)
        {
            if (!value.HasValue)
                return "";

            switch (value.Value)
            {
                case SayAs.SpellOut:
                    return "Spell the word or phrase.";
                case SayAs.NumberOrdinal:
                    return "Speak a number as an ordinal number.";
                case SayAs.NumberCardinal:
                    return "peak a number as a cardinal number.";
                case SayAs.Date:
                    return "Speak a number sequence as a date.";
                case SayAs.DayMonthYear:
                    return "Speak a number sequence as a date including the day, month, and year.";
                case SayAs.MonthDayYear:
                    return "Speak a number sequence as a date including the day, month, and year.";
                case SayAs.YearMonthDay:
                    return "Speak a number sequence as a date including the day, month, and year.";
                case SayAs.YearMonth:
                return "Speak a number sequence as a year and month";
                case SayAs.MonthYear:
                    return "Speak a number sequence as a month and year.";
                case SayAs.MonthDay:
                    return "Speak a number sequence as a month and day.";
                case SayAs.DayMonth:
                    return "Speak a number sequence as a day and month.";
                case SayAs.Year:
                    return "Speak a number as a year.";
                case SayAs.Month:
                    return "Speak a word as a month.";
                case SayAs.Day:
                    return "Speak a number as the day in a date.";
                case SayAs.Time:
                    return "Speak a number sequence as a time.";
                case SayAs.Time24:
                    return "Speak a number sequence as a time using the 24-hour clock.";
                case SayAs.Time12:
                    return "Speak a number sequence as a time using the 12-hour clock.";
                case SayAs.Telephone:
                    return "Speak a number sequence as a U.S. telephone number.";
            }
            return String.Format("Speak the word or phrase as {0}.", value.Value.ToString("F").ToLower());
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as SayAs?, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
