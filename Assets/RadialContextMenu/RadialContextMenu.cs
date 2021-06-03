using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class RadialMenuItem
{
    public string Name;
    public Dictionary<string, RadialMenuItem> MenuItems = new Dictionary<string, RadialMenuItem>();
    public Bounds Bounds;
    public BoxCollider Collider;
    public ToolTip ToolTip;
    public Transform Transform;
    public bool AreChildrenVisible;
    public bool IsMenuItemActive => Transform.gameObject.activeSelf;

    public bool HasChildren()
    {
        return MenuItems.Count != 0;
    }

    public void SetSubMenuVisibility(bool isVisible)
    {
        foreach (RadialMenuItem item in MenuItems.Values)
        {
           item.Transform.gameObject.SetActive(isVisible);
        }

        AreChildrenVisible = isVisible;
    }

    public void PrintSubMenus()
    {
        foreach (string name in MenuItems.Keys)
        {
            Debug.Log(name);
        }
    }
}


public class RadialContextMenu : MonoBehaviour
{ 
    private Dictionary<string, RadialMenuItem> menuItems = new Dictionary<string, RadialMenuItem>();

    [SerializeField]
    private Transform rootTransform;

    public Transform RootTransform
    {
        get => rootTransform;
        set => rootTransform = value;
    }

    private void Start()
    {
        SetMenuItems(transform, null);
    }

    private void SetMenuItems(Transform currentTransform, RadialMenuItem item)
    {
        for (int i = 0; i < currentTransform.childCount; i++)
        {
            Transform menuItemTransform = currentTransform.GetChild(i);
            ToolTip toolTip = menuItemTransform.GetComponent<ToolTip>();

            if (toolTip != null)
            {
                ToolTipBackgroundMesh toolTipBackground = menuItemTransform.GetComponent<ToolTipBackgroundMesh>();
                RadialMenuItem menuItem = new RadialMenuItem()
                {
                    Name = menuItemTransform.gameObject.name,
                    Transform = menuItemTransform,
                    ToolTip = toolTip,
                    Bounds = toolTipBackground.BackgroundRenderer.bounds,
                    Collider = toolTipBackground.BackgroundTransform.GetComponent<BoxCollider>()
                };

                if (menuItemTransform.parent == RootTransform)
                {
                    menuItems.Add(menuItem.Name, menuItem);
                }
                else
                {
                    item.MenuItems.Add(menuItem.Name, menuItem);
                }

                // Go to next item in the hierarchy if it has children
                if (menuItemTransform.childCount > 0)
                {
                    SetMenuItems(menuItemTransform, menuItem);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            menuItems["Color"].SetSubMenuVisibility(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            menuItems["Color"].MenuItems["Blue"].SetSubMenuVisibility(true);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            menuItems["Color"].SetSubMenuVisibility(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            menuItems["Color"].MenuItems["Blue"].SetSubMenuVisibility(false);
        }
    }
}
