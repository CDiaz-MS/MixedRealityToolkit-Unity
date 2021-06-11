// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class BaseVolume : MonoBehaviour
    {
        [SerializeField]
        private int volumeID;

        /// <summary>
        /// ID for this Volume
        /// </summary>
        public int VolumeID
        {
            get => volumeID;
        }

        [SerializeField]
        private VolumeSizeOrigin volumeSizeOrigin = VolumeSizeOrigin.LossyScale;

        /// <summary>
        /// The sizing entry point for this Volume.
        /// </summary>
        public VolumeSizeOrigin VolumeSizeOrigin
        {
            get => volumeSizeOrigin;
            set
            {
                volumeSizeOrigin = value;
                OnVolumeModified.Invoke();
            }
        }

        [SerializeField]
        private VolumeBounds volumeBounds = new VolumeBounds(Vector3.one,Vector3.zero, null);

        /// <summary>
        /// 
        /// </summary>
        public VolumeBounds VolumeBounds 
        {
            get => volumeBounds;
            set => volumeBounds = value;
        }

        [SerializeField]
        private VolumeBounds marginBounds = new VolumeBounds(Vector3.zero, Vector3.zero, null);

        /// <summary>
        /// 
        /// </summary>
        public VolumeBounds MarginBounds
        {
            // The margin bounds are going to be the volume bounds += 
            get => marginBounds;
            set => marginBounds = value;
        }

        /// <summary>
        /// The size of the Volume.
        /// </summary>
        public Vector3 VolumeSize
        {
            get => VolumeBounds.Size;
            set
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale || VolumeSizeOrigin == VolumeSizeOrigin.LocalScale || VolumeSizeOrigin == VolumeSizeOrigin.RendererBounds)
                {
                    Debug.LogError("The Volume Size can only be set if the VolumeSizeOrigin is Custom");
                }
                else
                {
                    VolumeBounds.Size = value;
                }
            } 
        }

        /// <summary>
        /// The center of the VolumeBounds.
        /// </summary>
        public Vector3 VolumeCenter
        {
            get => volumeBounds.Center;
            set => volumeBounds.Center = value;
        }

        /// <summary>
        /// The position of this volume.
        /// </summary>
        public Vector3 VolumePosition
        {
            get => transform.position;
            set => transform.position = value;
        }

        /// <summary>
        /// List of the corner point world positions of the VolumeBounds
        /// 
        /// [0] == LeftBottomForward
        /// [1] == LeftBottomBack
        /// [2] == LeftTopForward
        /// [3] == LeftTopBack
        /// [4] == RightBottomForward
        /// [5] == RightBottomBack
        /// [6] == RightTopForward
        /// [7] == RightTopBack
        /// </summary>
        public Vector3[] VolumeCorners
        {
            get => VolumeBounds.GetCornerPositions();
        }

        /// <summary>
        /// List of the corner point world positions of the VolumeBounds
        /// 
        /// [0] == Top
        /// [1] == Bottom
        /// [2] == Left
        /// [3] == Right
        /// [4] == Back
        /// [5] == Forward
        /// </summary>
        public Vector3[] VolumeFaces
        {
            get => VolumeBounds.GetFacePositions();
        }

        #region Gizmo Properties

        [SerializeField]
        [Tooltip("Draw the corner points of a volume.")]
        private bool drawCornerPoints = false;

        /// <summary>
        /// Draw the corner points of a volume.
        /// </summary>
        public bool DrawCornerPoints
        {
            get => drawCornerPoints;
            set => drawCornerPoints = value;
        }

        [SerializeField]
        [Tooltip("Draw the face points of a volume.")]
        private bool drawFacePoints = false;

        /// <summary>
        /// Draw the corner points of a volume.
        /// </summary>
        public bool DrawFacePoints
        {
            get => drawFacePoints;
            set => drawFacePoints = value;
        }

        [SerializeField]
        [Tooltip("Draw the VolumeBounds.")]
        private bool drawVolumeBounds = true;

        /// <summary>
        /// Draw the VolumeBounds.
        /// </summary>
        public bool DrawVolumeBounds
        {
            get => drawVolumeBounds;
            set => drawVolumeBounds = value;
        }

        [SerializeField]
        [Tooltip("Draw the MarginBounds.")]
        private bool drawMarginBounds = false;

        /// <summary>
        /// Draw the VolumeBounds.
        /// </summary>
        public bool DrawMarginBounds
        {
            get => drawMarginBounds;
            set => drawMarginBounds = value;
        }

        #endregion

        [SerializeField]
        [Tooltip("Draw the MarginBounds.")]
        private bool useMargin = false;

        /// <summary>
        /// Draw the VolumeBounds.
        /// </summary>
        public bool UseMargin
        {
            get => useMargin;
            set => useMargin = value;
        }

        [SerializeField]
        private List<ChildVolumeItem> childVolumeItems = new List<ChildVolumeItem>();

        public List<ChildVolumeItem> ChildVolumeItems
        {
            get => childVolumeItems;
            private set => childVolumeItems = value;
        }

        public List<BaseVolume> DirectChildVolumes
        {
            get => GetChildVolumes();
        }

        [SerializeField]
        private int childVolumeCount => ChildVolumeItems.Count;

        protected string[] cornerNames = Enum.GetNames(typeof(CornerPoint)).ToArray();
        protected string[] faceNames = Enum.GetNames(typeof(FacePoint)).ToArray();

        public Transform rootTransform => transform.parent == null || (transform.parent.GetComponent<BaseVolume>() == null) ? transform : transform.parent;

        public Transform VolumeParentTransform => transform != rootTransform ? transform.parent.GetComponent<BaseVolume>().transform : GetComponent<BaseVolume>().transform;

        public BaseVolume VolumeParent => transform != rootTransform ? transform.parent.GetComponent<BaseVolume>() : null;

        public bool IsRootVolume => (transform == rootTransform);

        [SerializeField]
        private UnityEvent onChildCountChanged = new UnityEvent();

        public UnityEvent OnChildCountChanged
        {
            get => onChildCountChanged;
        }

        [SerializeField]
        private UnityEvent onVolumePositionChanged = new UnityEvent();

        public UnityEvent OnVolumePositionChanged
        {
            get => onVolumePositionChanged;
        }

        [SerializeField]
        private UnityEvent onVolumeSizeChanged = new UnityEvent();

        public UnityEvent OnVolumeSizeChanged
        {
            get => onVolumeSizeChanged;
        }

        [SerializeField]
        private UnityEvent onVolumeScaleChanged = new UnityEvent();

        public UnityEvent OnVolumeScaleChanged
        {
            get => onVolumeScaleChanged;
        }

        [SerializeField]
        private UnityEvent onVolumeModified = new UnityEvent();

        public UnityEvent OnVolumeModified
        {
            get => onVolumeModified;
        }

        // Scale animation when switching volumes
        private Coroutine scaleCoroutine;
        private float scaleStartTime;

        private Vector3 currentPosition;
        private Vector3 currentVolumeScale;

        //[SerializeField, HideInInspector]
        private int currentChildCount;

        public virtual void Update()
        {
            CheckPositionChange();
            CheckScaleChange();
            CheckChildCount();

            SyncVolumeHierarchy();
        }

        private void CheckPositionChange()
        {
            if (currentPosition != transform.position)
            {
                OnVolumePositionChanged.Invoke();
                OnVolumeModified.Invoke();
            }

            currentPosition = transform.position;
        }

        private void CheckScaleChange()
        {
            if (currentVolumeScale != transform.lossyScale)
            {
                if (VolumeSizeOrigin != VolumeSizeOrigin.Custom)
                {
                    OnVolumeModified.Invoke();
                }

                OnVolumeSizeChanged.Invoke();
            }

            currentVolumeScale = transform.lossyScale;
        }

        private void CheckChildCount()
        {
            if (currentChildCount != transform.childCount)
            {
                OnChildCountChanged.Invoke();
            }

            currentChildCount = transform.childCount;
        }

        protected virtual void OnEnable()
        {
            volumeID = GetInstanceID();

            // Update the ChildVolumeItems if the child count has changed
            OnChildCountChanged.AddListener(() => SyncChildObjects());

            if (VolumeBounds.HostTransform == null)
            {
                VolumeBounds.HostTransform = transform;
            }

            if (MarginBounds.HostTransform == null)
            {
                MarginBounds.HostTransform = transform;
            }
        }

        public virtual void SyncVolumeHierarchy()
        {
            MaintainScaleChildItems();

            UpdateVolumeBounds();
            UpdateMarginBounds();
        }

        private void Start()
        {
            SyncVolumeHierarchy();
        }

        public void UpdateVolumeBounds()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                Collider collider = gameObject.transform.GetComponent<Collider>();

                if (collider != null)
                {
                    VolumeBounds.Size = collider.bounds.size;
                    VolumeBounds.Center = collider.bounds.center;
                }
                else
                {
                    Debug.Log($"Attach an active collider to {gameObject.name} to use the ColliderBounds VolumeSizeOrigin");
                }
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
            {
                VolumeBounds.Size = transform.localScale;
                VolumeBounds.Center = transform.position;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
            {
                VolumeBounds.Size = transform.lossyScale;
                VolumeBounds.Center = transform.position;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.RendererBounds)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();

                Bounds bounds = new Bounds();
                if (renderers.Length != 0)
                {
                    bounds = renderers[0].bounds;
                    foreach (Renderer r in renderers) { bounds.Encapsulate(r.bounds); }
                }

                VolumeBounds.Size = bounds.size;
                VolumeBounds.Center = bounds.center;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
            {
                VolumeBounds.Center = transform.position;
            }

            MarginBounds.Center = VolumeBounds.Center;

            if (!UseMargin)
            {
                MarginBounds.Size = VolumeBounds.Size;
            }
            


            OnVolumeSizeChanged.Invoke();
        }

        // Ensure that the margin bounds size is never less than the volume bounds size
        private void UpdateMarginBounds()
        {
            MarginBounds.Size = new Vector3(
                                    MarginBounds.Width < VolumeBounds.Width ? VolumeBounds.Width : MarginBounds.Width,
                                    MarginBounds.Height < VolumeBounds.Height ? VolumeBounds.Height : MarginBounds.Height,
                                    MarginBounds.Depth < VolumeBounds.Depth ? VolumeBounds.Depth : MarginBounds.Depth);
        }

        public List<BaseVolume> GetChildVolumes()
        {
            List<BaseVolume> uiVolumes = new List<BaseVolume>();
            
            foreach (var item in ChildVolumeItems)
            {
                uiVolumes.Add(item.Volume);
            }

            return uiVolumes;
        }

        public List<Transform> GetChildTransforms(bool includeInactiveObjects = true)
        {
            List<Transform> uiVolumeTransforms = new List<Transform>();

            ChildVolumeItems.ForEach((item) =>
            {
                // Should inactive game object be included
                if (includeInactiveObjects)
                {
                    uiVolumeTransforms.Add(item.Transform);
                }
                else
                {
                    // Check if the game object is active before adding
                    if (item.Transform.gameObject.activeSelf)
                    {
                        uiVolumeTransforms.Add(item.Transform);
                    }
                }
            });

            return uiVolumeTransforms;
        }

        public int GetActiveChildCount()
        {
            int count = 0; 

            foreach (ChildVolumeItem item in ChildVolumeItems)
            {
                if (item.Transform.gameObject.activeSelf)
                {
                    count++;
                }
            }

            return count;
        }

        public void SyncChildObjects(bool force = false)
        {
            currentChildCount = transform.childCount;

            if (childVolumeCount < currentChildCount)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    ChildVolumeItem volumeTransform = ChildVolumeItems.Find((volume) => volume.Transform == transform.GetChild(i));
                    
                    if (volumeTransform == null)
                    {
                        ChildVolumeItems.Add(new ChildVolumeItem(transform.GetChild(i)));
                    }
                }
            }
            else if (childVolumeCount > currentChildCount)
            {
                List<Transform> directChildren = new List<Transform>();

                for (int i = 0; i < transform.childCount; i++)
                {
                    directChildren.Add(transform.GetChild(i));
                }

                foreach (var volume in ChildVolumeItems.ToList())
                {
                    if (!directChildren.Contains(volume.Transform))
                    {
                        ChildVolumeItems.Remove(volume);
                    }
                }
            }
        }

        public void SetChildVolumePositions(Vector3[] positions, bool smoothingEnabled = true, float lerpTime = 3)
        {
            for (int i = 0; i < ChildVolumeItems.Count && i < positions.Length; i++)
            {
                if (ChildVolumeItems[i].Transform != null)
                { 
                    if (smoothingEnabled && Application.isPlaying)
                    {
                        ChildVolumeItems[i].Transform.position = Vector3.Lerp(ChildVolumeItems[i].Transform.position, positions[i], Time.deltaTime * lerpTime);
                    }
                    else
                    {
                        ChildVolumeItems[i].Transform.position = positions[i];
                    }
                }
            }
        }

        public void SwitchChildVolumes(BaseVolume child, BaseVolume targetVolume)
        {
            BaseVolume volumeToSwitch = DirectChildVolumes.Find((item) => item == child);
            Vector3 pos = targetVolume.transform.position;

            volumeToSwitch.transform.SetParent(targetVolume.transform, true);
            Vector3 velocity = Vector3.zero;

            child.transform.position = Vector3.SmoothDamp(child.transform.position, pos, ref velocity, 10 * Time.deltaTime);

            float diffX = targetVolume.transform.lossyScale.x / transform.lossyScale.x;
            float diffY = targetVolume.transform.lossyScale.y / transform.lossyScale.y;
            float diffZ = targetVolume.transform.lossyScale.z / transform.lossyScale.z;

            Vector3 diff = new Vector3(diffX, diffY, diffZ);
            Vector3 childScale = child.transform.localScale;
            Vector3 targetScale = Vector3.Scale(childScale, diff);

            scaleCoroutine = StartCoroutine(TransitionScale(child.transform, childScale, targetScale));

            SyncChildObjects();
        }

        private IEnumerator TransitionScale(Transform currentTransform, Vector3 currentScale, Vector3 targetScale)
        {
            scaleStartTime = Time.time;
            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime;
                currentTransform.localScale = Vector3.Lerp(currentScale, targetScale, t * 1.5f);
                yield return null;
            }
        }

        private void MaintainScaleChildItems()
        {
            int i = 0;

            foreach (var item in ChildVolumeItems)
            {
                // If the transform has switched to a rect transform, update it
                if (item.Transform == null)
                {
                    item.Transform = transform.GetChild(i);
                }

                VolumeSizeOrigin childVolumeSizeOrigin = item.IsVolume() ? item.Transform.GetComponent<BaseVolume>().VolumeSizeOrigin : VolumeSizeOrigin.None;

                item.OnParentScaleChanged(childVolumeSizeOrigin);

                i++;
            }
        }

        public bool ContainsVolume(BaseVolume volume)
        {
            foreach(var vol in ChildVolumeItems)
            {
                if (vol.Volume == volume)
                {
                    return true;
                }
            }

            return false;
        }

        public Vector3 GetAxisDistances()
        {
            return new Vector3(VolumeBounds.Width, VolumeBounds.Height, VolumeBounds.Depth);
        }

        public Vector3 GetFacePoint(FacePoint name)
        {
            return VolumeBounds.GetFacePoint(name);
        }

        public Vector3 GetCornerPoint(CornerPoint name)
        {
            return VolumeBounds.GetCornerPoint(name);
        }

        public Vector3 GetCornerMidPoint(CornerPoint p1, CornerPoint p2)
        {
            return VolumeBounds.GetCornerMidPoint(p1, p2);
        }

        public Vector3 GetFaceMidPoint(FacePoint p1, FacePoint p2)
        {
            return VolumeBounds.GetFaceMidPoint(p1, p2);
        }

        public float GetAxisDistance(VolumeAxis axis)
        {
            if (axis == VolumeAxis.X)
            {
                return VolumeBounds.Width;
            }
            else if (axis == VolumeAxis.Y)
            {
                return VolumeBounds.Height;
            }
            else
            {
                return VolumeBounds.Depth;
            }
        }

        public bool CheckExternalVolumeEntered(BaseVolume volume, bool maintainScale = true)
        {
            if (volume.transform.parent != transform)
            {
                foreach (var cornerPoint in volume.VolumeCorners)
                {
                    if (!VolumeBounds.Contains(cornerPoint))
                    {
                        return false;
                    }

                    volume.gameObject.transform.SetParent(transform);

                    if (maintainScale)
                    {
                        SetMaintainScale(true, volume.gameObject);
                    }
                    
                    Debug.Log($"{volume.gameObject.name} has entered the bounds of {gameObject.name}, changing parent transforms.");

                    return true;
                }
            }

            return false;
        }

        public bool CheckInternalVolumeExited(BaseVolume volume, Transform newParent)
        {
            if (volume.transform.parent == transform) 
            {
                foreach (var cornerPoint in volume.VolumeCorners)
                {
                    if (VolumeBounds.Contains(cornerPoint))
                    {
                        return false;
                    }

                    volume.gameObject.transform.SetParent(newParent);

                    Debug.Log($"{volume.gameObject.name} has exited the bounds of {gameObject.name}, changing to a new parent transform.");

                    return true;
                }                
            }

            return false;
        }

        public void SetMaintainScale(bool maintainScale, GameObject target)
        {
            SyncChildObjects();

            ChildVolumeItem childVolumeItem = ChildVolumeItems.Find((item) => item.Transform.gameObject == target);
            
            if (childVolumeItem == null)
            {
                Debug.LogError($"{target.name} is not present in the hierarchy, maintain scale was not applied");
            }
            else
            {
                childVolumeItem.MaintainScale = maintainScale;
            }
           
        }

        public bool GetMaintainScale(GameObject target)
        {
            BaseVolume parentUIVolume = VolumeParentTransform.GetComponent<BaseVolume>();

            if (parentUIVolume != null)
            {
                ChildVolumeItem childVolumeItem = parentUIVolume.ChildVolumeItems.Find((item) => item.Transform.gameObject == target);
                return childVolumeItem.MaintainScale;
            }

            return false;
        }


        /// <summary>
        /// Get the occupied space along an axis.
        /// </summary>
        /// <param name="volumeAxis">Axis for calculation</param>
        /// <returns>Occupied space along an axis</returns>
        public float GetOccupiedSpace(VolumeAxis volumeAxis)
        {
            float occupiedAxisSpace = 0;

            foreach (ChildVolumeItem item in ChildVolumeItems)
            {
                if (item.Volume == null)
                {
                    Debug.LogWarning($"{item.Transform.gameObject.name} does not have a Volume attached and has not been included in the remaining space calculation.");
                }
                else
                {
                    occupiedAxisSpace += item.Volume.GetAxisDistance(volumeAxis);
                }
            }

            return occupiedAxisSpace;
        }

        /// <summary>
        /// Get the empty space along an axis.
        /// </summary>
        /// <param name="volumeAxis">Axis for calculation</param>
        /// <returns>Empty space along an axis</returns>
        public float GetEmptySpace(VolumeAxis volumeAxis)
        {
            return GetAxisDistance(volumeAxis) - GetOccupiedSpace(volumeAxis);
        }

        public Vector3 GetMarginDifference()
        {
            return MarginBounds.Size - VolumeBounds.Size;
        }

        protected virtual void OnDrawGizmos()
        {
            DrawCornerAndFacePoints();
            DrawVolumeBoundsContainer();
        }

        private void DrawVolumeBoundsContainer()
        {
            if (DrawMarginBounds)
            {
                MarginBounds.DrawBounds(Color.magenta);
            }

            if (DrawVolumeBounds)
            {
                VolumeBounds.DrawBounds(Color.yellow);
            }
        }

        private void DrawCornerAndFacePoints()
        {
            float pointRadius = (VolumeBounds.Width + VolumeBounds.Height + VolumeBounds.Depth) * 0.01f;

            if (DrawCornerPoints)
            {
                Gizmos.color = Color.magenta;
                foreach (var point in VolumeCorners)
                {
                    Gizmos.DrawSphere(point, pointRadius);
                }
            }

            if (DrawFacePoints)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in VolumeFaces)
                {
                    Gizmos.DrawSphere(point, pointRadius);
                }
            }
        }

        public void PrintMatrix()
        {
            Matrix4x4 localToWoldMatrix = transform.localToWorldMatrix;

            Debug.Log("======== Local to World Matrix ==========");

            Debug.Log(localToWoldMatrix.GetRow(0).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(1).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(2).ToString("F6"));
            Debug.Log(localToWoldMatrix.GetRow(3).ToString("F6"));

            Debug.Log("==================");

            Debug.Log("Local Scale: " + transform.localScale.ToString("F6"));
            Debug.Log("Lossy Scale: " + transform.lossyScale.ToString("F6"));

            if (transform.parent != null)
            {
                Debug.Log("Parent Lossy Scale: " + transform.parent.lossyScale.ToString("F6"));
            }

            Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;

            Debug.Log("======== World to Local Matrix ==========");

            Debug.Log(worldToLocalMatrix.GetRow(0).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(1).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(2).ToString("F6"));
            Debug.Log(worldToLocalMatrix.GetRow(3).ToString("F6"));

            Debug.Log("==================");
        }
    }
}
