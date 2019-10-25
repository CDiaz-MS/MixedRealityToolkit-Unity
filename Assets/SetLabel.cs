using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLabel : MonoBehaviour
{
    public string LabelButton;
    private void OnValidate()
    {
        var tmp = GetComponentInChildren<TextMesh>();
        tmp.text = LabelButton;
        
    }
}
