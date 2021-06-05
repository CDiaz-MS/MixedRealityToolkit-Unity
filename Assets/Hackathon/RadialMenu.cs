using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    public List<string> menuOptions;
    public PrefabEmissionSystem prefabEmissionSystem;
    public RadialMenuElement[] menuElements;

    public MenuSelectedEvent OnMenuItemSelected = new MenuSelectedEvent();

    [SerializeField]
    private bool open = false;

    public bool Open
    {
        get => open;
        set
        {
            open = value;

            if (open)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

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
        //if(menuElements == null || menuElements.Length == 0)
        //{
        //    menuElements = prefabEmissionSystem.EmitFromEmissionSystemSource(menuOptions.Count, transform.position).ToArray().Select(x => x.GetComponent<RadialMenuElement>()).ToArray();
        //}
        //else
        //{
        //    prefabEmissionSystem.EmitExisting(menuElements.Select(x => x.gameObject).ToArray(), transform.position, transform);
        //}

        foreach (var menuElement in menuElements)
        {
            menuElement.Emit();
        }
        open = true;
    }

    private void CloseMenu()
    {
        foreach (var menuElement in menuElements)
        {
            menuElement.Retract();
        }
        open = false;
    }

    public void InitSubMenuElements()
    {
        int count = menuElements.Length;
        for (int i = 0; i < menuElements.Length; i++)
        {
            var obj = menuElements[i];
            obj.menuElementOrigin = transform;
        }
    }

    public void SetMenuItemVisibility(bool isVisible)
    {
        foreach (var menuElement in menuElements)
        {
            menuElement.gameObject.SetActive(isVisible);
        }
    }
}

[CustomEditor(typeof(RadialMenu))]
public class ActionSelectionMenuInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetRadialMenu = (target as RadialMenu);
        if (GUILayout.Button("Init MenuElements"))
        {
            var menuElementArray = serializedObject.FindProperty("menuElements");
            menuElementArray.arraySize = 0;
            serializedObject.ApplyModifiedProperties();

            foreach (Transform obj in targetRadialMenu.transform)
            {
                RadialMenuElement elem = obj.gameObject.GetComponent<RadialMenuElement>();
                if (elem != null)
                {
                    menuElementArray.arraySize++;
                    serializedObject.ApplyModifiedProperties();

                    menuElementArray.GetArrayElementAtIndex(menuElementArray.arraySize - 1).objectReferenceValue = elem;

                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        targetRadialMenu.InitSubMenuElements();
    }
}
