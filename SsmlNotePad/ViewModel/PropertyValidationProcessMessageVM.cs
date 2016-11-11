using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Erwine.Leonard.T.SsmlNotePad.Model;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class PropertyValidationProcessMessageVM : ProcessMessageVM, Reserved.INamedProcessMessageViewModel<ProcessMessageVM, ReadOnlyObservableCollection<ProcessMessageVM>>
    {
        #region PropertyName Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public const string PropertyName_PropertyName = "PropertyName";

        private static readonly DependencyPropertyKey PropertyNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PropertyName, typeof(string), typeof(PropertyValidationProcessMessageVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = PropertyNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get { return GetValue(PropertyNameProperty) as string; }
            private set { SetValue(PropertyNamePropertyKey, value); }
        }

        string INamedProcessMessage.Name { get { return PropertyName; } }

        #endregion
    }
}
