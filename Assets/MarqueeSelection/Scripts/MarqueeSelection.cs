using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class MarqueeSelection : MonoBehaviour, IMixedRealityPointerHandler
{
    //private bool isThumbsUp = false;

    //public bool IsThumbsUp
    //{
    //    get
    //    {
    //        var ThumbsUpGesturePose = ArticulatedHandPose.GetGesturePose(ArticulatedHandPose.GestureId.ThumbsUp);
    //        if(ThumbsUpGesturePose = )
    //        {
    //            foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
    //            {
                    
    //            }

    //        }
    //        // get the tracking data for the hand to see if it is in the thumbs up gesture
    //        //return true if in thumbs up state
    //        // return false if not

    //    }
    //}

    protected bool isSelectionDrawing = false;

    protected Vector3 ThumbPosition;

    private GameObject selectionIndicator;

    private Vector3 selectionIndicatorCurrentPosition = Vector3.zero;


    float maxDistance;
    float speed;
    bool hitDetect;

    Collider selectionAreaCollider;
    RaycastHit hit;

    public Vector3 SelectionIndicatorCurrentPosition
    {
        get
        {
            HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose ThumbPose);
            Vector3 pos = ThumbPose.Position + (ThumbPose.Forward * 0.01f);
            return pos;
        }
        set
        {

            selectionIndicator.transform.position = value;
            //selectionIndicatorPosition = value;
        }
    }

    private Vector3 selectionIndicatorStartPosition = Vector3.zero;

    private GameObject selectionArea;

    // Start is called before the first frame update
    void Start()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

        selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        selectionIndicator.transform.localScale = Vector3.one * 0.01f;

        maxDistance = 300.0f;
        speed = 20.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCubePosition();
    }

    private void UpdateCubePosition()
    {
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose ThumbPose);
        SelectionIndicatorCurrentPosition = ThumbPose.Position + (ThumbPose.Forward * 0.01f);
        
    }

    private void StartSelectionDraw()
    {
        selectionArea.transform.position = SelectionIndicatorCurrentPosition;

        ChangeQuadScale();

    }


    private void ChangeQuadScale()
    {
        Vector3 distance = selectionIndicatorStartPosition - SelectionIndicatorCurrentPosition;
        float magnitude = distance.sqrMagnitude;

        selectionArea.transform.localScale = (Vector3.one * magnitude) * 2;
    }

    private void StartBoxCast()
    {
        //Simple movement in x and z axes
        float xAxis = Input.GetAxis("Horizontal") * speed;
        float zAxis = Input.GetAxis("Vertical") * speed;
        transform.Translate(new Vector3(xAxis, 0, zAxis));

        hitDetect = Physics.BoxCast(selectionAreaCollider.bounds.center, selectionArea.transform.localScale, selectionArea.transform.forward, out hit, selectionArea.transform.rotation, maxDistance);

        if (hitDetect)
        {
            //Output the name of the Collider your Box hit
            Debug.Log("Hit : " + hit.collider.name);
        }

    }


    #region Pointer Handler

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        // Get the start position for the quad
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose ThumbPose);
        selectionIndicatorStartPosition = ThumbPose.Position;

        // Create a quad, once the quad is created, resize it
        selectionArea = GameObject.CreatePrimitive(PrimitiveType.Quad);
        selectionIndicator.transform.localScale = Vector3.zero;

        selectionArea.transform.position = SelectionIndicatorCurrentPosition;
        selectionAreaCollider = selectionArea.GetComponent<Collider>();

        isSelectionDrawing = true;

    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        ChangeQuadScale();
        StartBoxCast();
    }

    void OnDrawGizmos()
    {
        if(Application.isPlaying && isSelectionDrawing)
        {
            //Check if there has been a hit yet
            if (hitDetect)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(selectionArea.transform.position, selectionArea.transform.forward * hit.distance);
                Gizmos.DrawWireCube(selectionArea.transform.position + (selectionArea.transform.forward * (hit.distance/2)), selectionArea.transform.localScale + new Vector3(0, 0, hit.distance));
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(selectionArea.transform.position, selectionArea.transform.forward * maxDistance);
                Gizmos.DrawWireCube(selectionArea.transform.position + selectionArea.transform.forward * maxDistance, selectionArea.transform.localScale);
            }
        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        // Do a box cast to return the objects it hit
        isSelectionDrawing = false;
    }
    #endregion
 
}
