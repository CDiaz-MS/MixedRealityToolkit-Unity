using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformVectorTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       

        Debug.Log("World Position: " + transform.position.ToString("F4"));
        Debug.Log("Local Position: " + transform.localPosition.ToString("F4"));

        Debug.Log(transform.TransformVector(transform.localPosition));



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
