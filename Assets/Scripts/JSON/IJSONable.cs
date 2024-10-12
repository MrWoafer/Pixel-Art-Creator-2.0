namespace PAC.JSON
{
    /// <summary>
    /// An interface for objects that can be converted to JSON.
    /// </summary>
    public interface IJSONable
    {
        public JSON ToJSON();
    }
}
