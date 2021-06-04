using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutCopyPasteControls : MonoBehaviour
{
    public GameObject GameObjectClipboard;
    private bool instanitateNew;

    // Temp hack for a selected gameobject
    public GameObject FocusedGameObject;
    public Vector3 targetPosition;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Cut(FocusedGameObject);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Copy(FocusedGameObject);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Paste(targetPosition);
        }
    }

    public void Cut(GameObject target)
    {
        GameObjectClipboard = target;
        instanitateNew = false;
    }

    public void Copy(GameObject target)
    {
        GameObjectClipboard = target;
        instanitateNew = true;
    }

    public void Paste(Vector3 targetPostion)
    {
        var pastedObject = GameObjectClipboard;
        if (instanitateNew)
        {
            pastedObject = Instantiate(GameObjectClipboard);
        }
        pastedObject.transform.position = targetPosition;
    }
}
