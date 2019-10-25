using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ChangeToggleIndex : MonoBehaviour
{
    public void ChangeIndex()
    {
        var toggleCollection = GetComponent<InteractableToggleCollection>();
        int toggleListLength = toggleCollection.ToggleList.Length;

        if (toggleCollection.CurrentIndex == toggleListLength - 1)
        {
            toggleCollection.CurrentIndex = 0;

        }
        else
        {
            toggleCollection.CurrentIndex += 1;
        }
        

       Debug.Log(toggleCollection.CurrentIndex);
    }

    public void ToggleSelection()
    {
        
    }
}
