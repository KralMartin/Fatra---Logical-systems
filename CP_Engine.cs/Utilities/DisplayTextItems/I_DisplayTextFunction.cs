using CP_Engine.SchemeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    interface I_DisplayTextFunction
    {
        bool Create(List<string> parameters, NumberFormats format);

        string GetText(PhysScheme pScheme);
    }
}
