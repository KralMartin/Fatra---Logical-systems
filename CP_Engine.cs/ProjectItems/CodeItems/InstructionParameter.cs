using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP_Engine.cs.ProjectItems.CodeItems
{
    enum InstructionParameterTypes { Number, Variabile, Register }

    class InstructionParameter
    {
        internal InstructionParameterTypes Type { get; set; }

        internal List<bool> Bits { get; set; }
        internal int Lenght { get; set; }
        private string paramText;
        private int index;
        
        internal InstructionParameter(string paramText, Programmability programmability)
        {
            this.paramText = paramText.Replace(" ", "");
            Init(programmability);
        }

        internal void Init(Programmability programmability)
        {
            if (paramText[0] >= 'a' && paramText[0] <= 'z')
            {
                if (paramText.Length == 1)
                {
                    this.Type = InstructionParameterTypes.Register;
                    SetBits(RegisterToBits(paramText[0]));

                }
                else
                {
                    this.Lenght = programmability.InstructionWidth;
                    this.Type = InstructionParameterTypes.Variabile;
                }
            }
            else
            {
                SetBits(BinaryMath.GetBinary(paramText));
                this.Type = InstructionParameterTypes.Number;
            }
        }

        private List<bool> RegisterToBits(char c)
        {
            int decimalN = c - 'a';
            return BinaryMath.GetBinary(decimalN);
        }

        private void SetBits(List<bool> bits)
        {
            this.Bits = bits;
            this.Lenght = bits.Count;
        }

        internal void SetParameter(Compilator compilator, int expectedLenght, int adjustBy)
        {
            if (Type == InstructionParameterTypes.Variabile)
                this.Bits = compilator.GetNav(paramText);
            if (adjustBy != 0)
            {
                int decimalN = BinaryMath.GetDecimal(this.Bits.ToArray()) + adjustBy;
                this.Bits = BinaryMath.GetBinary(decimalN);
            }
            this.Bits = BinaryMath.Round(expectedLenght, this.Bits);
            index = this.Bits.Count;
        }

        internal bool GetBit()
        {
            index--;
            return Bits[index];
        }
    }
}
