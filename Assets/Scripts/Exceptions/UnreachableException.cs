using System;

namespace PAC.Exceptions
{
    /// <summary>
    /// Indicates that logically this code should not be reachable.
    /// </summary>
    public class UnreachableException : Exception
    {
        /// <summary>
        /// Indicates that logically this code should not be reachable.
        /// </summary>
        public UnreachableException() : base("Reached code that should be unreachable.") { }
        /// <summary>
        /// Indicates that logically this code should not be reachable.
        /// </summary>
        public UnreachableException(string message) : base(message) { }
    }
}
