using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class RayDrawing : MonoBehaviour
{
    void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            foreach (var source in CoreServices.InputSystem.DetectedInputSources)
            {
                if (source.SourceType == InputSourceType.Hand)
                {
                    MixedRealityPose indexFingerTip = GetIndexPosition();
                    Vector3 pos = GetIndexPosition().Position;
                    GetLinePointerDirection();

                    // FORWARD
                    Gizmos.color = Color.red;
                    Vector3 forwardIdx = indexFingerTip.Forward * 0.3f;
                    Gizmos.DrawRay(pos, forwardIdx);

                    // UP
                    Gizmos.color = Color.blue;
                    Vector3 upIdx = indexFingerTip.Up * 0.3f;
                    Gizmos.DrawRay(pos, upIdx);

                    // DOWN
                    Gizmos.color = Color.green;
                    Vector3 downIdx = indexFingerTip.Up * -0.3f;
                    Gizmos.DrawRay(pos, downIdx);

                    // RIGHT
                    Gizmos.color = Color.yellow;
                    Vector3 rightIdx = indexFingerTip.Right * 0.3f;
                    Gizmos.DrawRay(pos, rightIdx);

                    // LEFT
                    Gizmos.color = Color.black;
                    Vector3 leftIdx = indexFingerTip.Right * -0.3f;
                    Gizmos.DrawRay(pos, leftIdx);

                    // BACK
                    Gizmos.color = Color.cyan;
                    Vector3 backIdx = indexFingerTip.Forward * -0.3f;
                    Gizmos.DrawRay(pos, backIdx);
                }
            }
        }

        // FORWARD
        Gizmos.color = Color.red;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Gizmos.DrawRay(transform.position, forward);

        // UP
        Gizmos.color = Color.blue;
        Vector3 up = transform.TransformDirection(Vector3.up) * 10;
        Gizmos.DrawRay(transform.position, up);

        // DOWN
        Gizmos.color = Color.green;
        Vector3 down = transform.TransformDirection(Vector3.down) * 10;
        Gizmos.DrawRay(transform.position, down);

        // RIGHT
        Gizmos.color = Color.yellow;
        Vector3 right = transform.TransformDirection(Vector3.right) * 10;
        Gizmos.DrawRay(transform.position, right);

        // LEFT
        Gizmos.color = Color.black;
        Vector3 left = transform.TransformDirection(Vector3.left) * 10;
        Gizmos.DrawRay(transform.position, left);

        // BACK
        Gizmos.color = Color.cyan;
        Vector3 back = transform.TransformDirection(Vector3.back) * 10;
        Gizmos.DrawRay(transform.position, back);

    }

    public MixedRealityPose GetIndexPosition()
    {
        MixedRealityPose idx;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out idx);
        return idx;
    }

    public void GetLinePointerDirection()
    {
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            if (source.SourceType == InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is LinePointer)
                    {
                        var plp = p as LinePointer;
                        Debug.Log(plp.Rays[0].Direction);

                    }
                }
            }
        }

    }


}
