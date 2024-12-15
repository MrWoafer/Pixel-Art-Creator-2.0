using System;

namespace PAC.Exceptions
{
    /// <summary>
    /// Indicates that an operation attempting to be performed is undefined.
    /// </summary>
    public class UndefinedOperationException : InvalidOperationException
    {
        /// <summary>
        /// Indicates that an operation attempting to be performed is undefined.
        /// </summary>
        public UndefinedOperationException(string message) : base(message) { }
    }
}
