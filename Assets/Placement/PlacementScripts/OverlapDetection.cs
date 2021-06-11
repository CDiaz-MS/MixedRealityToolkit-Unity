// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDetection : MonoBehaviour
{ 
    public VolumeGrid TargetContainer;

    public VolumeGrid MiniVolume;

    public BaseVolume PianoVolume;
    public BaseVolume ManipVolume;
    public BaseVolume LunarVolume;

    public BaseVolume PianoContainer;
    public BaseVolume ManipContainer;
    public BaseVolume LunarContainer;

    public GameObject BoundaryVisual;

    private BaseVolume volume;

    public ObjectManipulator objectManipulator;

    public float SpacePercentage = 0.8f;

    private bool checkOverlap = false;

    private float currentRemainingSpace;
    void Start()
    {
        volume = gameObject.GetComponent<BaseVolume>();

        //objectManipulator = gameObject.GetComponent<ObjectManipulator>();

        objectManipulator.OnManipulationStarted.AddListener((data) => 
        {
            checkOverlap = true;
            BoundaryVisual.GetComponent<MeshRenderer>().enabled = true;
        });

        objectManipulator.OnManipulationEnded.AddListener((data) =>
        {
            checkOverlap = false;
            BoundaryVisual.GetComponent<MeshRenderer>().enabled = false;
        });
    }

    private void CheckForOverlap()
    {
        //if (volume == PianoVolume)
        //{
        //    TargetContainer.CheckExternalVolumeEntered(volume);
        //    TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);
        //}

        //if (volume == ManipVolume)
        //{
        //    if (TargetContainer.CheckExternalVolumeEntered(volume))
        //    {
        //        TargetContainer.SwitchChildVolumes(PianoVolume, MiniVolume);
        //    }

        //    TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);

        //}

        //if (volume == LunarVolume)
        //{
        //    if (TargetContainer.CheckExternalVolumeEntered(volume))
        //    {
        //        TargetContainer.SwitchChildVolumes(ManipVolume, MiniVolume);
        //    }

        //    TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);

        //}

        TargetContainer.Volume.CheckExternalVolumeEntered(volume);
        TargetContainer.Volume.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);
    }

    void Update()
    {
        if (checkOverlap)
        {
            CheckForOverlap();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (volume == PianoVolume)
            {
                TargetContainer.Volume.SwitchChildVolumes(volume, PianoContainer);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (volume == PianoVolume)
            {
                PianoContainer.SwitchChildVolumes(volume, TargetContainer.Volume);
            }
        }

        GetRemainingSpace();
    }

    private void GetRemainingSpace()
    {
        float targetWidth = TargetContainer.Volume.VolumeSize.x;

        float remainingSpaceRatio = TargetContainer.Volume.GetOccupiedSpace(VolumeAxis.X) / targetWidth;

        Debug.Log(remainingSpaceRatio);

        // If over 85% is taken up, then move the apps to the smaller volume
        if (remainingSpaceRatio > SpacePercentage)
        {
            Vector3 coordinates = new Vector3(1, TargetContainer.ChildVolumeItems.Count, 1);

            GameObject vol = TargetContainer.GetObjectAtCoordinates(coordinates);

            BaseVolume lastVolume = vol.GetComponent<BaseVolume>();

            TargetContainer.Volume.SwitchChildVolumes(lastVolume, GetContainer(lastVolume));
        }
    }

    private BaseVolume GetContainer(BaseVolume volume)
    {
        if (volume == PianoVolume)
        {
            return PianoContainer;
        }
        else if (volume == ManipVolume)
        {
            return ManipContainer;
        }
        else
        {
            return LunarContainer;
        }
    }

}
