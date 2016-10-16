using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts <seealso cref="Model.FileSaveStatus"/> values to  <seealso cref="Style"/> values.
    /// </summary>
    [ValueConversion(typeof(Model.FileSaveStatus), typeof(Style))]
    public class FileSaveStatusToStyleConverter : ToClassConverterBase<Model.FileSaveStatus, Style>
    {
        #region New Property Members

        /// <summary>
        /// Defines the name for the <see cref="New"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_New = "New";

        /// <summary>
        /// Identifies the <see cref="New"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NewProperty = DependencyProperty.Register(DependencyPropertyName_New, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// A <seealso cref="Style"/> to indicate that a file is new and has never been saved.
        /// </summary>
        public Style New
        {
            get { return GetValue(NewProperty) as Style; }
            set { SetValue(NewProperty, value); }
        }

        #endregion

        #region Modified Property Members

        /// <summary>
        /// Defines the name for the <see cref="Modified"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Modified = "Modified";

        /// <summary>
        /// Identifies the <see cref="Modified"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModifiedProperty = DependencyProperty.Register(DependencyPropertyName_Modified, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// A <seealso cref="Style"/> to indicate that a file has previously been saved to disk, but has been modified.
        /// </summary>
        public Style Modified
        {
            get { return GetValue(ModifiedProperty) as Style; }
            set { SetValue(ModifiedProperty, value); }
        }

        #endregion

        #region SaveError Property Members

        /// <summary>
        /// Defines the name for the <see cref="SaveError"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SaveError = "SaveError";

        /// <summary>
        /// Identifies the <see cref="SaveError"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveErrorProperty = DependencyProperty.Register(DependencyPropertyName_SaveError, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// A <seealso cref="Style"/> to indicate that an error has occurred while trying to save file to disk.
        /// </summary>
        public Style SaveError
        {
            get { return GetValue(SaveErrorProperty) as Style; }
            set { SetValue(SaveErrorProperty, value); }
        }

        #endregion

        #region SaveWarning Property Members

        /// <summary>
        /// Defines the name for the <see cref="SaveWarning"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SaveWarning = "SaveWarning";

        /// <summary>
        /// Identifies the <see cref="SaveWarning"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveWarningProperty = DependencyProperty.Register(DependencyPropertyName_SaveWarning, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// A <seealso cref="Style"/> to indicate that a warning has been issued while saving file to disk.
        /// </summary>
        public Style SaveWarning
        {
            get { return GetValue(SaveWarningProperty) as Style; }
            set { SetValue(SaveWarningProperty, value); }
        }

        #endregion

        #region SaveSuccess Property Members

        /// <summary>
        /// Defines the name for the <see cref="SaveSuccess"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SaveSuccess = "SaveSuccess";

        /// <summary>
        /// Identifies the <see cref="SaveSuccess"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveSuccessProperty = DependencyProperty.Register(DependencyPropertyName_SaveSuccess, typeof(Style), typeof(FileSaveStatusToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// A <seealso cref="Style"/> to indicate that a file has been successfuly saved to disk and has not been modified.
        /// </summary>
        public Style SaveSuccess
        {
            get { return GetValue(SaveSuccessProperty) as Style; }
            set { SetValue(SaveSuccessProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="Model.FileSaveStatus"/> value to a <seealso cref="Style"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="Model.FileSaveStatus"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="Model.FileSaveStatus"/> value converted to a <seealso cref="Style"/> value.</returns>
        public override Style Convert(Model.FileSaveStatus value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Model.FileSaveStatus.New:
                    return New;
                case Model.FileSaveStatus.Modified:
                    return Modified;
                case Model.FileSaveStatus.SaveError:
                    return SaveError;
                case Model.FileSaveStatus.SaveWarning:
                    return SaveWarning;
            }

            return SaveSuccess;
        }
    }
}
