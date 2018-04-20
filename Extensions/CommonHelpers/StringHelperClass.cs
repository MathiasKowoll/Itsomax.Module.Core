using System;
using System.Text;

namespace Itsomax.Module.Core.Extensions.CommonHelpers
{
    public  class StringHelperClass
    {
        public static string CamelSplit(string word)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in word)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            word = builder.ToString();
            return word;
        }
    }
}