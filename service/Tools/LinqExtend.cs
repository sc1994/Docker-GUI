using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerGui.Tools
{
    public static class LinqExtend
    {
        public static decimal Avg<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            var sum = source.Sum(selector);
            return sum / source.Count();
        }

        public static int Avg<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            var sum = source.Sum(selector);
            return sum / source.Count();
        }
    }
}