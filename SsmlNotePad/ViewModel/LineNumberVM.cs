using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// View model for displaying line numbers.
    /// </summary>
    public class LineNumberVM : DependencyObject
    {
        #region Margin Property Members

        /// <summary>
        /// Defines the name for the <see cref="Margin"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Margin = "Margin";

        /// <summary>
        /// Identifies the <see cref="Margin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register(DependencyPropertyName_Margin, typeof(Thickness), typeof(LineNumberVM),
                new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Margin for displaying line numbers.
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)(GetValue(MarginProperty)); }
            set { SetValue(MarginProperty, value); }
        }

        #endregion

        #region Number Property Members

        /// <summary>
        /// Defines the name for the <see cref="Number"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Number = "Number";

        /// <summary>
        /// Identifies the <see cref="Number"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(DependencyPropertyName_Number, typeof(int), typeof(LineNumberVM),
                new PropertyMetadata(1));

        /// <summary>
        /// Line number
        /// </summary>
        public int Number
        {
            get { return (int)(GetValue(NumberProperty)); }
            set { SetValue(NumberProperty, value); }
        }

        #endregion

        public LineNumberVM() { }

        public LineNumberVM(int number, double marginTop)
        {
            Number = number;
            Margin = new Thickness(0.0, marginTop, 0.0, 0.0);
        }
    }
}