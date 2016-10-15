using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class LineNumberVM : DependencyObject
    {
        #region Margin Property Members

        public const string DependencyPropertyName_Margin = "Margin";

        /// <summary>
        /// Identifies the <see cref="Margin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register(DependencyPropertyName_Margin, typeof(Thickness), typeof(LineNumberVM),
                new PropertyMetadata(default(Thickness),
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LineNumberVM).Margin_PropertyChanged((Thickness)(e.OldValue), (Thickness)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as LineNumberVM).Margin_PropertyChanged((Thickness)(e.OldValue), (Thickness)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as LineNumberVM).Margin_CoerceValue(baseValue)));

        /// <summary>
        /// Margin for displaying line numbers.
        /// </summary>
        public Thickness Margin
        {
            get
            {
                if (CheckAccess())
                    return (Thickness)(GetValue(MarginProperty));
                return Dispatcher.Invoke(() => Margin);
            }
            set
            {
                if (CheckAccess())
                    SetValue(MarginProperty, value);
                else
                    Dispatcher.Invoke(() => Margin = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Margin"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Thickness"/> value before the <seealso cref="Margin"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Thickness"/> value after the <seealso cref="Margin"/> property was changed.</param>
        protected virtual void Margin_PropertyChanged(Thickness oldValue, Thickness newValue)
        {
            // TODO: Implement LineNumberVM.Margin_PropertyChanged(Thickness, Thickness)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Margin"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Thickness Margin_CoerceValue(object baseValue)
        {
            // TODO: Implement LineNumberVM.Margin_CoerceValue(DependencyObject, object)
            return (Thickness)baseValue;
        }

        #endregion

        #region Number Property Members

        public const string DependencyPropertyName_Number = "Number";

        /// <summary>
        /// Identifies the <see cref="Number"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(DependencyPropertyName_Number, typeof(int), typeof(LineNumberVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LineNumberVM).Number_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as LineNumberVM).Number_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as LineNumberVM).Number_CoerceValue(baseValue)));

        /// <summary>
        /// Line number
        /// </summary>
        public int Number
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(NumberProperty));
                return Dispatcher.Invoke(() => Number);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NumberProperty, value);
                else
                    Dispatcher.Invoke(() => Number = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Number"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Number"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Number"/> property was changed.</param>
        protected virtual void Number_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement LineNumberVM.Number_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Number"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int Number_CoerceValue(object baseValue)
        {
            // TODO: Implement LineNumberVM.Number_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion
    }
}