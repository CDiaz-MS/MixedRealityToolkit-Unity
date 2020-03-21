using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityPhysics = UnityEngine.Physics;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class MarqueeSelection : MonoBehaviour,  IMixedRealityHandJointHandler
{
    #region original
    protected internal bool IsThumbsUp => ThumbsUp();

    protected bool isSelectionDrawing = false;

    protected bool hasStarted = false;

    protected bool isObjectSelected = false;

    [SerializeField]
    [Tooltip("This event is triggered when the hand is notin the Thumbs Up Gesture.")]
    private UnityEvent onObjectSelected = new UnityEvent();

    /// <summary>
    /// This event is triggered once when the game object to place is unselected, placed.
    /// </summary>
    public UnityEvent OnObjectSelected
    {
        get => onObjectSelected;
        set => onObjectSelected = value;
    }

    // BoxCast hit
    protected RaycastHit hit;

    bool hitDetect;

    // Corner points of the marquee
    private Vector3 marqueeStartPosition;
    private Vector3 marqueeCenter;
    private Vector3 topRight;
    private Vector3 bottomLeft;

    private LineRenderer lineRenderer;

    private Vector3 boxHalfVolume;

    private Vector3[] marqueeCornerPositions = new Vector3[5];

    public Vector3 CurrentThumbPosition
    {
        get
        {
            HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose thumb);
            return thumb.Position;
        }
    }

    void Start()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    /// <summary>
    /// Checks if the hand is in the thumbs up gesture by measuring distances between joints
    /// </summary>
    private bool ThumbsUp()
    {
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose indexTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out MixedRealityPose middleTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out MixedRealityPose ringTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out MixedRealityPose pinkyTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose palm);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose thumbTip);

        float indexPalmDistance = Vector3.Distance(palm.Position, indexTip.Position);
        float middlePalmDistance = Vector3.Distance(palm.Position, middleTip.Position);
        float ringPalmDistance = Vector3.Distance(palm.Position, ringTip.Position);
        float pinkyPalmDistance = Vector3.Distance(palm.Position, pinkyTip.Position);
        float thumbPalmDistance = Vector3.Distance(palm.Position, thumbTip.Position);

        if (indexPalmDistance < 0.08f && middlePalmDistance < 0.08f && ringPalmDistance < 0.08f && pinkyPalmDistance < 0.08f && thumbPalmDistance > 0.1f)
        {
            return true;
        }

        return false;
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        // If the hand is in the thumbs up gesture
        if (IsThumbsUp)
        {
            if (!isSelectionDrawing)
            {
                StartSelectionDraw();                
            }
            else
            {
                UpdateSelectionDraw();
            }
        }
        else
        {
            if (isSelectionDrawing)
            {
                StopSelectionDraw();
            }                 
        }
    }


    /// <summary>
    /// Get enable the line renderer attached to the game object
    /// </summary>
    private void StartSelectionDraw()
    {
        // Get the line renderer
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = true;

        // The position count is 5 starting at the left top corner, top right corner, bottom right, bottom left
        // then the start position again 
        lineRenderer.positionCount = 5;

        marqueeStartPosition = CurrentThumbPosition;

        // Add starting position to linerenderer
        lineRenderer.SetPosition(0, marqueeStartPosition);

        isSelectionDrawing = true;
    }

    private void UpdateSelectionDraw()
    {
        // Calculate top right and bottom left positions based on the start position and current position 
        topRight = CurrentThumbPosition + (Vector3.up * (marqueeStartPosition.y - CurrentThumbPosition.y));
        bottomLeft = marqueeStartPosition + (Vector3.down * (marqueeStartPosition.y - CurrentThumbPosition.y));

        // Add points to line renderer
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, CurrentThumbPosition);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, marqueeStartPosition);

        // Calculate center of marquee for the box cast
        marqueeCenter = (marqueeStartPosition + CurrentThumbPosition) / 2;

        // Calculate half volume of the box based on the marquee size 
        float xExtents = Mathf.Abs(marqueeCenter.x - CurrentThumbPosition.x);
        float yExtents = Mathf.Abs(marqueeCenter.y - marqueeStartPosition.y);
        float zExtents = 0.5f;

        // For the box cast
        boxHalfVolume = new Vector3(xExtents, yExtents, zExtents);

        PerformBoxCast(marqueeCenter, boxHalfVolume);
    }

    private void StopSelectionDraw()
    {
        lineRenderer.enabled = false;
        isSelectionDrawing = false;
        isObjectSelected = false;
    }

    private void PerformBoxCast(Vector3 marqueeCenter, Vector3 halfScaleOfBox)
    {
        hitDetect = Physics.BoxCast(marqueeCenter, halfScaleOfBox, CameraCache.Main.transform.forward, out hit);

        if (hitDetect)
        {
            SelectObject(hit.collider.gameObject);
        }
    }

    private void SelectObject(GameObject go)
    {
        // Create event that passes a game object data instead of this 
        Material mat = go.GetComponent<MeshRenderer>().material;
        mat.color = Color.yellow;
        var tapToPlace = go.AddComponent<TapToPlace>();
        tapToPlace.AutoStart = true;

        if (!isObjectSelected)
        {
            OnObjectSelected.Invoke();
            isObjectSelected = true;
        }
    }

    void OnDrawGizmos()
    {
        int rayLength = 30;
        if (Application.isPlaying && isSelectionDrawing)
        { 
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(marqueeStartPosition, CameraCache.Main.transform.forward * rayLength);
            Gizmos.DrawRay(topRight, CameraCache.Main.transform.forward * rayLength);
            Gizmos.DrawRay(CurrentThumbPosition, CameraCache.Main.transform.forward * rayLength);
            Gizmos.DrawRay(bottomLeft, CameraCache.Main.transform.forward * rayLength);
        }
    }

    #endregion

}
