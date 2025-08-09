namespace PAC.Extensions.System
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Creates an array of length <paramref name="length"/> filled with <paramref name="value"/>.
        /// </summary>
        public static T[] Filled<T>(T value, int length)
        {
            T[] array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = value;
            }
            return array;
        }
    }
}