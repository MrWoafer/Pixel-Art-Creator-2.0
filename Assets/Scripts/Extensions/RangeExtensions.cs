using System;
using System.Collections.Generic;

namespace PAC.Extensions
{
    public static class RangeExtensions
    {
        public static IEnumerable<int> AsIEnumerable(this Range range)
        {
            if (range.Start.Value <= range.End.Value)
            {
                for (int i = range.Start.Value; i < range.End.Value; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = range.Start.Value; i > range.End.Value; i--)
                {
                    yield return i;
                }
            }
        }
    }
}
