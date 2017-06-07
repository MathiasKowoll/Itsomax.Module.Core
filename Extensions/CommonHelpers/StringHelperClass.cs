using System;
using System.Text;

namespace Itsomax.Module.Core.Extensions.CommonHelpers
{
    public  class StringHelperClass
    {
        public static string CamelSplit(string Word)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in Word)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            Word = builder.ToString();
            return Word;
        }
    }
}