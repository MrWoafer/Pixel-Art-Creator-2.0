using System;

namespace PAC.Exceptions
{
    /// <summary>
    /// Denotes an error in implementing a feature, such as reaching code that should be unreachable or a true condition that should always be false.
    /// </summary>
    public class ImplementationException : Exception
    {
        public ImplementationException(string message) : base(message) { }
    }
}
