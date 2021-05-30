// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchVolumes : MonoBehaviour
{
    public Volume volume;

    public Volume child1;
    public Volume child2;
    public Volume child3;

    public Volume target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (volume.GetChildUIVolumes().Contains(child1))
            {
                volume.SwitchChildVolumes(child1, target);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (target.GetChildUIVolumes().Contains(child1))
            {
                target.SwitchChildVolumes(child1, volume);
            }
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            if (volume.GetChildUIVolumes().Contains(child2))
            {
                volume.SwitchChildVolumes(child2, target);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (target.GetChildUIVolumes().Contains(child2))
            {
                target.SwitchChildVolumes(child2, volume);
            }
        }


        if (Input.GetKeyDown(KeyCode.N))
        {
            if (volume.GetChildUIVolumes().Contains(child3))
            {
                volume.SwitchChildVolumes(child3, target);
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (target.GetChildUIVolumes().Contains(child3))
            {
                target.SwitchChildVolumes(child3, volume);
            }
        }
    }
}
