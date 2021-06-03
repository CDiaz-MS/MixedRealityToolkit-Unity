using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenu : MonoBehaviour
{
    public List<string> menuOptions;
    public PrefabEmissionSystem prefabEmissionSystem;
    public List<GameObject> menuElements;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        menuElements = new List<GameObject>(prefabEmissionSystem.Emit(menuOptions.Count));

        var i = 0;
        foreach (string option in menuOptions)
        {
            var menuElement = menuElements[i];
            menuElement.GetComponent<ToolTipConnector>().Target = this.gameObject;
            menuElement.GetComponent<ToolTip>().ToolTipText = option;
            i++;
        }
    }

}
