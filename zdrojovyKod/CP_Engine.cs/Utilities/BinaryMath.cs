using System;
using System.Collections.Generic;
using System.Text;

namespace CP_Engine
{
    public class BinaryMath
    {
        internal static string FormatBits(bool[] bits, NumberFormats format)
        {
            switch (format)
            {
                case NumberFormats.Binary:
                    return ToBinarry(bits);
                case NumberFormats.Decimal:
                    return ToDecimal(bits);
                case NumberFormats.Hex:
                    return ToHex(bits);
                default:
                    return ToBinarry(bits);
            }
        }

        public static string ToBinarry(bool[] bits)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bits.Length; i++)
            {
                if (i % 4 == 0)
                    sb.Append(' ');

                if (bits[i])
                    sb.Append('1');
                else
                    sb.Append('0');
            }
            sb.Append(" b");
            return sb.ToString();
        }

        public static string ToBinarry(bool[] bits, int instructionWidth)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bits.Length; i++)
            {
                if (i % instructionWidth == 0)
                {
                    if (i > 0)
                        sb.Append("\n");
                    sb.Append(String.Format("{0,3}", i / instructionWidth));
                    sb.Append(".   ");
                }
                else if (i % 4 == 0)
                    sb.Append(' ');

                if (bits[i])
                    sb.Append('1');
                else
                    sb.Append('0');
            }
            return sb.ToString();
        }

        internal static string ToHex(bool[] bits)
        {
            Stack<string> stack = new Stack<string>();
            int index = 0;
            int value = 0;
            while (index < bits.Length)
            {
                if (index>0 && index % 8 == 0)
                    stack.Push(" ");
                if (bits[index])
                    value = 1;
                else
                    value = 0;
                if (bits.Length > index + 1)
                {
                    if (bits[index + 1])
                        value += 2;
                    if (bits.Length > index + 2)
                    {
                        if (bits[index + 2])
                            value += 4;
                        if (bits.Length > index + 3)
                        {
                            if (bits[index + 3])
                                value += 8;
                        }
                    }
                }
                switch (value)
                {
                    case 15:
                        stack.Push("F");
                        break;
                    case 14:
                        stack.Push("E");
                        break;
                    case 13:
                        stack.Push("D");
                        break;
                    case 12:
                        stack.Push("C");
                        break;
                    case 11:
                        stack.Push("B");
                        break;
                    case 10:
                        stack.Push("A");
                        break;
                    default:
                        stack.Push(value.ToString());
                        break;
                }
                index += 4;
            }
            StringBuilder sb = new StringBuilder();
            while (stack.Count > 0)
                sb.Append(stack.Pop());
            sb.Append(" h");
            return sb.ToString();
        }

        internal static List<bool> Round(int lenght, List<bool> bits)
        {
            while (bits.Count < lenght)
                bits.Add(false);
            return bits;
        }

        internal static List<bool> GetBinary(int decimalN)
        {
            string binaryStr = Convert.ToString(decimalN, 2);
            return GetBinary(binaryStr);
        }

        internal static string ToDecimal(bool[] bits)
        {
            return GetDecimal(bits) + " d";
        }

        private static int IntPow(int x, int pow)
        {
            int toReturn = 1;
            while (pow > 0)
            {
                if ((pow & 1) == 1)
                    toReturn *= x;
                x *= x;
                pow >>= 1;
            }
            return toReturn;
        }

        internal static int GetDecimal(bool[] bits)
        {
            int toReturn = 0;
            int index = bits.Length - 1;
            while (index >= 0)
            {
                if (bits[index])
                    toReturn += IntPow(2, index);
                index--;
            }
            return toReturn;
        }
        internal static List<bool> GetBinary(string text)
        {
            List<bool> toReturn = new List<bool>();
            for (int i = text.Length-1; i >=0; i--)
            {
                if (text[i] == '1')
                    toReturn.Add(true);
                else if (text[i] == '0')
                    toReturn.Add(false);
                else if (text[i] != ' ')
                    return null;
            }
            return toReturn;
        }
        
    }
}
