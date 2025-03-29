namespace System.Runtime.CompilerServices
{
    // From the Unity docs: https://docs.unity3d.com/6000.0/Documentation/Manual/csharp-compiler.html
    //      "The type System.Runtime.CompilerServices.IsExternalInit is required for full record support as it uses init only setters, but is only available in .NET 5 and later (which Unity
    //      doesn't support). Users can work around this issue by declaring the System.Runtime.CompilerServices.IsExternalInit type in their own projects."
    public class IsExternalInit { }
}