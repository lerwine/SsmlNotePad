using System.ComponentModel;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class PropertyMessageVM : NotificationMessageVM
    {
        #region PropertyName Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string PropertyName_PropertyName = "PropertyName";

        private static readonly DependencyPropertyKey PropertyNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PropertyName, typeof(string), typeof(PropertyMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = PropertyNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Name of property associated with message, or empty string for all properties.
        /// </summary>
        [DefaultValue("")]
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            private set { SetValue(PropertyNamePropertyKey, value); }
        }
        
        #endregion

    }
}