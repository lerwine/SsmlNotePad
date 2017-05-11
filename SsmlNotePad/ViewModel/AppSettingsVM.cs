using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    /// <summary>
    /// View model representing application settings.
    /// </summary>
    public class AppSettingsVM : DependencyObject
    {
        #region Default property settings values

        /// <summary>
        /// Defines the default value for the <see cref="Properties.Settings.RelativeXmlBaseURI"/> app setting.
        /// </summary>
        public const string DefaultValue_RelativeXmlBaseURI = "Resources";

        /// <summary>
        /// Defines the default value for the <see cref="Properties.Settings.BlankSsmlFileName"/> app setting.
        /// </summary>
        public const string DefaultValue_BlankSsmlFileName = "BlankSsmlDocument.xml";

        /// <summary>
        /// Defines the default value for the <see cref="Properties.Settings.SsmlSchemaFileName"/> app setting.
        /// </summary>
        public const string DefaultValue_SsmlSchemaFileName = "WindowsPhoneSynthesis.xsd";

        /// <summary>
        /// Defines the default value for the <see cref="Properties.Settings.SsmlSchemaCoreFileName"/> app setting.
        /// </summary>
        public const string DefaultValue_SsmlSchemaCoreFileName = "WindowsPhoneSynthesis-core.xsd";

        #endregion

        public AppSettingsVM()
        {
            try { BaseUriPath = Model.FileUtility.ResolveFileUri(Properties.Settings.Default.RelativeXmlBaseURI); }
            catch { BaseUriPath = Model.FileUtility.ResolveFileUri(DefaultValue_RelativeXmlBaseURI); }

            try { BlankSsmlFilePath = Model.FileUtility.ResolveFileUri(Properties.Settings.Default.BlankSsmlFileName, BaseUriPath); }
            catch { BlankSsmlFilePath = Model.FileUtility.ResolveFileUri(DefaultValue_BlankSsmlFileName, BaseUriPath); }

            try { SsmlSchemaFilePath = Model.FileUtility.ResolveFileUri(Properties.Settings.Default.SsmlSchemaFileName, BaseUriPath); }
            catch { SsmlSchemaFilePath = Model.FileUtility.ResolveFileUri(DefaultValue_SsmlSchemaFileName, BaseUriPath); }

            try { SsmlSchemaCoreFilePath = Model.FileUtility.ResolveFileUri(Properties.Settings.Default.SsmlSchemaCoreFileName, BaseUriPath); }
            catch { SsmlSchemaCoreFilePath = Model.FileUtility.ResolveFileUri(DefaultValue_SsmlSchemaCoreFileName, BaseUriPath); }

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SsmlFileExtension))
                SsmlFileExtension = Model.FileUtility.EnsureValidExtension(Properties.Settings.Default.SsmlFileExtension);

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.PlsFileExtension))
                PlsFileExtension = Model.FileUtility.EnsureValidExtension(Properties.Settings.Default.PlsFileExtension);

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

        /// <summary>
        /// Defines the name for the <see cref="BaseUriPath"/> dependency property.
        /// </summary>
        public const string PropertyName_BaseUriPath = "BaseUriPath";

        private static readonly DependencyPropertyKey BaseUriPathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BaseUriPath, typeof(string),
            typeof(AppSettingsVM), new PropertyMetadata());

        /// <summary>
        /// Identifies the <see cref="BaseUriPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BaseUriPathProperty = BaseUriPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Path to folder which contains filesystem resources
        /// </summary>
        public string BaseUriPath
        {
            get { return GetValue(BaseUriPathProperty) as string; }
            private set { SetValue(BaseUriPathPropertyKey, value); }
        }

        #endregion

        #region BlankSsmlFilePath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="BlankSsmlFilePath"/> dependency property.
        /// </summary>
        public const string PropertyName_BlankSsmlFilePath = "BlankSsmlFilePath";

        private static readonly DependencyPropertyKey BlankSsmlFilePathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BlankSsmlFilePath, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="BlankSsmlFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BlankSsmlFilePathProperty = BlankSsmlFilePathPropertyKey.DependencyProperty;

        /// <summary>
        /// Path to the file which represents a blank (new) Speech Synthesis Markup document.
        /// </summary>
        public string BlankSsmlFilePath
        {
            get { return GetValue(BlankSsmlFilePathProperty) as string; }
            private set { SetValue(BlankSsmlFilePathPropertyKey, value); }
        }

        #endregion

        #region SsmlSchemaFilePath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SsmlSchemaFilePath"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlSchemaFilePath = "SsmlSchemaFilePath";

        private static readonly DependencyPropertyKey SsmlSchemaFilePathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlSchemaFilePath, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="SsmlSchemaFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlSchemaFilePathProperty = SsmlSchemaFilePathPropertyKey.DependencyProperty;

        /// <summary>
        /// Path to the XML Schema file for validating Speech Synthesis Markup markup.
        /// </summary>
        /// <remarks>If this is a relative path, then it will be relative to path defined by <see cref="BaseUriPath"/>.</remarks>
        public string SsmlSchemaFilePath
        {
            get { return GetValue(SsmlSchemaFilePathProperty) as string; }
            private set { SetValue(SsmlSchemaFilePathPropertyKey, value); }
        }

        #endregion

        #region SsmlSchemaCoreFilePath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SsmlSchemaCoreFilePath"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlSchemaCoreFilePath = "SsmlSchemaCoreFilePath";

        private static readonly DependencyPropertyKey SsmlSchemaCoreFilePathPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlSchemaCoreFilePath, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <see cref="SsmlSchemaCoreFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlSchemaCoreFilePathProperty = SsmlSchemaCoreFilePathPropertyKey.DependencyProperty;

        /// <summary>
        /// Path to SSML schema core file.
        /// </summary>
        public string SsmlSchemaCoreFilePath
        {
            get { return GetValue(SsmlSchemaCoreFilePathProperty) as string; }
            private set { SetValue(SsmlSchemaCoreFilePathPropertyKey, value); }
        }

        #endregion

        #region SsmlFileExtension Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SsmlFileExtension"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlFileExtension = "SsmlFileExtension";

        /// <summary>
        /// Defines the default value for the <see cref="SsmlFileExtension"/> dependency property.
        /// </summary>
        public const string DefaultValue_SsmlFileExtension = ".ssml";

        private static readonly DependencyPropertyKey SsmlFileExtensionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileExtension, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_SsmlFileExtension, null, SsmlFileExtension_CoerceValue));

        /// <summary>
        /// Identifies the <see cref="SsmlFileExtension"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileExtensionProperty = SsmlFileExtensionPropertyKey.DependencyProperty;

        /// <summary>
        /// File extension for Speech Synthesis Markup document files.
        /// </summary>
        public string SsmlFileExtension
        {
            get { return GetValue(SsmlFileExtensionProperty) as string; }
            private set { SetValue(SsmlFileExtensionPropertyKey, value); }
        }

        private static object SsmlFileExtension_CoerceValue(DependencyObject d, object baseValue)
        {
            string ext = baseValue as string;
            if (String.IsNullOrWhiteSpace(ext))
                return DefaultValue_SsmlFileExtension;
            ext = ext.Trim();
            if (ext.StartsWith("."))
                return ext;

            return "." + ext;
        }

        #endregion

        #region PlsFileExtension Property Members

        /// <summary>
        /// Defines the name for the <see cref="PlsFileExtension"/> dependency property.
        /// </summary>
        public const string PropertyName_PlsFileExtension = "PlsFileExtension";

        /// <summary>
        /// Defines the default value for the <see cref="PlsFileExtension"/> dependency property.
        /// </summary>
        public const string DefaultValue_PlsFileExtension = ".pls";

        private static readonly DependencyPropertyKey PlsFileExtensionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileExtension, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_PlsFileExtension, null, PlsFileExtension_CoerceValue));

        /// <summary>
        /// Identifies the <see cref="PlsFileExtension"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileExtensionProperty = PlsFileExtensionPropertyKey.DependencyProperty;

        /// <summary>
        /// Extension for Pronunciation Lexicon Specification files.
        /// </summary>
        public string PlsFileExtension
        {
            get { return GetValue(PlsFileExtensionProperty) as string; }
            private set { SetValue(PlsFileExtensionPropertyKey, value); }
        }

        private static object PlsFileExtension_CoerceValue(DependencyObject d, object baseValue)
        {
            string ext = baseValue as string;
            if (String.IsNullOrWhiteSpace(ext))
                return DefaultValue_PlsFileExtension;
            ext = ext.Trim();
            if (ext.StartsWith("."))
                return ext;

            return "." + ext;
        }

        #endregion

        #region SsmlFileTypeDescriptionLong Property Members

        /// <summary>
        /// Defines the name for the <see cref="SsmlFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlFileTypeDescriptionLong = "SsmlFileTypeDescriptionLong";

        /// <summary>
        /// Defines the default value for the <see cref="SsmlFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public const string DefaultValue_SsmlFileTypeDescriptionLong = "Speech Synthesis Markup Language Source";

        private static readonly DependencyPropertyKey SsmlFileTypeDescriptionLongPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileTypeDescriptionLong, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_SsmlFileTypeDescriptionLong));

        /// <summary>
        /// Identifies the <see cref="SsmlFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileTypeDescriptionLongProperty = SsmlFileTypeDescriptionLongPropertyKey.DependencyProperty;

        /// <summary>
        /// Long description text for Speech Synthesis Markup files.
        /// </summary>
        public string SsmlFileTypeDescriptionLong
        {
            get { return GetValue(SsmlFileTypeDescriptionLongProperty) as string; }
            private set { SetValue(SsmlFileTypeDescriptionLongPropertyKey, value); }
        }

        #endregion

        #region PlsFileTypeDescriptionLong Property Members

        /// <summary>
        /// Defines the name for the <see cref="PlsFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public const string PropertyName_PlsFileTypeDescriptionLong = "PlsFileTypeDescriptionLong";

        /// <summary>
        /// Defines the default value for the <see cref="PlsFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public const string DefaultValue_PlsFileTypeDescriptionLong = "Pronunciation Lexicon Specification File";

        private static readonly DependencyPropertyKey PlsFileTypeDescriptionLongPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileTypeDescriptionLong, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_PlsFileTypeDescriptionLong));

        /// <summary>
        /// Identifies the <see cref="PlsFileTypeDescriptionLong"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileTypeDescriptionLongProperty = PlsFileTypeDescriptionLongPropertyKey.DependencyProperty;

        /// <summary>
        /// Long description for Pronunciation Lexicon Specification files.
        /// </summary>
        public string PlsFileTypeDescriptionLong
        {
            get { return GetValue(PlsFileTypeDescriptionLongProperty) as string; }
            private set { SetValue(PlsFileTypeDescriptionLongPropertyKey, value); }
        }

        #endregion

        #region SsmlFileTypeDescriptionShort Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SsmlFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public const string PropertyName_SsmlFileTypeDescriptionShort = "SsmlFileTypeDescriptionShort";

        /// <summary>
        /// Defines the default value for the <see cref="SsmlFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public const string DefaultValue_SsmlFileTypeDescriptionShort = "Speech Synthesis Markup";

        private static readonly DependencyPropertyKey SsmlFileTypeDescriptionShortPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SsmlFileTypeDescriptionShort, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_SsmlFileTypeDescriptionShort));

        /// <summary>
        /// Identifies the <see cref="SsmlFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SsmlFileTypeDescriptionShortProperty = SsmlFileTypeDescriptionShortPropertyKey.DependencyProperty;

        /// <summary>
        /// Short description for Speech Synthesis Markup files.
        /// </summary>
        public string SsmlFileTypeDescriptionShort
        {
            get { return GetValue(SsmlFileTypeDescriptionShortProperty) as string; }
            private set { SetValue(SsmlFileTypeDescriptionShortPropertyKey, value); }
        }

        #endregion

        #region PlsFileTypeDescriptionShort Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PlsFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public const string PropertyName_PlsFileTypeDescriptionShort = "PlsFileTypeDescriptionShort";

        /// <summary>
        /// Defines the name for the <see cref="PlsFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public const string DefaultValue_PlsFileTypeDescriptionShort = "Pronunciation Lexicon Specification";

        private static readonly DependencyPropertyKey PlsFileTypeDescriptionShortPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PlsFileTypeDescriptionShort, typeof(string), typeof(AppSettingsVM),
            new PropertyMetadata(DefaultValue_PlsFileTypeDescriptionShort));

        /// <summary>
        /// Identifies the <see cref="PlsFileTypeDescriptionShort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlsFileTypeDescriptionShortProperty = PlsFileTypeDescriptionShortPropertyKey.DependencyProperty;

        /// <summary>
        /// Short description for Pronunciation Lexicon Specification files.
        /// </summary>
        public string PlsFileTypeDescriptionShort
        {
            get { return GetValue(PlsFileTypeDescriptionShortProperty) as string; }
            private set { SetValue(PlsFileTypeDescriptionShortPropertyKey, value); }
        }

        #endregion

        #region LastSsmlFilePath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastSsmlFilePath"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LastSsmlFilePath = "LastSsmlFilePath";

        /// <summary>
        /// Identifies the <see cref="LastSsmlFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastSsmlFilePathProperty = DependencyProperty.Register(DependencyPropertyName_LastSsmlFilePath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).LastSsmlFilePath_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastSsmlFilePath_CoerceValue(baseValue)));

        /// <summary>
        /// Path to last saved or loaded Speech Synthesis Markup file
        /// </summary>
        public string LastSsmlFilePath
        {
            get { return GetValue(LastSsmlFilePathProperty) as string; }
            set { SetValue(LastSsmlFilePathProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LastSsmlFilePath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="LastSsmlFilePath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="LastSsmlFilePath"/> property was changed.</param>
        protected virtual void LastSsmlFilePath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastSsmlFilePath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <see cref="LastSsmlFilePath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastSsmlFilePath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Model.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastSavedWavPath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastSavedWavPath"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LastSavedWavPath = "LastSavedWavPath";

        /// <summary>
        /// Identifies the <see cref="LastSavedWavPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastSavedWavPathProperty = DependencyProperty.Register(DependencyPropertyName_LastSavedWavPath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).LastSavedWavPath_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastSavedWavPath_CoerceValue(baseValue)));

        /// <summary>
        /// Path to last saved WAV file path.
        /// </summary>
        public string LastSavedWavPath
        {
            get { return GetValue(LastSavedWavPathProperty) as string; }
            set { SetValue(LastSavedWavPathProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LastSavedWavPath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="LastSavedWavPath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="LastSavedWavPath"/> property was changed.</param>
        protected virtual void LastSavedWavPath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastSavedWavPath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <see cref="LastSavedWavPath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastSavedWavPath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Model.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastAudioPath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastAudioPath"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LastAudioPath = "LastAudioPath";

        /// <summary>
        /// Identifies the <see cref="LastAudioPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastAudioPathProperty = DependencyProperty.Register(DependencyPropertyName_LastAudioPath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).LastAudioPath_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastAudioPath_CoerceValue(baseValue)));

        /// <summary>
        /// Path to last file imported as audio into SSML.
        /// </summary>
        public string LastAudioPath
        {
            get { return GetValue(LastAudioPathProperty) as string; }
            set { SetValue(LastAudioPathProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LastAudioPath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="LastAudioPath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="LastAudioPath"/> property was changed.</param>
        protected virtual void LastAudioPath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastAudioPath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <see cref="LastAudioPath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastAudioPath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Model.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastPlsFilePath Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastPlsFilePath"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LastPlsFilePath = "LastPlsFilePath";

        /// <summary>
        /// Identifies the <see cref="LastPlsFilePath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastPlsFilePathProperty = DependencyProperty.Register(DependencyPropertyName_LastPlsFilePath, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).LastPlsFilePath_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastPlsFilePath_CoerceValue(baseValue)));

        /// <summary>
        /// Path to last loaded or saved Pronunciation Lexicon Specification file.
        /// </summary>
        public string LastPlsFilePath
        {
            get { return GetValue(LastPlsFilePathProperty) as string; }
            set { SetValue(LastPlsFilePathProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LastPlsFilePath"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="LastPlsFilePath"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="LastPlsFilePath"/> property was changed.</param>
        protected virtual void LastPlsFilePath_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastPlsFilePath = newValue;
            SavePropertiesAsync();
            UpdateLastBrowedDirectory(newValue);
        }

        /// <summary>
        /// This gets called whenever <see cref="LastPlsFilePath"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastPlsFilePath_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            Model.FileUtility.GetLocalPath(path, out path);
            return path;
        }

        #endregion

        #region LastBrowsedSubdirectory Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="LastBrowsedSubdirectory"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_LastBrowsedSubdirectory = "LastBrowsedSubdirectory";

        /// <summary>
        /// Identifies the <see cref="LastBrowsedSubdirectory"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LastBrowsedSubdirectoryProperty = DependencyProperty.Register(DependencyPropertyName_LastBrowsedSubdirectory, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).LastBrowsedSubdirectory_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).LastBrowsedSubdirectory_CoerceValue(baseValue)));

        /// <summary>
        /// Path to last folder when browsing to save, load or import a file.
        /// </summary>
        public string LastBrowsedSubdirectory
        {
            get { return GetValue(LastBrowsedSubdirectoryProperty) as string; }
            set { SetValue(LastBrowsedSubdirectoryProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="LastBrowsedSubdirectory"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="LastBrowsedSubdirectory"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="LastBrowsedSubdirectory"/> property was changed.</param>
        protected virtual void LastBrowsedSubdirectory_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.LastBrowsedSubdirectory = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <see cref="LastBrowsedSubdirectory"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string LastBrowsedSubdirectory_CoerceValue(object baseValue)
        {
            string path = baseValue as string;
            if (String.IsNullOrWhiteSpace(path))
                return "";

            if (Model.FileUtility.GetLocalPath(path, out path) && File.Exists(path))
                return System.IO.Path.GetDirectoryName(path);
            return path;
        }

        #endregion

        #region DefaultSpeechRate Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="DefaultSpeechRate"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DefaultSpeechRate = "DefaultSpeechRate";

        /// <summary>
        /// Minimum acceptable value for speech rate.
        /// </summary>
        public const int SpeechRate_MinValue = -10;

        /// <summary>
        /// Maximum acceptable value for speech rate.
        /// </summary>
        public const int SpeechRate_MaxValue = 10;

        /// <summary>
        /// Defines the default value for the <see cref="DefaultSpeechRate"/> dependency property.
        /// </summary>
        public const int DefaultValue_DefaultSpeechRate = 0;

        /// <summary>
        /// Identifies the <see cref="DefaultSpeechRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultSpeechRateProperty = DependencyProperty.Register(DependencyPropertyName_DefaultSpeechRate, typeof(int), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_DefaultSpeechRate,
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).DefaultSpeechRate_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)),
                    DefaultSpeechRate_CoerceValue));

        /// <summary>
        /// Default speech rate for speech generation.
        /// </summary>
        /// <remarks>Values range from -10 to 10 (inclusive).</remarks>
        public int DefaultSpeechRate
        {
            get { return (int)(GetValue(DefaultSpeechRateProperty)); }
            set { SetValue(DefaultSpeechRateProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="DefaultSpeechRate"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="DefaultSpeechRate"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="DefaultSpeechRate"/> property was changed.</param>
        protected virtual void DefaultSpeechRate_PropertyChanged(int oldValue, int newValue)
        {
            Properties.Settings.Default.DefaultSpeechRate = newValue;
            SavePropertiesAsync();
        }
        
        private static object DefaultSpeechRate_CoerceValue(DependencyObject d, object baseValue)
        {
            int? value = baseValue as int?;
            return (value.HasValue) ? ((value.Value < SpeechRate_MinValue) ? SpeechRate_MinValue : ((value.Value < SpeechRate_MaxValue) ? SpeechRate_MaxValue : value.Value)) : DefaultValue_DefaultSpeechRate;
        }

        #endregion

        #region DefaultSpeechVolume Property Members

        /// <summary>
        /// Defines the name for the <see cref="DefaultSpeechVolume"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DefaultSpeechVolume = "DefaultSpeechVolume";

        /// <summary>
        /// Minimum acceptable value for speech rate.
        /// </summary>
        public const int SpeechVolume_MinValue = 0;

        /// <summary>
        /// Maximum acceptable value for speech rate.
        /// </summary>
        public const int SpeechVolume_MaxValue = 100;

        public const int DefaultValue_DefaultSpeechVolume = 100;

        /// <summary>
        /// Identifies the <see cref="DefaultSpeechVolume"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultSpeechVolumeProperty = DependencyProperty.Register(DependencyPropertyName_DefaultSpeechVolume, typeof(int), typeof(AppSettingsVM),
                new PropertyMetadata(DefaultValue_DefaultSpeechVolume,
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).DefaultSpeechVolume_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)),
                    DefaultSpeechVolume_CoerceValue));

        /// <summary>
        /// Default volume for speech generation
        /// </summary>
        /// <remarks>Values range from 0 to 100, inclusive.</remarks>
        public int DefaultSpeechVolume
        {
            get { return (int)(GetValue(DefaultSpeechVolumeProperty)); }
            set { SetValue(DefaultSpeechVolumeProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="DefaultSpeechVolume"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="DefaultSpeechVolume"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="DefaultSpeechVolume"/> property was changed.</param>
        protected virtual void DefaultSpeechVolume_PropertyChanged(int oldValue, int newValue)
        {
            Properties.Settings.Default.DefaultSpeechVolume = newValue;
            SavePropertiesAsync();
        }

        private static object DefaultSpeechVolume_CoerceValue(DependencyObject d, object baseValue)
        {
            int? value = baseValue as int?;
            return (value.HasValue) ? ((value.Value < SpeechVolume_MinValue) ? SpeechVolume_MinValue : ((value.Value < SpeechVolume_MaxValue) ? SpeechVolume_MaxValue : value.Value)) : DefaultValue_DefaultSpeechVolume;
        }

        #endregion

        #region DefaultVoiceName Property Members

        /// <summary>
        /// Defines the name for the <see cref="DefaultVoiceName"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_DefaultVoiceName = "DefaultVoiceName";

        /// <summary>
        /// Identifies the <see cref="DefaultVoiceName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultVoiceNameProperty = DependencyProperty.Register(DependencyPropertyName_DefaultVoiceName, typeof(string), typeof(AppSettingsVM),
                new PropertyMetadata("",
                    (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as AppSettingsVM).DefaultVoiceName_PropertyChanged(e.OldValue as string, e.NewValue as string),
                    (DependencyObject d, object baseValue) => (d as AppSettingsVM).DefaultVoiceName_CoerceValue(baseValue)));

        /// <summary>
        /// Default voice name for speech generation
        /// </summary>
        public string DefaultVoiceName
        {
            get { return GetValue(DefaultVoiceNameProperty) as string; }
            set { SetValue(DefaultVoiceNameProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="DefaultVoiceName"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <see cref="DefaultVoiceName"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <see cref="DefaultVoiceName"/> property was changed.</param>
        protected virtual void DefaultVoiceName_PropertyChanged(string oldValue, string newValue)
        {
            Properties.Settings.Default.DefaultVoiceName = newValue;
            SavePropertiesAsync();
        }

        /// <summary>
        /// This gets called whenever <see cref="DefaultVoiceName"/> is being re-evaluated, or coercion is specifically requested.
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
                    LastBrowsedSubdirectory = System.IO.Path.GetDirectoryName(filePath);
                else if (Directory.Exists(filePath))
                    LastBrowsedSubdirectory = filePath;
                else
                {
                    string path = System.IO.Path.GetDirectoryName(filePath);
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
