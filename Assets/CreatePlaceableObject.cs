using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class CreatePlaceableObject : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 0, 0.6f);
        cube.transform.localScale = Vector3.one * 0.1f;
        var tapToPlace = cube.AddComponent<TapToPlace>();
    }
}
