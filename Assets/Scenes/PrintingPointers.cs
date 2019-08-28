using Boo.Lang;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Security.Policy;

public class PrintingPointers : MonoBehaviour, IMixedRealityPointerHandler
{
    [Header("Input Source")]
    public string inputSourceStr = "null";

    [Header("GameObject in Focus")]
    public string gameObjectFocus = "null";

    public string pointersInScene = "null";

    [Header("Near Pointers")]
    public bool nearPointer = false;

    [Space(10)]
    public bool shellHandRayPointer = false;
    public bool shellResult = false;

    [Space(10)]
    public bool pokePointer = false;
    public bool pokeResult = false;

    [Space(10)]
    public bool grabPointer = false;
    public bool grabResult = false;

    [Space(10)]
    [Header("Far Pointers")]
    public bool linePointer = false;
    public bool lineResult = false;

    public bool ggvPointer = false;
    public bool ggvResult = false;


    
    public void OnPointerDown(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    private void ListPointers()
    {
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == Microsoft.MixedReality.Toolkit.Input.InputSourceType.Hand)
            {
                inputSourceStr = "Hand Input";

                //int count = 0;

                System.Collections.Generic.List<string> pointersInScene = new System.Collections.Generic.List<string>();

                foreach (var p in source.Pointers)
                {
                    // Default Controller Pointer is Shell Hand Ray Pointer - not null
                    // Poke Pointer - not null
                    // Grab Pointer - null

                    //pointersInScene += count + " " + p.PointerName + " ";

                    pointersInScene.Add(p.PointerName);

                    if (p.Result != null)
                    {
                        var hitObject = p.Result.Details.Object;
                        if (hitObject)
                        {
                            gameObjectFocus = p.Result.CurrentPointerTarget.name;

                        }

                        // Near Pointers
                        if (p is ShellHandRayPointer) //Default
                        {
                            shellHandRayPointer = true; // is shell hand ray pointer in scene
                            shellResult = true; // does shell hand ray pointer return a result
                        }

                        if (p is PokePointer)
                        {
                            pokePointer = true;
                            pokeResult = true;

                            var poke = p as PokePointer;
                            UnityEngine.Debug.Log("Poke Pointer isNearObject: " + poke.IsNearObject);
                        }

                        if (p is SpherePointer)
                        {
                            grabPointer = true;
                            grabResult = true;
                        }

                        // Far Pointers
                        if (p is LinePointer)
                        {
                            linePointer = true;
                            lineResult = true;
                        }

                        if (p is GGVPointer)
                        {
                            ggvPointer = true;
                            ggvResult = true;
                        }
                    }
                }                
            }
            else
            {
                inputSourceStr = "Other";
                nearPointer = false;

                pokePointer = false;
                pokeResult = false;

                shellHandRayPointer = false;
                shellResult = false;

                grabPointer = false;
                grabResult = false;

                linePointer = false;
                lineResult = false;

                ggvPointer = false;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ListPointers();
    }

}
