using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface for objects that can be converted to JSON.
/// </summary>
public interface IJSONable
{
    public JSON ToJSON();
}
