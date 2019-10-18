using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class CreatePressableCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var pressableButton = gameObject.AddComponent<PressableButton>();
        pressableButton.

        gameObject.AddComponent<NearInteractionTouchable>();


        var nearTouchableSurface = gameObject.GetComponent<NearInteractionTouchableSurface>();
        if (nearTouchableSurface == null)
        {
            Debug.Log("NULLLL");

        }
        else
        {
            Debug.Log("Got ittt");
        }
  
    }


    public void CreateCube()
    {
        GameObject cubePressable = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubePressable.transform.position = gameObject.transform.position + new Vector3(0.15f, 0, 0);
        cubePressable.transform.localScale = gameObject.transform.localScale;

        cubePressable.AddComponent<PressableButton>();
        cubePressable.AddComponent<NearInteractionTouchable>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
