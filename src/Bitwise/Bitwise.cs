using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.Utils
{
    public abstract class Bitwise<T> where T: struct
    {
        protected int Bit { get; set; }

        protected Bitwise()
        {
            if (typeof(T).GetTypeInfo().BaseType != typeof(Enum) || typeof(T).GetTypeInfo().BaseType != typeof(int))
            {
                throw new InvalidCastException();
            }
        }

        protected Bitwise(T bit)
        {
            Bit = Convert.ToInt32(bit);
        }

        public virtual bool Has(T bit)
        {
            return (Bit & Convert.ToInt32(bit)) > 0;
        }

        public virtual bool Is(T bit)
        {
            return Bit == Convert.ToInt32(bit);
        }

        public virtual Bitwise<T> Set(T bit)
        {
            Bit = Convert.ToInt32(bit);
            return this;
        }

        public virtual Bitwise<T> Add(T bit)
        {
            Bit = Bit | Convert.ToInt32(bit);
            return this;
        }

        public virtual Bitwise<T> Remove(T bit)
        {
            var convertedBit = Convert.ToInt32(bit);
            Bit = (Bit | convertedBit) & convertedBit;
            return this;
        }

        public virtual IEnumerable<T> ToArray()
        {
            for (var i = 1; i <= Bit; i *= 2)
            {
                if ((i & Bit) <= 0) continue;

                yield return (T)Enum.ToObject(typeof(T), i);
            }
        }
    }
}
