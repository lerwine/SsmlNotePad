using System;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    [Flags]
    public enum XmlEncodeOption : byte
    {
        Minimal = 0,
        DoubleQuoteEncoded = 1,
        ApostropheEncoded = 2,
        ControlCharacterEncoded = 4,
        DoubleQuotedAttribute = 5,
        SingeQuotedAtribute = 6,
        Full = 7,
        WindowsLineEndings = 24,
        UnixLineEndings = 40
    }
}