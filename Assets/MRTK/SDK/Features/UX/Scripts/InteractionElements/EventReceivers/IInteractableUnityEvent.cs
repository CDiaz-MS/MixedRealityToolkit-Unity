using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableUnityEvent
{
    /// <summary>
    /// The concrete type for the system, feature or manager.
    /// </summary>
    SystemType EventType { get; }


    /// <summary>
    /// The name of the system, feature or manager.
    /// </summary>
    string Name { get; }


    void CreateUnityEvents();

}
