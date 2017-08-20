using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP_Engine.SchemeItems;

namespace CP_Engine
{
    class MemoryFunction : I_DisplayTextFunction
    {
        List<int> pBugIDs;
        NumberFormats format;

        public bool Create(List<string> parameters, NumberFormats format)
        {
            this.format = format;
            if (parameters.Count == 1 && parameters[0] == "all")
            {
            }
            else
            {
                pBugIDs = new List<int>();
                foreach (string param in parameters)
                    pBugIDs.Add(Convert.ToInt32(param));
            }
            return true;
        }

        public string GetText(PhysScheme pScheme)
        {
            if (pBugIDs == null)
            {
                List<bool> bits = new List<bool>();
                pScheme.PlacedBug.Bug.GetMemmoryValue(bits, pScheme);
                return BinaryMath.FormatBits(bits.ToArray(), format);
            }
            return "err not implemented";
        }
    }
}
