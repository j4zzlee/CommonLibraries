using System;
using System.Collections.Generic;
using System.Text;

namespace StringExtensions
{
    public static class StringExtensions
    {
        public static string Random(int length, string allowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz")
        {
            Random rd = new Random();
            char[] chars = new char[length];
            int setLength = allowedCharacters.Length;
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedCharacters[rd.Next(0, allowedCharacters.Length)];
            }
            return new string(chars);
        }
    }
}
