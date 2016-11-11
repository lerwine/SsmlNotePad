using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class KeyValuePairVM<TKey, TValue> : DependencyObject
    {
        #region Key Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Key"/> dependency property.
        /// </summary>
        public const string PropertyName_Key = "Key";

        private static readonly DependencyPropertyKey KeyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Key, typeof(TKey), typeof(KeyValuePairVM<TKey, TValue>),
            new PropertyMetadata(default(TKey)));

        /// <summary>
        /// Identifies the <see cref="Key"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = KeyPropertyKey.DependencyProperty;

        /// <summary>
        /// Key assosiated with <see cref="Value"/>.
        /// </summary>
        public TKey Key
        {
            get { return (TKey)(GetValue(KeyProperty)); }
            private set { SetValue(KeyPropertyKey, value); }
        }

        #endregion

        #region Value Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Value"/> dependency property.
        /// </summary>
        public const string PropertyName_Value = "Value";

        private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Value, typeof(TValue), typeof(KeyValuePairVM<TKey, TValue>),
            new PropertyMetadata(default(TValue)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

        /// <summary>
        /// Value associated with <see cref="Key"/>.
        /// </summary>
        public TValue Value
        {
            get { return (TValue)(GetValue(ValueProperty)); }
            private set { SetValue(ValuePropertyKey, value); }
        }

        #endregion
        
        public KeyValuePairVM(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePairVM(KeyValuePair<TKey, TValue> keyValuePair) : this(keyValuePair.Key, keyValuePair.Value) { }

        public static IEnumerable<KeyValuePairVM<TKey, TValue>> FromEnumerable(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            if (enumerable == null)
                return new KeyValuePairVM<TKey, TValue>[0];

            return enumerable.Select(e => new KeyValuePairVM<TKey, TValue>(e));
        }

        public static IEnumerable<KeyValuePairVM<TKey, TValue>> FromDictionary(IDictionary<TKey, TValue> dictionary) { return FromEnumerable(dictionary); }
    }
}