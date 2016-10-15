using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.ThemeGallery
{
    public class ThemeGalleryVM : DependencyObject
    {
        #region Colors Property Members

        public const string PropertyName_Colors = "Colors";

        private static readonly DependencyPropertyKey ColorsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Colors, typeof(ObservableCollection<ThemeGalleryItemVM<Brush>>), typeof(ThemeGalleryVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Colors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorsProperty = ColorsPropertyKey.DependencyProperty;

        /// <summary>
        /// Colors
        /// </summary>
        public ObservableCollection<ThemeGalleryItemVM<Brush>> Colors
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<ThemeGalleryItemVM<Brush>>)(GetValue(ColorsProperty));
                return Dispatcher.Invoke(() => Colors);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ColorsPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Colors = value);
            }
        }

        #endregion

        #region Brushes Property Members

        public const string PropertyName_Brushes = "Brushes";

        private static readonly DependencyPropertyKey BrushesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Brushes, typeof(ObservableCollection<ThemeGalleryItemVM<Brush>>), typeof(ThemeGalleryVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Brushes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrushesProperty = BrushesPropertyKey.DependencyProperty;

        /// <summary>
        /// Brushes
        /// </summary>
        public ObservableCollection<ThemeGalleryItemVM<Brush>> Brushes
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<ThemeGalleryItemVM<Brush>>)(GetValue(BrushesProperty));
                return Dispatcher.Invoke(() => Brushes);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BrushesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Brushes = value);
            }
        }

        #endregion

        #region Geometries Property Members

        public const string PropertyName_Geometries = "Geometries";

        private static readonly DependencyPropertyKey GeometriesPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Geometries, typeof(ObservableCollection<ThemeGalleryItemVM<StreamGeometry>>), typeof(ThemeGalleryVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Geometries"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometriesProperty = GeometriesPropertyKey.DependencyProperty;

        /// <summary>
        /// StreamGeometry resources.
        /// </summary>
        public ObservableCollection<ThemeGalleryItemVM<StreamGeometry>> Geometries
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<ThemeGalleryItemVM<StreamGeometry>>)(GetValue(GeometriesProperty));
                return Dispatcher.Invoke(() => Geometries);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(GeometriesPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Geometries = value);
            }
        }

        #endregion

        public ThemeGalleryVM()
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/SsmlNotePad;component/Themes/ColorDefinitions.xaml", UriKind.RelativeOrAbsolute);
            Colors = new ObservableCollection<ThemeGalleryItemVM<Brush>>(GetAllResourcesOfType<Color, Brush>(resourceDictionary, c => new SolidColorBrush(c)).OrderBy(i => i.Key));
            Brushes = new ObservableCollection<ThemeGalleryItemVM<Brush>>(GetAllResourcesOfType<Brush>(resourceDictionary).OrderBy(i => i.Key));
            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/SsmlNotePad;component/Themes/Icons.xaml", UriKind.RelativeOrAbsolute);
            Geometries = new ObservableCollection<ThemeGalleryItemVM<StreamGeometry>>(GetAllResourcesOfType<StreamGeometry>(resourceDictionary).OrderBy(i => i.Key));
        }

        private IEnumerable<ThemeGalleryItemVM<TTarget>> GetAllResourcesOfType<TSource, TTarget>(ResourceDictionary dictionary, Func<TSource, TTarget> convert)
        {
            IEnumerable<ThemeGalleryItemVM<TTarget>> result = dictionary.Keys.OfType<string>().Select(key => new { Key = key, Resource = dictionary[key] })
                .Where(a => a.Resource != null && a.Resource is TSource).Select(a => new ThemeGalleryItemVM<TTarget>(a.Key, convert((TSource)(a.Resource))));
            if (dictionary.MergedDictionaries.Count > 0)
                foreach (ResourceDictionary d in dictionary.MergedDictionaries)
                    result = result.Concat(GetAllResourcesOfType<TSource, TTarget>(d, convert));
            return result;
        }

        private IEnumerable<ThemeGalleryItemVM<T>> GetAllResourcesOfType<T>(ResourceDictionary dictionary)
        {
            IEnumerable<ThemeGalleryItemVM<T>> result = dictionary.Keys.OfType<string>().Select(key => new { Key = key, Resource = dictionary[key] })
                .Where(a => a.Resource != null && a.Resource is T).Select(a => new ThemeGalleryItemVM<T>(a.Key, (T)(a.Resource)));
            if (dictionary.MergedDictionaries.Count > 0)
                foreach (ResourceDictionary d in dictionary.MergedDictionaries)
                    result = result.Concat(GetAllResourcesOfType<T>(d));
            return result;
        }
    }
}
