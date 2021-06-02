using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;


public class RadialMenuItem
{
    public string Name;
    public List<RadialMenuItem> MenuItems = new List<RadialMenuItem>();
    public Bounds Bounds;
    public BoxCollider Collider;
    public ToolTip ToolTip;
    public Transform Transform;
    public bool AreChildrenVisible;

    public bool HasChildren()
    {
        return MenuItems.Count != 0;
    }

    public void SetSubMenuVisibility(bool isVisible)
    {
        MenuItems.ForEach((item) => item.Transform.gameObject.SetActive(isVisible));
        AreChildrenVisible = isVisible;
    }

    public void PrintSubMenus()
    {
        MenuItems.ForEach((item) => Debug.Log(item.Name));
    }
}


public class RadialContextMenu : MonoBehaviour
{ 
    private List<RadialMenuItem> menuItems = new List<RadialMenuItem>();

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
                    menuItems.Add(menuItem);
                }
                else
                {
                    item.MenuItems.Add(menuItem);
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
            menuItems[0].SetSubMenuVisibility(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            menuItems[0].MenuItems[0].SetSubMenuVisibility(true);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            menuItems[0].SetSubMenuVisibility(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            menuItems[0].MenuItems[0].SetSubMenuVisibility(false);
        }
    }

}
