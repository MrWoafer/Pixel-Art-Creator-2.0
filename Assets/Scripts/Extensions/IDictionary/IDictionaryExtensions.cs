using System.Collections.Generic;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for IDictionary&lt;TKey, TValue&gt;.
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// If the IDictionary already contains the key, it will return the existing value associated with it.
        /// If the key is not present, it will add it with the given value and return that value.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out TValue existingValue))
            {
                return existingValue;
            }
            dict.Add(key, value);
            return value;
        }
    }
}
