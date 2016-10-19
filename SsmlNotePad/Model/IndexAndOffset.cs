using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.SsmlNotePad.Model
{
    public class IndexAndOffset
    {
        public int Index { get; set; }
        public double Top { get; set; }
        public IndexAndOffset() { }
        public IndexAndOffset(int index, double top)
        {
            Index = index;
            Top = top;
        }
    }
}
