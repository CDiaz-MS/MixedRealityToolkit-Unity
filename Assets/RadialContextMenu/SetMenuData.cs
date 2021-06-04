using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMenuData : MonoBehaviour
{
    public RadialContextMenu Menu;

    private Material material;

    private Color purple = new Color(0.63f, 0.13f, 0.94f);
    private Color lightPurple = new Color(0.87f, 0.60f, 1.00f);
    private Color lightBlue = new Color(0.60f, 0.87f, 1.00f);
    private Color darkBlue = new Color(0.00f, 0.00f, 0.30f);
    private Color lightPink = new Color(1.00f, 0.60f, 0.93f);
    
    void Start()
    {
        material = gameObject.GetComponent<MeshRenderer>().material;

        Menu.OnMenuItemSelected.AddListener((menuItemSelected) =>
        {
            SetOptions(menuItemSelected.name);
        });     
    }

    private void SetOptions(string optionName)
    {
        if (optionName == "Blue")
        {
            material.color = Color.blue;
        }
        else if (optionName == "LightBlue")
        {
            material.color = lightBlue;
        }
        else if (optionName == "DarkBlue")
        {
            material.color = darkBlue;
        }
        else if (optionName == "Pink")
        {
            material.color = Color.magenta;
        }
        else if (optionName == "LightPink")
        {
            material.color = lightPink;
        }
        else if (optionName == "Purple")
        {
            material.color = purple;
        }
        else if (optionName == "LightPurple")
        {
            material.color = lightPurple;
        }
        else if (optionName == "Double")
        {
            transform.localScale = transform.localScale * 2;
        }
        else if (optionName == "Half")
        {
            transform.localScale = transform.localScale * 0.5f;
        }
    }
}
