using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP_Engine.SchemeItems;

namespace CP_Engine
{
    class SimpleStringFunction : I_DisplayTextFunction
    {
        string text;

        public bool Create(List<string> parameters, NumberFormats format)
        {
            if (parameters.Count != 1)
                return false;
            text = parameters[0];
            return true;
        }

        public string GetText(PhysScheme pScheme)
        {
            return text;
        }
    }
}
