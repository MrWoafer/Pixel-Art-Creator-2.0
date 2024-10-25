namespace PAC.JSON
{
    /// <summary>
    /// An interface for objects that can be converted to/from JSON.
    /// </summary>
    public interface IJSONable<T>
    {
        public JSON ToJSON();
        public static T FromJSON(JSON json) => throw new System.NotImplementedException("FromJSON() has not yet been implemented for type " + typeof(T) + ".");
    }
}
