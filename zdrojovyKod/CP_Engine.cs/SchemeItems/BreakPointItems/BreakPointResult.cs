using CP_Engine.SchemeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class BreakPointResult
    {
        internal BreakPoint BreakPoint { get; private set; }
        internal List<BreakPointOccourance> BreakOccourances { get; private set; }

        internal BreakPointResult(BreakPoint breakPoint)
        {
            this.BreakPoint = breakPoint;
            BreakOccourances = new List<BreakPointOccourance>();
        }
    }
}
