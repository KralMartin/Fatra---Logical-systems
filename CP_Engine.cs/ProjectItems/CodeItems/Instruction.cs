using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine.cs.ProjectItems.CodeItems
{
    enum InstructionTypes { Ignored, Line }

    class Instruction
    {
        internal List<InstructionParameter> Parameters { get; private set; }
        internal int BinaryIndex { get; set; }

        internal InstructionTypes ignored;
        private string functionName;
        internal Function boundFunction { get; private set; }

        public Instruction(InstructionTypes ignored, string name)
        {
            this.Parameters = new List<InstructionParameter>();
            this.ignored = ignored;
            if (ignored == InstructionTypes.Line)
                this.functionName = name;
            else
            {
                InstructionParameter p = new InstructionParameter(name, null);
                this.Parameters.Add(p);
            }
        }
        
        internal void BindFunction(Programmability programmability)
        {
            foreach (Function fun in programmability.FunctionItems)
            {
                if (functionName == fun.FunctionName && fun.Parameters.Count== this.Parameters.Count)
                {
                    if (CompareParameters(fun) == true)
                        this.boundFunction = fun;
                }
            }
        }
        
        private bool CompareParameters(Function fun)
        {
            int i = 0;
            foreach (InstructionParameter insParam in this.Parameters)
            {
                Parameter funParam = fun.Parameters[i];
                if (insParam.Lenght > funParam.NumberLenght)
                    return false;
                i++;
            }
            return true;
        }

        internal void ToBits(List<bool> result)
        {
            if (ignored == InstructionTypes.Ignored)
            {
                result.AddRange(Parameters[0].Bits);
            }
            else
            {
                string lowVariant = boundFunction.FunctionCode.ToLowerInvariant();
                for (int i = 0; i < lowVariant.Length; i++)
                {
                    char c = lowVariant[i];
                    if (c == '+' || c == '-')
                        i++;
                    if (c == '1')
                        result.Add(true);
                    else if (c == '0')
                        result.Add(false);
                    else if(c>='a' && c<='z')
                    {
                        Parameter p = boundFunction.GetParameter(c);
                        result.Add(this.Parameters[p.Index].GetBit());
                    }
                }
            }
        }

        internal void SetParameters(Compilator compilator)
        {
            int index = 0;
            Parameter p = null;
            foreach (InstructionParameter insParam in this.Parameters)
            {
                if (boundFunction != null)
                {
                    p = boundFunction.Parameters[index];
                    insParam.SetParameter(compilator, p.NumberLenght, p.Adjust);
                }
                else
                    insParam.SetParameter(compilator, compilator.programmability.InstructionWidth, 0);
                index++;
            }
        }
    }
}
