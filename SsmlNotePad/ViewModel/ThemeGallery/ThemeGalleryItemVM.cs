using System;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.ThemeGallery
{
    public class ThemeGalleryItemVM<T> : DependencyObject
    {
        #region Key Property Members

        public const string PropertyName_Key = "Key";

        private static readonly DependencyPropertyKey KeyPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Key, typeof(string), typeof(ThemeGalleryItemVM<T>),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="Key"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty = KeyPropertyKey.DependencyProperty;

        /// <summary>
        /// Key of resource
        /// </summary>
        public string Key
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(KeyProperty));
                return Dispatcher.Invoke(() => Key);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(KeyPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Key = value);
            }
        }

        #endregion

        #region Resource Property Members

        public const string PropertyName_Resource = "Resource";

        private static readonly DependencyPropertyKey ResourcePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Resource, typeof(T), typeof(ThemeGalleryItemVM<T>),
                new PropertyMetadata(default(T)));

        /// <summary>
        /// Identifies the <see cref="Resource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResourceProperty = ResourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Associated resource.
        /// </summary>
        public T Resource
        {
            get
            {
                if (CheckAccess())
                    return (T)(GetValue(ResourceProperty));
                return Dispatcher.Invoke(() => Resource);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ResourcePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Resource = value);
            }
        }

        #endregion

        public ThemeGalleryItemVM(string key, T resource)
        {
            Key = key;
            Resource = resource;
        }
    }
}