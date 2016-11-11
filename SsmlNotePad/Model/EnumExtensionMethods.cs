using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public static class EnumExtensionMethods
    {
        public static AlertLevel ToAlertLevel(this MessageLevel messageLevel)
        {
            switch (messageLevel)
            {
                case MessageLevel.Diagnostic:
                    return AlertLevel.Diagnostic;
                case MessageLevel.Verbose:
                    return AlertLevel.Verbose;
                case MessageLevel.Information:
                    return AlertLevel.Information;
                case MessageLevel.Alert:
                    return AlertLevel.Alert;
                case MessageLevel.Warning:
                    return AlertLevel.Warning;
                case MessageLevel.Error:
                    return AlertLevel.Error;
            }

            return AlertLevel.Critical;
        }
    }
}
