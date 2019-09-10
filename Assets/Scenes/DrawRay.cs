using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DrawRay : MonoBehaviour
{
    public Vector3 direction;


    /// <summary>
    /// Draw a ray with a given direction from the position of the current game object
    /// </summary>
    /// <returns></returns>
    /// 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 ray = transform.TransformDirection(direction) * 10;
        Gizmos.DrawRay(transform.position, ray);
    }
}
