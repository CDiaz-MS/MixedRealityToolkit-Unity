using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementVolumeManager : MonoBehaviour
{
    public BaseVolume currentChild;

    public BaseVolume targetVolume;
    
    public BaseVolume parentContainer;

    public GameObject newCube;

    public void Switch()
    {
        parentContainer.SwitchChildVolumes(currentChild, targetVolume);
    }

    public void DisableLabel()
    {
        ToolTip[] tooltips = parentContainer.gameObject.GetComponentsInChildren<ToolTip>();

        foreach (var tooltip in tooltips)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void EnableLabel()
    {
        ToolTip[] tooltips = parentContainer.gameObject.GetComponentsInChildren<ToolTip>();

        foreach (var tooltip in tooltips)
        {
            tooltip.gameObject.SetActive(true);
        }
    }


    private void Update()
    {
        if (gameObject.transform.GetChild(0).GetComponent<Renderer>().bounds.Contains(newCube.transform.position))
        {
            if (newCube.transform.parent != parentContainer.transform)
            {
                newCube.transform.SetParent(parentContainer.transform, true);
            }
            
        }
    }






}
