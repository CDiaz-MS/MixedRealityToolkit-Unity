using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetText : MonoBehaviour
{
    public string labelText;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnValidate()
    {
        var textMesh = gameObject.GetComponentInChildren<TextMesh>();

        if(textMesh != null)
        {
            textMesh.text = labelText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
