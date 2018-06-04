using System;
using System.Collections.Generic;
using System.Text;

namespace Bitwise
{
    public static class BitwiseExtensions
    {
        public static IEnumerable<int> GetBits(this int bit)
        {
            for (var i = 1; i <= bit; i *= 2)
            {
                if ((i & bit) <= 0) continue;
                yield return i;
            }
        }

        public static bool HasBit(this int bit, int targetBit)
        {
            return (bit & targetBit) > 0;
        }

        public static bool IsBit(this int bit, int targetBit)
        {
            return (bit & targetBit) > 0;
        }

        public static int AddBit(this int bit, int targetBit)
        {
            return bit == 0 ? targetBit : (bit | targetBit);
        }

        public static int RemoveBit(this int bit, int targetBit)
        {
            return bit == 0 ? bit : ((bit | targetBit) ^ targetBit);
        }
    }
}
