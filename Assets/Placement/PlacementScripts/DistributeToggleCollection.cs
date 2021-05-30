﻿using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.UI.Layout;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DistributeToggleCollection : MonoBehaviour
{
    [SerializeField]
    private Volume uiVolume;

    public Volume UIVolume
    {
        get => uiVolume;
        set => uiVolume = value;
    }

    [SerializeField]
    private List<BaseInteractiveElement> interactiveElementToggleList = new List<BaseInteractiveElement>();

    public List<BaseInteractiveElement> InteractiveElementToggleList
    {
        get => interactiveElementToggleList;
        set => interactiveElementToggleList = value;
    }

    private void Start()
    {
        AddToggleListeners();
    }

    private void AddToggleListeners()
    {
        for (int i = 0; i < InteractiveElementToggleList.Count; i++)
        {
            ToggleOnEvents toggleOn = InteractiveElementToggleList[i].GetStateEvents<ToggleOnEvents>("ToggleOn");

            if (i == 0)
            {
                toggleOn.OnToggleOn.AddListener(() =>
                {
                    //Volume.XAxisDynamicDistribute = true;
                    //Volume.YAxisDynamicDistribute = false;
                    //Volume.ZAxisDynamicDistribute = false;


                    // Turn off the other toggles
                    InteractiveElementToggleList[1].ForceSetToggleStates(false);
                    InteractiveElementToggleList[2].ForceSetToggleStates(false);
                });
            }
            else if (i == 1)
            {
                toggleOn.OnToggleOn.AddListener(() =>
                {                   
                    //Volume.YAxisDynamicDistribute = true;
                    //Volume.ZAxisDynamicDistribute = false;
                    //Volume.XAxisDynamicDistribute = false;

                    InteractiveElementToggleList[0].ForceSetToggleStates(false);
                    InteractiveElementToggleList[2].ForceSetToggleStates(false);

                });
            }
            else if (i == 2)
            {
                toggleOn.OnToggleOn.AddListener(() =>
                {
                    //Volume.ZAxisDynamicDistribute = true;
                    //Volume.XAxisDynamicDistribute = false;
                    //Volume.YAxisDynamicDistribute = false;

                    InteractiveElementToggleList[0].ForceSetToggleStates(false);
                    InteractiveElementToggleList[1].ForceSetToggleStates(false);
                });
            }
        }
    }
}