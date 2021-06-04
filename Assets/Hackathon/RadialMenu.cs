using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    public List<string> menuOptions;
    public PrefabEmissionSystem prefabEmissionSystem;
    public List<RadialMenuElement> menuElements;

    public bool open;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && !open)
        {
            OpenMenu();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && open)
        {
            CloseMenu();
        }
    }

    private void OpenMenu()
    {
        if(menuElements == null || menuElements.Count == 0)
            menuElements = prefabEmissionSystem.Emit(menuOptions.Count).ToArray().Select(x => x.GetComponent<RadialMenuElement>()).ToList();

        foreach(var menuElement in menuElements)
        {
            menuElement.state = TransitionState.emitting;
        }
        open = true;
        //var i = 0;
        //foreach (string option in menuOptions)
        //{
        //    var menuElement = menuElements[i];
        //    menuElement.GetComponent<ToolTipConnector>().Target = this.gameObject;
        //    menuElement.GetComponent<ToolTip>().ToolTipText = option;
        //    i++;
        //}
    }
    private void CloseMenu()
    {
        foreach (var menuElement in menuElements)
        {
            menuElement.state = TransitionState.retracting;
        }
        open = false;
    }




}
