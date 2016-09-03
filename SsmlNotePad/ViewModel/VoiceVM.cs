using System;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel
{
    public class VoiceVM : DependencyObject
    {
        #region Id Property Members

        public const string PropertyName_Id = "Id";

        private static readonly DependencyPropertyKey IdPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Id, typeof(string), typeof(VoiceVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Id"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty = IdPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(IdProperty));
                return Dispatcher.Invoke(() => Id);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(IdPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Id = value);
            }
        }

        #endregion

        #region Name Property Members

        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name, typeof(string), typeof(VoiceVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = NamePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NameProperty));
                return Dispatcher.Invoke(() => Name);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(NamePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Name = value);
            }
        }

        #endregion

        #region DisplayText Property Members

        public const string NotSet_DisplayText = "Not Set";
        public const string PropertyName_DisplayText = "DisplayText";

        private static readonly DependencyPropertyKey DisplayTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DisplayText, typeof(string), typeof(VoiceVM),
                new PropertyMetadata(NotSet_DisplayText));

        /// <summary>
        /// Identifies the <seealso cref="DisplayText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayTextProperty));
                return Dispatcher.Invoke(() => DisplayText);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DisplayTextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => DisplayText = value);
            }
        }

        #endregion

        #region Gender Property Members

        public const string PropertyName_Gender = "Gender";

        private static readonly DependencyPropertyKey GenderPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Gender, typeof(VoiceGender), typeof(VoiceVM),
                new PropertyMetadata(VoiceGender.NotSet));

        /// <summary>
        /// Identifies the <seealso cref="Gender"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GenderProperty = GenderPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public VoiceGender Gender
        {
            get
            {
                if (CheckAccess())
                    return (VoiceGender)(GetValue(GenderProperty));
                return Dispatcher.Invoke(() => Gender);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(GenderPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Gender = value);
            }
        }

        #endregion

        #region Age Property Members

        public const string PropertyName_Age = "Age";

        private static readonly DependencyPropertyKey AgePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Age, typeof(VoiceAge), typeof(VoiceVM),
                new PropertyMetadata(VoiceAge.NotSet));

        /// <summary>
        /// Identifies the <seealso cref="Age"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AgeProperty = AgePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public VoiceAge Age
        {
            get
            {
                if (CheckAccess())
                    return (VoiceAge)(GetValue(AgeProperty));
                return Dispatcher.Invoke(() => Age);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(AgePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Age = value);
            }
        }

        #endregion

        #region Culture Property Members

        public const string PropertyName_Culture = "Culture";

        private static readonly DependencyPropertyKey CulturePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Culture, typeof(string), typeof(VoiceVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Culture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty = CulturePropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Culture
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(CultureProperty));
                return Dispatcher.Invoke(() => Culture);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CulturePropertyKey, value);
                else
                    Dispatcher.Invoke(() => Culture = value);
            }
        }

        #endregion

        #region Description Property Members

        public const string PropertyName_Description = "Description";

        private static readonly DependencyPropertyKey DescriptionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Description, typeof(string), typeof(VoiceVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="Description"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DescriptionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DescriptionProperty));
                return Dispatcher.Invoke(() => Description);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DescriptionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Description = value);
            }
        }

        #endregion

        internal void SetVoice(VoiceInfo voice)
        {
            CultureInfo culture;
            if (voice == null)
            {
                Age = VoiceAge.NotSet;
                Description = "";
                Gender = VoiceGender.NotSet;
                Id = "";
                Name = "";
                DisplayText = NotSet_DisplayText;
                culture = CultureInfo.InvariantCulture;
            }
            else
            {
                Age = voice.Age;
                Description = voice.Description ?? "";
                Gender = voice.Gender;
                Id = voice.Id ?? "";
                Name = voice.Name ?? "";
                if (String.IsNullOrWhiteSpace(voice.Description))
                {
                    if (String.IsNullOrWhiteSpace(voice.Name))
                    {
                        if (String.IsNullOrWhiteSpace(voice.Id))
                        {
                            if (voice.Age == VoiceAge.NotSet)
                                DisplayText = (voice.Gender == VoiceGender.NotSet) ? NotSet_DisplayText : String.Format("{0} voice", voice.Gender.ToString("F"));
                            else
                                DisplayText = (voice.Gender == VoiceGender.NotSet) ? String.Format("{0} voice", voice.Age.ToString("F")) : 
                                    String.Format("{0} {1} voice", voice.Gender.ToString("F"), voice.Age.ToString("F"));
                        }
                        else
                            DisplayText = voice.Id;
                    }
                    else
                        DisplayText = voice.Name;
                }
                else
                    DisplayText = voice.Description;
                culture = voice.Culture ?? CultureInfo.InvariantCulture;
            }

            Culture = (String.IsNullOrWhiteSpace(culture.DisplayName)) ? culture.ToString() : culture.DisplayName;
        }
    }
}