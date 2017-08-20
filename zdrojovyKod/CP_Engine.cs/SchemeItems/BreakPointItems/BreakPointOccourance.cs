using CP_Engine.SchemeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class BreakPointOccourance
    {
        internal PhysScheme PhysScheme { get; set; }
        internal bool[] OldValues { get; set; }
        internal bool[] NewValues { get; set; }
    }
}
