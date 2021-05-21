using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorWidgetItem : MonoBehaviour
{
    [SerializeField]
    private UIVolume uiVolume;

    public UIVolume UIVolume
    {
        get => uiVolume;
        set => uiVolume = value;
    }

    //[SerializeField]
    //private AnchorLocation anchorLocation;

    //public AnchorLocation AnchorLocation
    //{
    //    get => anchorLocation;
    //    set => anchorLocation = value;
    //}

    [SerializeField]
    private InteractiveElement interactiveElement;

    public InteractiveElement InteractiveElement
    {
        get => interactiveElement;
        set => interactiveElement = value;
    }

    [SerializeField]
    private Material activeMaterial;

    public Material ActiveMaterial
    {
        get => activeMaterial;
        set => activeMaterial = value;
    }

    [SerializeField]
    private Material inactiveMaterial;

    public Material InactiveMaterial
    {
        get => inactiveMaterial;
        set => inactiveMaterial = value;
    }

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = inactiveMaterial;

        TouchEvents touchEvents = InteractiveElement.GetStateEvents<TouchEvents>("Touch");

        touchEvents.OnTouchStarted.AddListener((touchData) =>
        {
            meshRenderer.sharedMaterial = activeMaterial;

            if (!UIVolume.UseAnchorPositioning)
            {
                UIVolume.UseAnchorPositioning = true;
            }

            //UIVolume.AnchorLocation = AnchorLocation;
        });

        touchEvents.OnTouchCompleted.AddListener((touchData) =>
        {
            meshRenderer.sharedMaterial = inactiveMaterial;
        });
    }
}
