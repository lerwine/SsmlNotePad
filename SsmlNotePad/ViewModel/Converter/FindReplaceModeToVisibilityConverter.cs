using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts find/replace display mode enumerated value to a <seealso cref="Visibility"/> value.
    /// </summary>
    [ValueConversion(typeof(FindReplaceDisplayMode), typeof(Visibility))]
    public class FindReplaceModeToVisibilityConverter : ToValueConverterBase<FindReplaceDisplayMode, Visibility>
    {
        #region None Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="None"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_None = "None";

        /// <summary>
        /// Identifies the <see cref="None"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoneProperty = DependencyProperty.Register(DependencyPropertyName_None, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when the find/replace window should not be shown.
        /// </summary>
        public Visibility? None
        {
            get { return (Visibility?)(GetValue(NoneProperty)); }
            set { SetValue(NoneProperty, value); }
        }
        
        #endregion

        #region Find Property Members

        /// <summary>
        /// Defines the name for the <see cref="Find"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Find = "Find";

        /// <summary>
        /// Identifies the <see cref="Find"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindProperty = DependencyProperty.Register(DependencyPropertyName_Find, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when entering the search terms for a &quot;Find&quot; command.
        /// </summary>
        public Visibility? Find
        {
            get { return (Visibility?)(GetValue(FindProperty)); }
            set { SetValue(FindProperty, value); }
        }
        
        #endregion

        #region FindNext Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="FindNext"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_FindNext = "FindNext";

        /// <summary>
        /// Identifies the <see cref="FindNext"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindNextProperty = DependencyProperty.Register(DependencyPropertyName_FindNext, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when searching for the next match of a &quot;Find&quot; command.
        /// </summary>
        public Visibility? FindNext
        {
            get { return (Visibility?)(GetValue(FindNextProperty)); }
            set { SetValue(FindNextProperty, value); }
        }
        
        #endregion

        #region Replace Property Members

        /// <summary>
        /// Defines the name for the <see cref="Replace"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Replace = "Replace";

        /// <summary>
        /// Identifies the <see cref="Replace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceProperty = DependencyProperty.Register(DependencyPropertyName_Replace, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when entering the search and replace terms for a &quot;Replace&quot; command.
        /// </summary>
        public Visibility? Replace
        {
            get { return (Visibility?)(GetValue(ReplaceProperty)); }
            set { SetValue(ReplaceProperty, value); }
        }
        
        #endregion

        #region ReplaceNext Property Members

        /// <summary>
        /// Defines the name for the <see cref="ReplaceNext"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ReplaceNext = "ReplaceNext";

        /// <summary>
        /// Identifies the <see cref="ReplaceNext"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceNextProperty = DependencyProperty.Register(DependencyPropertyName_ReplaceNext, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when searching for the next match of a &quot;Replace&quot; command.
        /// </summary>
        public Visibility? ReplaceNext
        {
            get { return (Visibility?)(GetValue(ReplaceNextProperty)); }
            set { SetValue(ReplaceNextProperty, value); }
        }
        
        #endregion

        #region FindNotFound Property Members

        /// <summary>
        /// Defines the name for the <see cref="FindNotFound"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_FindNotFound = "FindNotFound";

        /// <summary>
        /// Identifies the <see cref="FindNotFound"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FindNotFoundProperty = DependencyProperty.Register(DependencyPropertyName_FindNotFound, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when indicating that a &quot;Find&quot; command found no matches.
        /// </summary>
        public Visibility? FindNotFound
        {
            get { return (Visibility?)(GetValue(FindNotFoundProperty)); }
            set { SetValue(FindNotFoundProperty, value); }
        }
        
        #endregion

        #region ReplaceNotFound Property Members

        /// <summary>
        /// Defines the name for the <see cref="ReplaceNotFound"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ReplaceNotFound = "ReplaceNotFound";

        /// <summary>
        /// Identifies the <see cref="ReplaceNotFound"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReplaceNotFoundProperty = DependencyProperty.Register(DependencyPropertyName_ReplaceNotFound, typeof(Visibility?), typeof(FindReplaceModeToVisibilityConverter),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// <seealso cref="Visibility"/> to use when indicating that a &quot;Replace&quot; command found no matches.
        /// </summary>
        public Visibility? ReplaceNotFound
        {
            get { return (Visibility?)(GetValue(ReplaceNotFoundProperty)); }
            set { SetValue(ReplaceNotFoundProperty, value); }
        }

        #endregion

        /// <summary>
        /// Converts a <seealso cref="FindReplaceDisplayMode"/> value to a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="FindReplaceDisplayMode"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="FindReplaceDisplayMode"/>value converted to a <seealso cref="Visibility"/> or null value.</returns>
        public override Visibility? Convert(FindReplaceDisplayMode value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case FindReplaceDisplayMode.Find:
                    return Find;
                case FindReplaceDisplayMode.FindNext:
                    return FindNext;
                case FindReplaceDisplayMode.Replace:
                    return Replace;
                case FindReplaceDisplayMode.ReplaceNext:
                    return ReplaceNext;
                case FindReplaceDisplayMode.FindNotFound:
                    return FindNotFound;
                case FindReplaceDisplayMode.ReplaceNotFound:
                    return ReplaceNotFound;
            }

            return None;
        }
    }
}
