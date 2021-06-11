// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniVolumeButtons : MonoBehaviour
{
    public CompressableButton pianoButton;
    public CompressableButton manipButton;
    public CompressableButton lunarButton;

    public BaseVolume TargetContainer;

    public VolumeGrid MiniVolume;

    public BaseVolume PianoVolume;
    public BaseVolume ManipVolume;
    public BaseVolume LunarVolume;

    public BaseVolume PianoContainer;
    public BaseVolume ManipContainer;
    public BaseVolume LunarContainer;

    void Start()
    {
        var toggleOnPiano = pianoButton.GetStateEvents<ToggleOnEvents>("ToggleOn");
        var toggleOnManip = manipButton.GetStateEvents<ToggleOnEvents>("ToggleOn");
        var toggleOnLunar = lunarButton.GetStateEvents<ToggleOnEvents>("ToggleOn");

        toggleOnPiano.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(PianoVolume))
            {
                TargetContainer.SwitchChildVolumes(PianoVolume, PianoContainer);
            }
        });

        toggleOnManip.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(ManipVolume))
            {
                TargetContainer.SwitchChildVolumes(ManipVolume, ManipContainer);
            }
        });

        toggleOnLunar.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(LunarVolume))
            {
                TargetContainer.SwitchChildVolumes(LunarVolume, LunarContainer);
            }
        });

        var toggleOffPiano = pianoButton.GetStateEvents<ToggleOffEvents>("ToggleOff");
        var toggleOffManip = manipButton.GetStateEvents<ToggleOffEvents>("ToggleOff");
        var toggleOffLunar = lunarButton.GetStateEvents<ToggleOffEvents>("ToggleOff");

        toggleOffPiano.OnToggleOff.AddListener(() =>
        {
            if (PianoContainer.ContainsVolume(PianoVolume))
            {
                if (CanVolumeFit(0.25f))
                {
                    PianoContainer.SwitchChildVolumes(PianoVolume, TargetContainer);
                }
            }
        });

        toggleOffManip.OnToggleOff.AddListener(() =>
        {
            if (ManipContainer.ContainsVolume(ManipVolume))
            {
                if (CanVolumeFit(0.32f))
                {
                    ManipContainer.SwitchChildVolumes(ManipVolume, TargetContainer);
                }
              
            }
        });

        toggleOffLunar.OnToggleOff.AddListener(() =>
        {
            if (LunarContainer.ContainsVolume(LunarVolume))
            {
                if (CanVolumeFit(0.16f))
                {
                    LunarContainer.SwitchChildVolumes(LunarVolume, TargetContainer);
                }
            }
        });

    }


    private bool CanVolumeFit(float space)
    {
        float targetWidth = TargetContainer.VolumeSize.x;
        float occupiedSpaceRatio = (TargetContainer.GetOccupiedSpace(VolumeAxis.X) + space) / targetWidth;

        if (occupiedSpaceRatio < 0.8f)
        {
            return true;
        }

        return false;
    }


}
