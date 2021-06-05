using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDrivenVisibility : MonoBehaviour, IMixedRealityPointerHandler
{
    public RadialMenu Menu;

    private float delay = 0.3f;

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (Menu.Open)
        {
            Menu.Open = false;
            StartCoroutine(DelayDisable());
        }
        else
        {
            Menu.SetMenuItemVisibility(true);
            Menu.Open = true;           
        }

        Menu.OnMenuItemSelected.Invoke(gameObject);
        Debug.Log("Object Selected: " + gameObject.name);
    }

    private IEnumerator DelayDisable()
    {
        yield return new WaitForSeconds(delay);
        Menu.SetMenuItemVisibility(false);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}
