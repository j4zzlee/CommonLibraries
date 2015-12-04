using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Utils
{
    public class Bitwise : Bitwise<int>
    {
        public Bitwise(int i) : base(i) { }

        public override IEnumerable<int> ToArray()
        {
            for (var i = 1; i <= Bit; i *= 2)
            {
                if ((i & Bit) <= 0) continue;
                yield return i;
            }
        }
    }
}
