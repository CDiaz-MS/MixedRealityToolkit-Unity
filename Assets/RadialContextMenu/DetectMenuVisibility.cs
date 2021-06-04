using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectMenuVisibility : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler
{
    public RadialContextMenu Menu;

    public void OnFocusEnter(FocusEventData eventData)
    {
        RadialMenuItem menuItem = Menu.FindMenuItemByName(gameObject.name);

        if (menuItem != null)
        {
            menuItem.SetSubMenuVisibility(true);
        }
        else
        {
            Debug.LogError($"{gameObject.name} could not be found in the menu hierarchy");
        }
    }

    public void OnFocusExit(FocusEventData eventData){ }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) 
    {
        RadialMenuItem menuItem = Menu.FindMenuItemByName(gameObject.name);

        if (menuItem != null)
        {
            menuItem.RootMenuItem.DisableAllItemsInHierarchy();
        }
        else
        {
            Debug.LogError($"{gameObject.name} could not be found in the menu hierarchy");
        }

        Menu.ObjectSelected = gameObject;

        Menu.OnMenuItemSelected.Invoke(Menu.ObjectSelected);
    }
}
