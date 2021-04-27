// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDetection : MonoBehaviour
{ 
    public UIVolume TargetContainer;

    public UIVolumeGrid MiniVolume;

    public UIVolume PianoVolume;
    public UIVolume ManipVolume;
    public UIVolume LunarVolume;

    private UIVolume volume;

    private ObjectManipulator objectManipulator;

    private bool checkOverlap = false;

    void Start()
    {
        volume = gameObject.GetComponent<UIVolume>();

        objectManipulator = gameObject.GetComponent<ObjectManipulator>();

        objectManipulator.OnManipulationStarted.AddListener((data) => 
        {
            checkOverlap = true;
        });

        objectManipulator.OnManipulationEnded.AddListener((data) =>
        {
            checkOverlap = false;
        });
    }

    private void CheckForOverlap()
    {
        if (volume == PianoVolume)
        {
            TargetContainer.CheckExternalVolumeEntered(volume);
            TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);
        }

        if (volume == ManipVolume)
        {
            if (TargetContainer.CheckExternalVolumeEntered(volume))
            {
                TargetContainer.SwitchChildVolumes(PianoVolume, MiniVolume);
            }

            TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);

        }

        if (volume == LunarVolume)
        {
            if (TargetContainer.CheckExternalVolumeEntered(volume))
            {
                TargetContainer.SwitchChildVolumes(ManipVolume, MiniVolume);
            }

            TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);

        }

        //TargetContainer.CheckExternalVolumeEntered(volume);
        //TargetContainer.CheckInternalVolumeExited(volume, TargetContainer.transform.root.parent);
    }

    void Update()
    {
        if (checkOverlap)
        {
            CheckForOverlap();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (volume == LunarVolume)
            {
                TargetContainer.SwitchChildVolumes(volume, MiniVolume);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            MiniVolume.SwitchChildVolumes(volume, TargetContainer);
        }
    }
}
