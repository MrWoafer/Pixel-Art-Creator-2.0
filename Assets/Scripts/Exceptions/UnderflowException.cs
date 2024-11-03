using System;

namespace PAC.Exceptions
{
    public class UnderflowException : ArithmeticException
    {
        public UnderflowException(string message) : base(message) { }
    }
}