using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Erwine.Leonard.T.SsmlNotePad.Process
{
    public static class Utility
    {
        public static Regex OuterWhitespaceRegex = new Regex(@"^(?<lws>\s*)(?<t>\S+(\s+\S+)*)(?<rws>\s*)$", RegexOptions.Compiled);
    }
}
