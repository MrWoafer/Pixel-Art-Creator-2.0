using System;
using System.Collections.Generic;

namespace PAC.Extensions
{
    public static class RangeExtensions
    {
        public static IEnumerable<int> AsIEnumerable(this Range range, int indexedObjectLength)
        {
            int start = range.Start.IsFromEnd ? indexedObjectLength - range.Start.Value : range.Start.Value;
            int end = range.End.IsFromEnd ? indexedObjectLength - range.End.Value : range.End.Value;
            if (start <= end)
            {
                for (int i = start; i < end; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = start; i > end; i--)
                {
                    yield return i;
                }
            }
        }
    }
}
