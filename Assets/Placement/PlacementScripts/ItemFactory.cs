// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;

    public BaseInteractiveElement interactiveElement1;
    public BaseInteractiveElement interactiveElement2;
    public BaseInteractiveElement interactiveElement3;

    void Start()
    {
        ClickedEvents click1 = interactiveElement1.GetStateEvents<ClickedEvents>("Clicked");
        click1.OnClicked.AddListener(() => Instantiate(prefab1));

        ClickedEvents click2 = interactiveElement2.GetStateEvents<ClickedEvents>("Clicked");
        click2.OnClicked.AddListener(() => Instantiate(prefab2));

        ClickedEvents click3 = interactiveElement3.GetStateEvents<ClickedEvents>("Clicked");
        click3.OnClicked.AddListener(() => Instantiate(prefab3));
    }
}
