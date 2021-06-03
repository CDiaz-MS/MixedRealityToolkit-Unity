using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMenuVisibility : MonoBehaviour, IMixedRealityFocusHandler
{
    public RadialContextMenu menu;

    private void Start()
    {

    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        // Set Visibility of menu
    }

    public void OnFocusExit(FocusEventData eventData)
    {

    }
}
