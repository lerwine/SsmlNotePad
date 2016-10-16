using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="string"/> values to  <seealso cref="Visibility"/> values.
    /// </summary>
    [ValueConversion(typeof(Model.XmlValidationStatus), typeof(Style))]
    public class StatusTypeToStyleConverter : ToClassConverterBase<Model.XmlValidationStatus, Style>
    {
        #region None Property Members

        /// <summary>
        /// Defines the name for the <see cref="None"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_None = "None";

        /// <summary>
        /// Identifies the <see cref="None"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneProperty = DependencyProperty.Register(DependencyPropertyName_None, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.None"/>.
        /// </summary>
        public Style None
        {
            get { return GetValue(NoneProperty) as Style; }
            set { SetValue(NoneProperty, value); }
        }

        #endregion

        #region Information Property Members

        /// <summary>
        /// Defines the name for the <see cref="Information"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Information = "Information";

        /// <summary>
        /// Identifies the <see cref="Information"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InformationProperty = DependencyProperty.Register(DependencyPropertyName_Information, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Information"/>.
        /// </summary>
        public Style Information
        {
            get { return GetValue(InformationProperty) as Style; }
            set { SetValue(InformationProperty, value); }
        }

        #endregion

        #region Warning Property Members

        /// <summary>
        /// Defines the name for the <see cref="Warning"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Warning = "Warning";

        /// <summary>
        /// Identifies the <see cref="Warning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WarningProperty = DependencyProperty.Register(DependencyPropertyName_Warning, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Warning"/>.
        /// </summary>
        public Style Warning
        {
            get { return GetValue(WarningProperty) as Style; }
            set { SetValue(WarningProperty, value); }
        }

        #endregion

        #region Error Property Members

        /// <summary>
        /// Defines the name for the <see cref="Error"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Error = "Error";

        /// <summary>
        /// Identifies the <see cref="Error"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.Register(DependencyPropertyName_Error, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Error"/>.
        /// </summary>
        public Style Error
        {
            get { return GetValue(ErrorProperty) as Style; }
            set { SetValue(ErrorProperty, value); }
        }

        #endregion

        #region Critical Property Members

        /// <summary>
        /// Defines the name for the <see cref="Critical"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Critical = "Critical";

        /// <summary>
        /// Identifies the <see cref="Critical"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CriticalProperty = DependencyProperty.Register(DependencyPropertyName_Critical, typeof(Style), typeof(StatusTypeToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// Visibility value when source is <see cref="XmlValidationStatus.Critical"/>.
        /// </summary>
        public Style Critical
        {
            get { return GetValue(CriticalProperty) as Style; }
            set { SetValue(CriticalProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="Model.XmlValidationStatus"/> value to a <seealso cref="Style"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="Model.XmlValidationStatus"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="Model.XmlValidationStatus"/> value converted to a <seealso cref="Style"/> or null value.</returns>
        public override Style Convert(Model.XmlValidationStatus value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Model.XmlValidationStatus.Critical:
                    return Critical;
                case Model.XmlValidationStatus.Error:
                    return Error;
                case Model.XmlValidationStatus.Warning:
                    return Warning;
                case Model.XmlValidationStatus.Information:
                    return Information;
            }

            return None;
        }
    }
}
