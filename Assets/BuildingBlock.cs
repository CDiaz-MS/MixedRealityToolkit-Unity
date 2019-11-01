using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    // Start is called before the first frame update

    private Transform pos;

    void Start()
    {
        pos = gameObject.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateBlock()
    {
        Instantiate(gameObject, pos.position, pos.rotation);
    }


}
