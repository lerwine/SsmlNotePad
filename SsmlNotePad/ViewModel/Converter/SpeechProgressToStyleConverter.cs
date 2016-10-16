using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts speech status enumerated value to a Style value.
    /// </summary>
    [ValueConversion(typeof(Model.SpeechProgressState), typeof(Style))]
    public class SpeechProgressToStyleConverter : ToClassConverterBase<Model.SpeechProgressState, Style>, IValueConverter
    {
        #region NotStarted Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="NotStarted"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_NotStarted = "NotStarted";

        /// <summary>
        /// Identifies the <see cref="NotStarted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotStartedProperty = DependencyProperty.Register(DependencyPropertyName_NotStarted, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer has not been started
        /// (<see cref="Model.SpeechProgressState.NotStarted"/> source value).
        /// </summary>
        public Style NotStarted
        {
            get { return (Style)(GetValue(NotStartedProperty)); }
            set { SetValue(NotStartedProperty, value); }
        }

        #endregion

        #region SpeakingNormal Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SpeakingNormal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SpeakingNormal = "SpeakingNormal";

        /// <summary>
        /// Identifies the <see cref="SpeakingNormal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeakingNormalProperty = DependencyProperty.Register(DependencyPropertyName_SpeakingNormal, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer is currently speaking, no faults have been encountered and is not in a
        /// state of being canceled (<see cref="Model.SpeechProgressState.SpeakingNormal"/> source value).
        /// </summary>
        public Style SpeakingNormal
        {
            get { return (Style)(GetValue(SpeakingNormalProperty)); }
            set { SetValue(SpeakingNormalProperty, value); }
        }

        #endregion

        #region SpeakingWithFault Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="SpeakingWithFault"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_SpeakingWithFault = "SpeakingWithFault";

        /// <summary>
        /// Identifies the <see cref="SpeakingWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeakingWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_SpeakingWithFault, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer is currently speaking, at least one fault has been encountered and is
        /// not in a state of being canceled (<see cref="Model.SpeechProgressState.SpeakingWithFault"/> source value).
        /// </summary>
        public Style SpeakingWithFault
        {
            get { return (Style)(GetValue(SpeakingWithFaultProperty)); }
            set { SetValue(SpeakingWithFaultProperty, value); }
        }

        #endregion

        #region PausedNormal Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PausedNormal"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PausedNormal = "PausedNormal";

        /// <summary>
        /// Identifies the <see cref="PausedNormal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedNormalProperty = DependencyProperty.Register(DependencyPropertyName_PausedNormal, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer is currently paused and no faults have been encountered
        /// (<see cref="Model.SpeechProgressState.PausedNormal"/> source value).
        /// </summary>
        public Style PausedNormal
        {
            get { return (Style)(GetValue(PausedNormalProperty)); }
            set { SetValue(PausedNormalProperty, value); }
        }

        #endregion

        #region PausedWithFault Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="PausedWithFault"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_PausedWithFault = "PausedWithFault";

        /// <summary>
        /// Identifies the <see cref="PausedWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_PausedWithFault, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer is currently paused and at least one fault has been encountered
        /// (<see cref="Model.SpeechProgressState.PausedWithFault"/> source value).
        /// </summary>
        public Style PausedWithFault
        {
            get { return (Style)(GetValue(PausedWithFaultProperty)); }
            set { SetValue(PausedWithFaultProperty, value); }
        }

        #endregion

        #region Cancelling Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Cancelling"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Cancelling = "Cancelling";

        /// <summary>
        /// Identifies the <see cref="Cancelling"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancellingProperty = DependencyProperty.Register(DependencyPropertyName_Cancelling, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer is currently speaking and is in the state of being canceled
        /// (<see cref="Model.SpeechProgressState.Cancelling"/> source value).
        /// </summary>
        public Style Cancelling
        {
            get { return (Style)(GetValue(CancellingProperty)); }
            set { SetValue(CancellingProperty, value); }
        }

        #endregion

        #region CompletedSuccess Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="CompletedSuccess"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_CompletedSuccess = "CompletedSuccess";

        /// <summary>
        /// Identifies the <see cref="CompletedSuccess"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompletedSuccessProperty = DependencyProperty.Register(DependencyPropertyName_CompletedSuccess, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer has completed successfully, no faults have been encountered and was not canceled
        /// (<see cref="Model.SpeechProgressState.CompletedSuccess"/> source value).
        /// </summary>
        public Style CompletedSuccess
        {
            get { return (Style)(GetValue(CompletedSuccessProperty)); }
            set { SetValue(CompletedSuccessProperty, value); }
        }
        
        #endregion

        #region CompletedWithFault Property Members

        /// <summary>
        /// Defines the name for the <see cref="CompletedWithFault"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_CompletedWithFault = "CompletedWithFault";

        /// <summary>
        /// Identifies the <see cref="CompletedWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompletedWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_CompletedWithFault, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer has completed, at least one fault has been encountered, and was not canceled
        /// (<see cref="Model.SpeechProgressState.CompletedWithFault"/> source value).
        /// </summary>
        public Style CompletedWithFault
        {
            get { return (Style)(GetValue(CompletedWithFaultProperty)); }
            set { SetValue(CompletedWithFaultProperty, value); }
        }

        #endregion

        #region Canceled Property Members
        
        /// <summary>
        /// Defines the name for the <see cref="Canceled"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_Canceled = "Canceled";

        /// <summary>
        /// Identifies the <see cref="Canceled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanceledProperty = DependencyProperty.Register(DependencyPropertyName_Canceled, typeof(Style), typeof(SpeechProgressToStyleConverter),
                new PropertyMetadata(null));

        /// <summary>
        /// <see cref="Style"/> value which indicates that the speech synthesizer has completed and was canceled
        /// (<seealso cref="Model.SpeechProgressState.Canceled"/> source value).
        /// </summary>
        public Style Canceled
        {
            get { return (Style)(GetValue(CanceledProperty)); }
            set { SetValue(CanceledProperty, value); }
        }
        
        #endregion

        public override Style Convert(Model.SpeechProgressState value, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Model.SpeechProgressState.SpeakingNormal:
                    return SpeakingNormal;
                case Model.SpeechProgressState.SpeakingWithFault:
                    return SpeakingWithFault;
                case Model.SpeechProgressState.PausedNormal:
                    return PausedNormal;
                case Model.SpeechProgressState.PausedWithFault:
                    return PausedWithFault;
                case Model.SpeechProgressState.Cancelling:
                    return Cancelling;
                case Model.SpeechProgressState.CompletedSuccess:
                    return CompletedSuccess;
                case Model.SpeechProgressState.CompletedWithFault:
                    return CompletedWithFault;
                case Model.SpeechProgressState.Canceled:
                    return Canceled;
            }

            return NotStarted;
        }
    }
}
