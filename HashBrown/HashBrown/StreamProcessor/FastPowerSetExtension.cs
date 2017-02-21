using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamProcessor
{
    public static class FastPowerSetExtension
    {
        public static T[][] PowerSet<T>(this T[] seq)
        {
            var powerSet = new T[1 << seq.Length][];
            powerSet[0] = new T[0]; // empty set

            for (int i = 0; i < seq.Length; i++)
            {
                var cur = seq[i];
                int count = 1 << i;
                for (int j = 0; j < count; j++)
                {
                    var source = powerSet[j];
                    var destination = powerSet[count + j] = new T[source.Length + 1];
                    for (int q = 0; q < source.Length; q++)
                    {
                        destination[q] = source[q];
                    }
                    destination[source.Length] = cur;
                }
            }

            return powerSet;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> seq, int k)
        {
            return k == 0 ? new[] { new T[0] } :
                seq.SelectMany((e, i) =>
                seq.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}
