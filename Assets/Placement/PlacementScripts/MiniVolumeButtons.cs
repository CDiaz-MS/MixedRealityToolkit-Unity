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

    public UIVolume TargetContainer;

    public UIVolume MiniVolume;

    public UIVolume PianoVolume;
    public UIVolume ManipVolume;
    public UIVolume LunarVolume;

    void Start()
    {
        var toggleOnPiano = pianoButton.GetStateEvents<ToggleOnEvents>("ToggleOn");
        var toggleOnManip = manipButton.GetStateEvents<ToggleOnEvents>("ToggleOn");
        var toggleOnLunar = lunarButton.GetStateEvents<ToggleOnEvents>("ToggleOn");

        toggleOnPiano.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(PianoVolume))
            {
                TargetContainer.SwitchChildVolumes(PianoVolume, MiniVolume);
            }
        });

        toggleOnManip.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(ManipVolume))
            {
                TargetContainer.SwitchChildVolumes(ManipVolume, MiniVolume);
            }
        });

        toggleOnLunar.OnToggleOn.AddListener(() =>
        {
            if (TargetContainer.ContainsVolume(LunarVolume))
            {
                TargetContainer.SwitchChildVolumes(LunarVolume, MiniVolume);
            }
        });

        var toggleOffPiano = pianoButton.GetStateEvents<ToggleOffEvents>("ToggleOff");
        var toggleOffManip = manipButton.GetStateEvents<ToggleOffEvents>("ToggleOff");
        var toggleOffLunar = lunarButton.GetStateEvents<ToggleOffEvents>("ToggleOff");

        toggleOffPiano.OnToggleOff.AddListener(() =>
        {
            if (MiniVolume.ContainsVolume(PianoVolume))
            {
                MiniVolume.SwitchChildVolumes(PianoVolume, TargetContainer);
            }
        });

        toggleOffManip.OnToggleOff.AddListener(() =>
        {
            if (MiniVolume.ContainsVolume(ManipVolume))
            {
                MiniVolume.SwitchChildVolumes(ManipVolume, TargetContainer);
            }
        });

        toggleOffLunar.OnToggleOff.AddListener(() =>
        {
            if (MiniVolume.ContainsVolume(LunarVolume))
            {
                MiniVolume.SwitchChildVolumes(LunarVolume, TargetContainer);
            }
        });

    }
}
