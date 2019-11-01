using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class HandCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MixedRealityPose palm = HandCollider();
        gameObject.transform.position = palm.Position;
        gameObject.transform.localRotation = palm.Rotation;
    }


    private MixedRealityPose HandCollider()
    {
        MixedRealityPose palm;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palm);
        return palm;
    }


}
