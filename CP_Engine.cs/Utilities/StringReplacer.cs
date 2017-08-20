using CP_Engine.SchemeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    class StringReplacer
    {
        internal Dictionary<string, string> Items { get; private set; }

        char bracketLeft, bracketRight;
        PhysScheme pScheme;

        internal StringReplacer()
        {
            this.Items = new Dictionary<string, string>();
            bracketLeft = '{';
            bracketRight = '}';
        }

        //internal string SaveReplace(string value)
        //{
        //    try
        //    {
        //        return Replace(value);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        internal string Replace(string value, PhysScheme pScheme)
        {
            this.pScheme = pScheme;
            StringBuilder sb = new StringBuilder();
            string buffer = null;
            bool fillingBuffer = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (fillingBuffer == false)
                {
                    if (value[i] == bracketLeft)
                    {
                        buffer = "";
                        fillingBuffer = true;
                    }
                    else
                        sb.Append(value[i]);
                }
                else
                {
                    if (value[i] == bracketRight)
                    {
                        sb.Append(this.Items[buffer]);
                        fillingBuffer = false;
                    }
                    else
                        buffer += value[i];
                }
            }
            return sb.ToString();
        }
    }
}
