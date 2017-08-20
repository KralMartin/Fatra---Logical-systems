using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine.cs.ProjectItems.CodeItems
{
    class Compilator
    {
        internal Programmability programmability { get; set; }
        List<Instruction> instructions;
        Dictionary<string, int> navs;

        internal Compilator(Programmability programmability)
        {
            this.programmability = programmability;
            instructions = new List<Instruction>();
            navs = new Dictionary<string, int>();
        }

        internal List<bool> GetBinaryCode(List<string> cleanText)
        {
            List<bool> toReturn = new List<bool>();
            foreach (string line in cleanText)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '1')
                        toReturn.Add(true);
                    else if (line[i] == '0')
                        toReturn.Add(false);
                }
            }           
            return toReturn;
        }

        internal List<bool> Translate(Code code)
        {
            List<string> cleanText = code.GetCleanText();
            if (cleanText[0] != "using comp")
                return GetBinaryCode(cleanText);
            cleanText.RemoveAt(0);
            return Compile(cleanText);
        }

        private List<bool> Compile(List<string> cleanText)
        {
            List<bool> bits = new List<bool>();
            foreach (string line in cleanText)
                ExecuteLine(line.ToLowerInvariant());
            int binaryIndex = 0;
            foreach (Instruction ins in instructions)
            {
                ins.BinaryIndex = binaryIndex;
                if (ins.ignored == InstructionTypes.Ignored)
                {
                    binaryIndex += ins.Parameters[0].Bits.Count / programmability.InstructionWidth;
                }
                else
                {
                    binaryIndex += ins.boundFunction.TotalBits / programmability.InstructionWidth;
                }
            }
            foreach (Instruction ins in instructions)
            {
                ins.SetParameters(this);
                ins.ToBits(bits);
            }
            return bits;
        }
        
        private void ExecuteLine(string line)
        {
            if (line.EndsWith(":"))
            {
                navs.Add(line.Substring(0, line.Length - 1), instructions.Count);
            }
            else if (line.StartsWith("i"))
            {

                Instruction ins = new Instruction(InstructionTypes.Ignored, line.Substring(1, line.Length - 1));
                instructions.Add(ins);
                return;
            }
            else
            {
                int spaceIndex = line.IndexOf(' ');
                string instructionName = line.Substring(0, spaceIndex);

                Instruction ins = new Instruction(InstructionTypes.Line, instructionName);
                instructions.Add(ins);
                string[] parameters = line.Substring(spaceIndex + 1, line.Length - spaceIndex - 1).Split(',');
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].Length > 0)
                        ins.Parameters.Add(new InstructionParameter(parameters[i], programmability));
                }
                ins.BindFunction(programmability);
            }
        }

        internal List<bool> GetNav(string paramText)
        {
            int rowID = navs[paramText];
            int binID = instructions[rowID].BinaryIndex;
            return BinaryMath.GetBinary(binID);
        }
    }
}
