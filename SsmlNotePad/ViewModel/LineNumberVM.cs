using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// Represents a displayed line number.
    /// </summary>
    public class LineNumberVM : DependencyObject
    {
        #region Number Property Members

        public const string DependencyPropertyName_Number = "Number";

        /// <summary>
        /// Identifies the <seealso cref="Number"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(DependencyPropertyName_Number, typeof(int), typeof(LineNumberVM),
                new PropertyMetadata(1));

        /// <summary>
        /// Line number to display
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

        #endregion

        #region Margin Property Members

        public const string DependencyPropertyName_Margin = "Margin";

        /// <summary>
        /// Identifies the <seealso cref="Margin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty = DependencyProperty.Register(DependencyPropertyName_Margin, typeof(Thickness), typeof(LineNumberVM),
                new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// Margin to utilize when positioning the line number control.
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

        #endregion

        /// <summary>
        /// Create new line number view model object with a line number of 1 and a zero margin.
        /// </summary>
        public LineNumberVM() : this(1, 0.0) { }

        /// <summary>
        /// Create new line number view model object.
        /// </summary>
        /// <param name="number">Line number to be displayed.</param>
        /// <param name="marginTop">Margin from top, which is used to position the control with the associated line in the input textbox.</param>
        public LineNumberVM(int number, double marginTop)
        {
            Number = number;
            Margin = new Thickness(0.0, marginTop, 0.0, 0.0);
        }
    }
}
