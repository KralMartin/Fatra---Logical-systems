using CP_Engine.SchemeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine
{
    public enum NumberFormats { None, Binary, Hex, Decimal }
    class DisplayTextConvertor
    {
        internal string Text { get; set; }

        enum States { OutsideBracklet, FunctionName, Parameters, Format }
        List<I_DisplayTextFunction> functions;
        bool errorInDisplayText;

        internal DisplayTextConvertor()
        {
            functions = new List<I_DisplayTextFunction>();
        }

        internal string GetText(PhysScheme pScheme)
        {
            if (errorInDisplayText)
                return "display text error";
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (I_DisplayTextFunction function in functions)
                    sb.Append(function.GetText(pScheme));
                return sb.ToString();
            }
            catch
            {
                return "referencing nonexisting item";
            }
        }
           
        internal bool Changed()
        {
            //{M:1,3,2:b} je display text     
            functions.Clear();
            if (Text == null)
                return true;
            try
            {
                errorInDisplayText = false;
                I_DisplayTextFunction fun = null;
                string functionName = null;
                NumberFormats format = NumberFormats.None;
                States state = States.OutsideBracklet;
                List<string> parameters = new List<string>();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Text.Length; i++)
                {
                    char c = Text[i];
                    switch (c)
                    {
                        case '{':
                            if (state != States.OutsideBracklet)
                                return false;
                            if (sb.Length > 0)
                            {
                                fun = new SimpleStringFunction();
                                parameters.Add(sb.ToString());
                                fun.Create(parameters, NumberFormats.None);
                                functions.Add(fun);
                            }
                            state = States.FunctionName;
                            format = NumberFormats.None;
                            parameters.Clear();
                            sb.Clear();
                            break;
                        case ':':
                            if (state == States.FunctionName)
                            {
                                functionName = sb.ToString();
                                state = States.Parameters;
                            }
                            else if (state == States.Parameters)
                            {
                                parameters.Add(sb.ToString());
                                state = States.Format;
                            }
                            sb.Clear();
                            break;
                        case ',':
                            if (state == States.Parameters)
                            {
                                parameters.Add(sb.ToString());
                                sb.Clear();
                            }
                            break;
                        case '}':
                            if (state == States.Format)
                            {
                                format = FormatFromString(sb.ToString());
                            }
                            else if (state == States.Parameters)
                            {
                                parameters.Add(sb.ToString());
                            }
                            //Create function
                            fun = GetFunction(functionName);
                            fun.Create(parameters, format);
                            functions.Add(fun);
                            state = States.OutsideBracklet;
                            parameters.Clear();
                            sb.Clear();
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }
                if (sb.Length > 0)
                {
                    fun = new SimpleStringFunction();
                    parameters.Add(sb.ToString());
                    fun.Create(parameters, NumberFormats.None);
                    functions.Add(fun);
                }
            }
            catch
            {
                errorInDisplayText = true;
            }
            return errorInDisplayText;
        }

        private I_DisplayTextFunction GetFunction(string functionName)
        {
            switch (functionName)
            {
                case "M":
                    return new MemoryFunction();
                default:
                    return null;
            }
        }

        private NumberFormats FormatFromString(string text)
        {
            switch (text)
            {
                case "b":
                    return NumberFormats.Binary;
                case "d":
                    return NumberFormats.Decimal;
                case "h":
                    return NumberFormats.Hex;
                default:
                    return NumberFormats.None;
            }
        }       

    }
}
