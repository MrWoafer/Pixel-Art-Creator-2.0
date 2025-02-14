using System;

namespace PAC.Exceptions
{
    /// <summary>
    /// Indicates that an exception arose when running a test. Used to add an extra message, such as printing the test case, on top of the exception that occurred in the test.
    /// </summary>
    public class TestException : AggregateException
    {
        public TestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
