using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Erwine.Leonard.T.SsmlNotePad.ViewModel.Converter
{
    /// <summary>
    /// Converts speech status enumerated value to an integer value.
    /// </summary>
    [ValueConversion(typeof(Model.SpeechProgressState), typeof(int))]
    public class SpeechProgressToIntConverter : ToValueConverterBase<Model.SpeechProgressState, int>
    {
        #region NotStarted Property Members

        public const string DependencyPropertyName_NotStarted = "NotStarted";

        /// <summary>
        /// Identifies the <see cref="NotStarted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotStartedProperty = DependencyProperty.Register(DependencyPropertyName_NotStarted, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata((int)(Model.SpeechProgressState.NotStarted)));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer has not been started
        /// (<see cref="Model.SpeechProgressState.NotStarted"/> source value).
        /// </summary>
        public int? NotStarted
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(NotStartedProperty));
                return Dispatcher.Invoke(() => NotStarted);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NotStartedProperty, value);
                else
                    Dispatcher.Invoke(() => NotStarted = value);
            }
        }


        #endregion

        #region SpeakingNormal Property Members

        public const string DependencyPropertyName_SpeakingNormal = "SpeakingNormal";

        /// <summary>
        /// Identifies the <see cref="SpeakingNormal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeakingNormalProperty = DependencyProperty.Register(DependencyPropertyName_SpeakingNormal, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer is currently speaking, no faults have been encountered and is not in a
        /// state of being canceled (<see cref="Model.SpeechProgressState.SpeakingNormal"/> source value).
        /// </summary>
        public int? SpeakingNormal
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(SpeakingNormalProperty));
                return Dispatcher.Invoke(() => SpeakingNormal);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SpeakingNormalProperty, value);
                else
                    Dispatcher.Invoke(() => SpeakingNormal = value);
            }
        }

        #endregion

        #region SpeakingWithFault Property Members

        public const string DependencyPropertyName_SpeakingWithFault = "SpeakingWithFault";

        /// <summary>
        /// Identifies the <see cref="SpeakingWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeakingWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_SpeakingWithFault, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer is currently speaking, at least one fault has been encountered and is
        /// not in a state of being canceled (<see cref="Model.SpeechProgressState.SpeakingWithFault"/> source value).
        /// </summary>
        public int? SpeakingWithFault
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(SpeakingWithFaultProperty));
                return Dispatcher.Invoke(() => SpeakingWithFault);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SpeakingWithFaultProperty, value);
                else
                    Dispatcher.Invoke(() => SpeakingWithFault = value);
            }
        }
        
        #endregion

        #region PausedNormal Property Members

        public const string DependencyPropertyName_PausedNormal = "PausedNormal";

        /// <summary>
        /// Identifies the <see cref="PausedNormal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedNormalProperty = DependencyProperty.Register(DependencyPropertyName_PausedNormal, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer is currently paused and no faults have been encountered
        /// (<see cref="Model.SpeechProgressState.PausedNormal"/> source value).
        /// </summary>
        public int? PausedNormal
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(PausedNormalProperty));
                return Dispatcher.Invoke(() => PausedNormal);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PausedNormalProperty, value);
                else
                    Dispatcher.Invoke(() => PausedNormal = value);
            }
        }

        #endregion

        #region PausedWithFault Property Members

        public const string DependencyPropertyName_PausedWithFault = "PausedWithFault";

        /// <summary>
        /// Identifies the <see cref="PausedWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PausedWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_PausedWithFault, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer is currently paused and at least one fault has been encountered
        /// (<see cref="Model.SpeechProgressState.PausedWithFault"/> source value).
        /// </summary>
        public int? PausedWithFault
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(PausedWithFaultProperty));
                return Dispatcher.Invoke(() => PausedWithFault);
            }
            set
            {
                if (CheckAccess())
                    SetValue(PausedWithFaultProperty, value);
                else
                    Dispatcher.Invoke(() => PausedWithFault = value);
            }
        }

        #endregion

        #region Cancelling Property Members

        public const string DependencyPropertyName_Cancelling = "Cancelling";

        /// <summary>
        /// Identifies the <see cref="Cancelling"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancellingProperty = DependencyProperty.Register(DependencyPropertyName_Cancelling, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer is currently speaking and is in the state of being canceled
        /// (<see cref="Model.SpeechProgressState.Cancelling"/> source value).
        /// </summary>
        public int? Cancelling
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(CancellingProperty));
                return Dispatcher.Invoke(() => Cancelling);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CancellingProperty, value);
                else
                    Dispatcher.Invoke(() => Cancelling = value);
            }
        }
        
        #endregion

        #region CompletedSuccess Property Members

        public const string DependencyPropertyName_CompletedSuccess = "CompletedSuccess";

        /// <summary>
        /// Identifies the <see cref="CompletedSuccess"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompletedSuccessProperty = DependencyProperty.Register(DependencyPropertyName_CompletedSuccess, typeof(int?), 
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer has completed successfully, no faults have been encountered and was not canceled
        /// (<see cref="Model.SpeechProgressState.CompletedSuccess"/> source value).
        /// </summary>
        public int? CompletedSuccess
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(CompletedSuccessProperty));
                return Dispatcher.Invoke(() => CompletedSuccess);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CompletedSuccessProperty, value);
                else
                    Dispatcher.Invoke(() => CompletedSuccess = value);
            }
        }
        
        #endregion

        #region CompletedWithFault Property Members

        public const string DependencyPropertyName_CompletedWithFault = "CompletedWithFault";

        /// <summary>
        /// Identifies the <see cref="CompletedWithFault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CompletedWithFaultProperty = DependencyProperty.Register(DependencyPropertyName_CompletedWithFault, typeof(int?),
            typeof(SpeechProgressToIntConverter), new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer has completed, at least one fault has been encountered, and was not canceled
        /// (<see cref="Model.SpeechProgressState.CompletedWithFault"/> source value).
        /// </summary>
        public int? CompletedWithFault
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(CompletedWithFaultProperty));
                return Dispatcher.Invoke(() => CompletedWithFault);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CompletedWithFaultProperty, value);
                else
                    Dispatcher.Invoke(() => CompletedWithFault = value);
            }
        }
        
        #endregion
        
        #region Canceled Property Members

        public const string DependencyPropertyName_Canceled = "Canceled";

        /// <summary>
        /// Identifies the <see cref="Canceled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanceledProperty = DependencyProperty.Register(DependencyPropertyName_Canceled, typeof(int?), typeof(SpeechProgressToIntConverter),
                new PropertyMetadata(0));

        /// <summary>
        /// <see cref="Nullable{Int32}"/> value which indicates that the speech synthesizer has completed and was canceled
        /// (<seealso cref="Model.SpeechProgressState.Canceled"/> source value).
        /// </summary>
        public int? Canceled
        {
            get
            {
                if (CheckAccess())
                    return (int?)(GetValue(CanceledProperty));
                return Dispatcher.Invoke(() => Canceled);
            }
            set
            {
                if (CheckAccess())
                    SetValue(CanceledProperty, value);
                else
                    Dispatcher.Invoke(() => Canceled = value);
            }
        }

        #endregion

        public SpeechProgressToIntConverter()
        {
            NotStarted = (int)(Model.SpeechProgressState.NotStarted);
            NullSource = NotStarted;
            SpeakingNormal = (int)(Model.SpeechProgressState.SpeakingNormal);
            SpeakingWithFault = (int)(Model.SpeechProgressState.SpeakingWithFault);
            PausedNormal = (int)(Model.SpeechProgressState.PausedNormal);
            PausedWithFault = (int)(Model.SpeechProgressState.PausedWithFault);
            Cancelling = (int)(Model.SpeechProgressState.Cancelling);
            CompletedSuccess = (int)(Model.SpeechProgressState.CompletedSuccess);
            CompletedWithFault = (int)(Model.SpeechProgressState.CompletedWithFault);
            Canceled = (int)(Model.SpeechProgressState.Canceled);
        }

        /// <summary>
        /// Converts a <seealso cref="Model.SpeechProgressState"/> value to an <seealso cref="int"/> value.
        /// </summary>
        /// <param name="value">The <seealso cref="Model.SpeechProgressState"/> produced by the binding source.</param>
        /// <param name="parameter">Parameter passed by the binding source.</param>
        /// <param name="culture">Culture specified through the binding source.</param>
        /// <returns><seealso cref="Model.SpeechProgressState"/>value converted to an <seealso cref="int"/> or null value.</returns>
        public override int? Convert(Model.SpeechProgressState value, object parameter, CultureInfo culture)
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
