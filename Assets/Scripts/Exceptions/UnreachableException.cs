using System;

/// <summary>
/// Indicates that logically this code should not be reachable.
/// </summary>
public class UnreachableException : Exception
{
    public UnreachableException(string message) : base(message) { }
}
