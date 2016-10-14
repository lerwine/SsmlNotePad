using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class AppSettingsVM : ValidatingViewModel
    {
        public AppSettingsVM()
        {
            try { BaseUriPath = Common.FileUtility.ResolveFileUri(Properties.Settings.Default.RelativeXmlBaseURI); }
            catch { BaseUriPath = Common.FileUtility.ResolveFileUri(DefaultValue_BaseUriPath); }

            try { BlankSsmlFileName = Common.FileUtility.ResolveFileUri(Properties.Settings.Default.BlankSsmlFileName, BaseUriPath); }
            catch { BlankSsmlFileName = Common.FileUtility.ResolveFileUri(DefaultValue_BlankSsmlFileName, BaseUriPath); }

            try { SsmlSchemaFileName = Common.FileUtility.ResolveFileUri(Properties.Settings.Default.SsmlSchemaFileName, BaseUriPath); }
            catch { SsmlSchemaFileName = Common.FileUtility.ResolveFileUri(DefaultValue_SsmlSchemaFileName, BaseUriPath); }

            try { SsmlSchemaCoreFileName = Common.FileUtility.ResolveFileUri(Properties.Settings.Default.SsmlSchemaCoreFileName, BaseUriPath); }
            catch { SsmlSchemaCoreFileName = Common.FileUtility.ResolveFileUri(DefaultValue_SsmlSchemaCoreFileName, BaseUriPath); }

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SsmlFileExtension))
                SsmlFileExtension = Common.FileUtility.EnsureValidExtension(Properties.Settings.Default.SsmlFileExtension);

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PlsFileExtension))
                PlsFileExtension = Common.FileUtility.EnsureValidExtension(Properties.Settings.Default.SsmlFileExtension);

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SsmlFileTypeDescriptionLong))
                SsmlFileTypeDescriptionLong = Properties.Settings.Default.SsmlFileTypeDescriptionLong.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PlsFileTypeDescriptionLong))
                PlsFileTypeDescriptionLong = Properties.Settings.Default.PlsFileTypeDescriptionLong.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SsmlFileTypeDescriptionShort))
                SsmlFileTypeDescriptionShort = Properties.Settings.Default.SsmlFileTypeDescriptionShort.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PlsFileTypeDescriptionShort))
                PlsFileTypeDescriptionShort = Properties.Settings.Default.PlsFileTypeDescriptionShort.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PlsFileTypeDescriptionShort))
                PlsFileTypeDescriptionShort = Properties.Settings.Default.PlsFileTypeDescriptionShort.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastSsmlFilePath))
                LastSsmlFilePath = Properties.Settings.Default.LastSsmlFilePath.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastSavedWavPath))
                LastSavedWavPath = Properties.Settings.Default.LastSavedWavPath.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastSavedWavPath))
                LastSavedWavPath = Properties.Settings.Default.LastSavedWavPath.Trim();
            
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastAudioPath))
                LastAudioPath = Properties.Settings.Default.LastAudioPath.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastPlsFilePath))
                LastPlsFilePath = Properties.Settings.Default.LastPlsFilePath.Trim();

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.LastBrowsedSubdirectory))
                LastBrowsedSubdirectory = Properties.Settings.Default.LastBrowsedSubdirectory.Trim();

            DefaultSpeechRate = Properties.Settings.Default.DefaultSpeechRate;
            DefaultSpeechVolume = Properties.Settings.Default.DefaultSpeechVolume;

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultVoiceName))
                DefaultVoiceName = Properties.Settings.Default.DefaultVoiceName.Trim();
        }

        #region BaseUriPath Property Members

        public const string PropertyName_BaseUriPath = "BaseUriPath";
        public const string DefaultValue_BaseUriPath = "Resources";

        private static readonly DependencyPropertyKey BaseUriPathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BaseUriPath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="BaseUriPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseUriPathProperty = BaseUriPathPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string BaseUriPath
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(BaseUriPathProperty));
                return Dispatcher.Invoke(() => BaseUriPath);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BaseUriPathPropertyKey, value);
                else
                    Dispatcher.Invoke(() => BaseUriPath = value);
            }
        }

        #endregion

        #region BlankSsmlFileName Property Members

        public const string PropertyName_BlankSsmlFileName = "BlankSsmlFileName";
        public const string DefaultValue_BlankSsmlFileName = "BlankSsmlDocument.xml";

        private static readonly DependencyPropertyKey BlankSsmlFileNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BlankSsmlFileName, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_BlankSsmlFileName));

        /// <summary>
        /// Identifies the <seealso cref="BlankSsmlFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlankSsmlFileNameProperty = BlankSsmlFileNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string BlankSsmlFileName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(BlankSsmlFileNameProperty));
                return Dispatcher.Invoke(() => BlankSsmlFileName);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BlankSsmlFileNamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => BlankSsmlFileName = value);
            }
        }

        #endregion

        #region SsmlSchemaFileName Property Members

        public const string PropertyName_SsmlSchemaFileName = "SsmlSchemaFileName";
        public const string DefaultValue_SsmlSchemaFileName = "WindowsPhoneSynthesis.xsd";

        private static readonly DependencyPropertyKey SsmlSchemaFileNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlSchemaFileName, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_SsmlSchemaFileName));

        /// <summary>
        /// Identifies the <seealso cref="SsmlSchemaFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlSchemaFileNameProperty = SsmlSchemaFileNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string SsmlSchemaFileName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SsmlSchemaFileNameProperty));
                return Dispatcher.Invoke(() => SsmlSchemaFileName);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlSchemaFileNamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlSchemaFileName = value);
            }
        }

        #endregion

        #region SsmlSchemaCoreFileName Property Members

        public const string PropertyName_SsmlSchemaCoreFileName = "SsmlSchemaCoreFileName";
        public const string DefaultValue_SsmlSchemaCoreFileName = "WindowsPhoneSynthesis-core.xsd";

        private static readonly DependencyPropertyKey SsmlSchemaCoreFileNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlSchemaCoreFileName, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_SsmlSchemaCoreFileName));

        /// <summary>
        /// Identifies the <seealso cref="SsmlSchemaCoreFileName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlSchemaCoreFileNameProperty = SsmlSchemaCoreFileNamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string SsmlSchemaCoreFileName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SsmlSchemaCoreFileNameProperty));
                return Dispatcher.Invoke(() => SsmlSchemaCoreFileName);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlSchemaCoreFileNamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlSchemaCoreFileName = value);
            }
        }

        #endregion

        #region SsmlFileExtension Property Members

        public const string PropertyName_SsmlFileExtension = "SsmlFileExtension";
        public const string DefaultValue_SsmlFileExtension = ".ssml";

        private static readonly DependencyPropertyKey SsmlFileExtensionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileExtension, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_SsmlFileExtension));

        /// <summary>
        /// Identifies the <seealso cref="SsmlFileExtension"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileExtensionProperty = SsmlFileExtensionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string SsmlFileExtension
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SsmlFileExtensionProperty));
                return Dispatcher.Invoke(() => SsmlFileExtension);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlFileExtensionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlFileExtension = value);
            }
        }

        #endregion

        #region PlsFileExtension Property Members

        public const string PropertyName_PlsFileExtension = "PlsFileExtension";
        public const string DefaultValue_PlsFileExtension = ".pls";

        private static readonly DependencyPropertyKey PlsFileExtensionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileExtension, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_PlsFileExtension));

        /// <summary>
        /// Identifies the <seealso cref="PlsFileExtension"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileExtensionProperty = PlsFileExtensionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string PlsFileExtension
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PlsFileExtensionProperty));
                return Dispatcher.Invoke(() => PlsFileExtension);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PlsFileExtensionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => PlsFileExtension = value);
            }
        }

        #endregion

        #region SsmlFileTypeDescriptionLong Property Members

        public const string PropertyName_SsmlFileTypeDescriptionLong = "SsmlFileTypeDescriptionLong";
        public const string DefaultValue_SsmlFileTypeDescriptionLong = "Speech Synthesis Markup Language Source";

        private static readonly DependencyPropertyKey SsmlFileTypeDescriptionLongPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileTypeDescriptionLong, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_SsmlFileTypeDescriptionLong));

        /// <summary>
        /// Identifies the <seealso cref="SsmlFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileTypeDescriptionLongProperty = SsmlFileTypeDescriptionLongPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string SsmlFileTypeDescriptionLong
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SsmlFileTypeDescriptionLongProperty));
                return Dispatcher.Invoke(() => SsmlFileTypeDescriptionLong);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlFileTypeDescriptionLongPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlFileTypeDescriptionLong = value);
            }
        }

        #endregion

        #region PlsFileTypeDescriptionLong Property Members

        public const string PropertyName_PlsFileTypeDescriptionLong = "PlsFileTypeDescriptionLong";
        public const string DefaultValue_PlsFileTypeDescriptionLong = "Pronunciation Lexicon Source";

        private static readonly DependencyPropertyKey PlsFileTypeDescriptionLongPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileTypeDescriptionLong, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_PlsFileTypeDescriptionLong));

        /// <summary>
        /// Identifies the <seealso cref="PlsFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileTypeDescriptionLongProperty = PlsFileTypeDescriptionLongPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string PlsFileTypeDescriptionLong
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PlsFileTypeDescriptionLongProperty));
                return Dispatcher.Invoke(() => PlsFileTypeDescriptionLong);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PlsFileTypeDescriptionLongPropertyKey, value);
                else
                    Dispatcher.Invoke(() => PlsFileTypeDescriptionLong = value);
            }
        }

        #endregion
        
        #region SsmlFileTypeDescriptionShort Property Members

        public const string PropertyName_SsmlFileTypeDescriptionShort = "SsmlFileTypeDescriptionShort";
        public const string DefaultValue_SsmlFileTypeDescriptionShort = "Speech Synthesis Markup";

        private static readonly DependencyPropertyKey SsmlFileTypeDescriptionShortPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileTypeDescriptionShort, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_SsmlFileTypeDescriptionShort));

        /// <summary>
        /// Identifies the <seealso cref="SsmlFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileTypeDescriptionShortProperty = SsmlFileTypeDescriptionShortPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string SsmlFileTypeDescriptionShort
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SsmlFileTypeDescriptionShortProperty));
                return Dispatcher.Invoke(() => SsmlFileTypeDescriptionShort);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SsmlFileTypeDescriptionShortPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SsmlFileTypeDescriptionShort = value);
            }
        }

        #endregion
        
        #region PlsFileTypeDescriptionShort Property Members

        public const string PropertyName_PlsFileTypeDescriptionShort = "PlsFileTypeDescriptionShort";
        public const string DefaultValue_PlsFileTypeDescriptionShort = "Pronunciation Lexicon";

        private static readonly DependencyPropertyKey PlsFileTypeDescriptionShortPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileTypeDescriptionShort, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_PlsFileTypeDescriptionShort));

        /// <summary>
        /// Identifies the <seealso cref="PlsFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileTypeDescriptionShortProperty = PlsFileTypeDescriptionShortPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string PlsFileTypeDescriptionShort
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PlsFileTypeDescriptionShortProperty));
                return Dispatcher.Invoke(() => PlsFileTypeDescriptionShort);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PlsFileTypeDescriptionShortPropertyKey, value);
                else
                    Dispatcher.Invoke(() => PlsFileTypeDescriptionShort = value);
            }
        }

        #endregion
        
        #region LastSsmlFilePath Property Members

        public const string DependencyPropertyName_LastSsmlFilePath = "LastSsmlFilePath";

        /// <summary>
        /// Identifies the <seealso cref="LastSsmlFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastSsmlFilePathProperty = DependencyProperty.Register(DependencyPropertyName_LastSsmlFilePath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).LastSsmlFilePath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).LastSsmlFilePath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastSsmlFilePath_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string LastSsmlFilePath
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LastSsmlFilePathProperty));
                return Dispatcher.Invoke(() => LastSsmlFilePath);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LastSsmlFilePathProperty, value);
                else
                    Dispatcher.Invoke(() => LastSsmlFilePath = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LastSsmlFilePath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="LastSsmlFilePath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="LastSsmlFilePath"/> property was changed.</param>
        protected virtual void LastSsmlFilePath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastSsmlFilePath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="LastSsmlFilePath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastSsmlFilePath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Common.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastSavedWavPath Property Members

        public const string DependencyPropertyName_LastSavedWavPath = "LastSavedWavPath";

        /// <summary>
        /// Identifies the <seealso cref="LastSavedWavPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastSavedWavPathProperty = DependencyProperty.Register(DependencyPropertyName_LastSavedWavPath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).LastSavedWavPath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).LastSavedWavPath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastSavedWavPath_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string LastSavedWavPath
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LastSavedWavPathProperty));
                return Dispatcher.Invoke(() => LastSavedWavPath);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LastSavedWavPathProperty, value);
                else
                    Dispatcher.Invoke(() => LastSavedWavPath = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LastSavedWavPath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="LastSavedWavPath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="LastSavedWavPath"/> property was changed.</param>
        protected virtual void LastSavedWavPath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastSavedWavPath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="LastSavedWavPath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastSavedWavPath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Common.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastAudioPath Property Members

        public const string DependencyPropertyName_LastAudioPath = "LastAudioPath";

        /// <summary>
        /// Identifies the <seealso cref="LastAudioPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastAudioPathProperty = DependencyProperty.Register(DependencyPropertyName_LastAudioPath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).LastAudioPath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).LastAudioPath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastAudioPath_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string LastAudioPath
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LastAudioPathProperty));
                return Dispatcher.Invoke(() => LastAudioPath);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LastAudioPathProperty, value);
                else
                    Dispatcher.Invoke(() => LastAudioPath = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LastAudioPath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="LastAudioPath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="LastAudioPath"/> property was changed.</param>
        protected virtual void LastAudioPath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastAudioPath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="LastAudioPath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastAudioPath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Common.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastPlsFilePath Property Members

        public const string DependencyPropertyName_LastPlsFilePath = "LastPlsFilePath";

        /// <summary>
        /// Identifies the <seealso cref="LastPlsFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastPlsFilePathProperty = DependencyProperty.Register(DependencyPropertyName_LastPlsFilePath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).LastPlsFilePath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).LastPlsFilePath_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastPlsFilePath_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string LastPlsFilePath
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LastPlsFilePathProperty));
                return Dispatcher.Invoke(() => LastPlsFilePath);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LastPlsFilePathProperty, value);
                else
                    Dispatcher.Invoke(() => LastPlsFilePath = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LastPlsFilePath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="LastPlsFilePath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="LastPlsFilePath"/> property was changed.</param>
        protected virtual void LastPlsFilePath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastPlsFilePath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <seealso cref="LastPlsFilePath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastPlsFilePath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Common.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastBrowsedSubdirectory Property Members

        public const string DependencyPropertyName_LastBrowsedSubdirectory = "LastBrowsedSubdirectory";

        /// <summary>
        /// Identifies the <seealso cref="LastBrowsedSubdirectory"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastBrowsedSubdirectoryProperty = DependencyProperty.Register(DependencyPropertyName_LastBrowsedSubdirectory, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).LastBrowsedSubdirectory_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).LastBrowsedSubdirectory_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastBrowsedSubdirectory_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string LastBrowsedSubdirectory
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LastBrowsedSubdirectoryProperty));
                return Dispatcher.Invoke(() => LastBrowsedSubdirectory);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LastBrowsedSubdirectoryProperty, value);
                else
                    Dispatcher.Invoke(() => LastBrowsedSubdirectory = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="LastBrowsedSubdirectory"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="LastBrowsedSubdirectory"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="LastBrowsedSubdirectory"/> property was changed.</param>
        protected virtual void LastBrowsedSubdirectory_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastBrowsedSubdirectory = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="LastBrowsedSubdirectory"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastBrowsedSubdirectory_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            if (Common.FileUtility.GetLocalPath(path, out path) && File.Exists(path))
                return Path.GetDirectoryName(path);
            return path;
        }

        #endregion

        #region DefaultSpeechRate Property Members

        public const string DependencyPropertyName_DefaultSpeechRate = "DefaultSpeechRate";

        /// <summary>
        /// Identifies the <seealso cref="DefaultSpeechRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultSpeechRateProperty = DependencyProperty.Register(DependencyPropertyName_DefaultSpeechRate, typeof(int), typeof(AppSettingsVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).DefaultSpeechRate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).DefaultSpeechRate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).DefaultSpeechRate_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int DefaultSpeechRate
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(DefaultSpeechRateProperty));
                return Dispatcher.Invoke(() => DefaultSpeechRate);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DefaultSpeechRateProperty, value);
                else
                    Dispatcher.Invoke(() => DefaultSpeechRate = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DefaultSpeechRate"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="DefaultSpeechRate"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="DefaultSpeechRate"/> property was changed.</param>
        protected virtual void DefaultSpeechRate_PropertyChanged(int oldValue, int newValue)
        {
            Properties.Settings.Default.DefaultSpeechRate = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DefaultSpeechRate"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int DefaultSpeechRate_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < -10) ? -10 : ((i > 10) ? 10 : i);
        }

        #endregion

        #region DefaultSpeechVolume Property Members

        public const string DependencyPropertyName_DefaultSpeechVolume = "DefaultSpeechVolume";

        /// <summary>
        /// Identifies the <seealso cref="DefaultSpeechVolume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultSpeechVolumeProperty = DependencyProperty.Register(DependencyPropertyName_DefaultSpeechVolume, typeof(int), typeof(AppSettingsVM),
                new PropertyMetadata(100,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).DefaultSpeechVolume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).DefaultSpeechVolume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).DefaultSpeechVolume_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public int DefaultSpeechVolume
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(DefaultSpeechVolumeProperty));
                return Dispatcher.Invoke(() => DefaultSpeechVolume);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DefaultSpeechVolumeProperty, value);
                else
                    Dispatcher.Invoke(() => DefaultSpeechVolume = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DefaultSpeechVolume"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="DefaultSpeechVolume"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="DefaultSpeechVolume"/> property was changed.</param>
        protected virtual void DefaultSpeechVolume_PropertyChanged(int oldValue, int newValue)
        {
            Properties.Settings.Default.DefaultSpeechVolume = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DefaultSpeechVolume"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int DefaultSpeechVolume_CoerceValue(object baseValue)
        {
            int i = (int)baseValue;
            return (i < 0) ? 0 : ((i > 100) ? 100 : i);
        }

        #endregion

        #region DefaultVoiceName Property Members

        public const string DependencyPropertyName_DefaultVoiceName = "DefaultVoiceName";

        /// <summary>
        /// Identifies the <seealso cref="DefaultVoiceName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultVoiceNameProperty = DependencyProperty.Register(DependencyPropertyName_DefaultVoiceName, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as AppSettingsVM).DefaultVoiceName_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as AppSettingsVM).DefaultVoiceName_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as AppSettingsVM).DefaultVoiceName_CoerceValue(baseValue)));

        /// <summary>
        /// 
        /// </summary>
        public string DefaultVoiceName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DefaultVoiceNameProperty));
                return Dispatcher.Invoke(() => DefaultVoiceName);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DefaultVoiceNameProperty, value);
                else
                    Dispatcher.Invoke(() => DefaultVoiceName = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DefaultVoiceName"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="DefaultVoiceName"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="DefaultVoiceName"/> property was changed.</param>
        protected virtual void DefaultVoiceName_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.DefaultVoiceName = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DefaultVoiceName"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DefaultVoiceName_CoerceValue(object baseValue) { return (baseValue as string) ?? ""; }

        #endregion

        private void UpdateLastBrowedDirectory(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                return;

            try
            {
                if (File.Exists(filePath))
                    LastBrowsedSubdirectory = Path.GetDirectoryName(filePath);
                else if (Directory.Exists(filePath))
                    LastBrowsedSubdirectory = filePath;
                else
                {
                    string path = Path.GetDirectoryName(filePath);
                    if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
                        LastBrowsedSubdirectory = path;
                }
            }
            catch { }
        }

        private Task _saveSettingsTask = null;
        private object _syncRoot = new object();
        private CancellationTokenSource _tokenSource = null;

        private void SavePropertiesAsync()
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null && !_tokenSource.IsCancellationRequested && !_saveSettingsTask.IsCompleted)
                    _tokenSource.Cancel();
                _tokenSource = new CancellationTokenSource();
                CancellationToken token = _tokenSource.Token;
                _saveSettingsTask = Task.Factory.StartNew(o =>
                {
                    CancellationToken t = (CancellationToken)o;
                    if (t.WaitHandle.WaitOne(new TimeSpan(0, 0, 5)))
                        return;

                    lock (_syncRoot)
                    {
                        if (!t.IsCancellationRequested)
                            Properties.Settings.Default.Save();
                    }
                }, token, token);
                _saveSettingsTask.ContinueWith((t, o) =>
                {
                    lock (_syncRoot)
                    {
                        if (_tokenSource != null && ReferenceEquals(o, _tokenSource))
                            _tokenSource = null;
                    }
                    (o as CancellationTokenSource).Dispose();
                }, _tokenSource);
            }
        }

        private void SavePropertiesImmediately()
        {
            lock (_syncRoot)
            {
                if (_tokenSource != null && !_tokenSource.IsCancellationRequested && !_saveSettingsTask.IsCompleted)
                {
                    _tokenSource.Cancel();
                    _tokenSource = null;
                }
                Properties.Settings.Default.Save();
            }
        }

    }
}
