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

    //float maxDistance;
    float speed;
    bool hitDetect;

    protected Collider marqueeAreaCollider;
    protected RaycastHit hit;

    //private GameObject marquee = null;
    private Vector3 marqueeStartPosition;

    private Vector3 marqueeCenter;

    private LineRenderer lineRenderer;

    private Vector3 boxHalfVolume;

    private Vector3 topRight;

    private Vector3 bottomLeft;

    public Vector3 CurrentThumbPosition
    {
        get
        {
            MixedRealityPose thumb;
            HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumb);
            return thumb.Position;
        }
    }

    private bool ThumbsUp()
    {
        MixedRealityPose indexTip;
        MixedRealityPose middleTip;
        MixedRealityPose ringTip;
        MixedRealityPose pinkyTip;
        MixedRealityPose palm;
        MixedRealityPose thumbTip;

        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out ringTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.PinkyTip, Handedness.Right, out pinkyTip);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palm);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbTip);

        float indexPalmDistance = Vector3.Distance(palm.Position, indexTip.Position);
        float middlePalmDistance = Vector3.Distance(palm.Position, middleTip.Position);
        float ringPalmDistance = Vector3.Distance(palm.Position, ringTip.Position);
        float pinkyPalmDistance = Vector3.Distance(palm.Position, pinkyTip.Position);
        float thumbPalmDistance = Vector3.Distance(palm.Position, thumbTip.Position);


        if (indexPalmDistance < 0.06f && middlePalmDistance < 0.06f && ringPalmDistance < 0.06f && pinkyPalmDistance < 0.06f && thumbPalmDistance > 0.1f)
        {
            return true;
        }
        return false;
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
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

    void Start()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);

        speed = 20.0f;
    }

    private void StartSelectionDraw()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = true;

        lineRenderer.positionCount = 5;

        marqueeStartPosition = CurrentThumbPosition;

        lineRenderer.SetPosition(0, marqueeStartPosition);

        isSelectionDrawing = true;
    }

    private void UpdateSelectionDraw()
    {
        topRight = CurrentThumbPosition + (Vector3.up * (marqueeStartPosition.y - CurrentThumbPosition.y));

        bottomLeft = marqueeStartPosition + (Vector3.down * (marqueeStartPosition.y - CurrentThumbPosition.y));

        lineRenderer.SetPosition(1, topRight);

        lineRenderer.SetPosition(2, CurrentThumbPosition);

        lineRenderer.SetPosition(3, bottomLeft);

        lineRenderer.SetPosition(4, marqueeStartPosition);

        marqueeCenter = (marqueeStartPosition + CurrentThumbPosition) / 2;

        float xExtents = Mathf.Abs(marqueeCenter.x - CurrentThumbPosition.x);
        float yExtents = Mathf.Abs(marqueeCenter.y - marqueeStartPosition.y);
        float zExtents = 0.5f;

        boxHalfVolume = new Vector3(xExtents, yExtents, zExtents);

        StartBoxCast(marqueeCenter, boxHalfVolume);
    }

    private void StopSelectionDraw()
    {
        lineRenderer.enabled = false;
        isSelectionDrawing = false;
        isObjectSelected = false;
    }

    private void StartBoxCast(Vector3 marqueeCenter, Vector3 halfScaleOfBox)
    {
        //Simple movement in x and z axes
        float xAxis = Input.GetAxis("Horizontal") * speed;
        float zAxis = Input.GetAxis("Vertical") * speed;
        transform.Translate(new Vector3(xAxis, 0, zAxis));

        hitDetect = Physics.BoxCast(marqueeCenter, halfScaleOfBox, CameraCache.Main.transform.forward, out hit);

        if (hitDetect)
        {
            //Output the name of the Collider your Box hit
            SetSelectedState(hit.collider.gameObject);
            Debug.Log("Hit : " + hit.collider.name);
        }
    }

    private void SetSelectedState(GameObject go)
    {
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
