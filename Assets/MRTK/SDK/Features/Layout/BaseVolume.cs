// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public abstract class BaseVolume : MonoBehaviour
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
            set => volumeSizeOrigin = value;
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

        [SerializeField]
        private float marginLeftAndRightMM = 0;

        /// <summary>
        /// 
        /// </summary>
        public float MarginLeftAndRightMM
        {
            get => marginLeftAndRightMM;
            set
            {
                marginLeftAndRightMM = value;

                float mmToMeters = marginLeftAndRightMM * 0.001f;

                MarginBounds.Width = VolumeBounds.Width + (mmToMeters * 2f);
            }
        }


        [SerializeField]
        private float marginTopAndBottomMM = 0;

        /// <summary>
        /// 
        /// </summary>
        public float MarginTopAndBottomMM
        {
            get => marginTopAndBottomMM;
            set
            {
                marginTopAndBottomMM = value;

                float mmToMeters = marginTopAndBottomMM * 0.001f;

                MarginBounds.Height = VolumeBounds.Height + (mmToMeters * 2f);
            }
        }

        [SerializeField]
        private float marginForwardAndBackMM = 0;

        /// <summary>
        /// 
        /// </summary>
        public float MarginForwardAndBackMM
        {
            get => marginForwardAndBackMM;
            set
            {
                marginForwardAndBackMM = value;

                float mmToMeters = marginForwardAndBackMM * 0.001f;

                MarginBounds.Depth = VolumeBounds.Depth + (mmToMeters * 2f);
            }
        }


        [SerializeField]
        private bool editMargin;

        /// <summary>
        /// 
        /// </summary>
        public bool EditMargin
        {
            get => editMargin;
            set => editMargin = value;
        }

        /// <summary>
        /// The size of the volume
        /// </summary>
        public Vector3 VolumeSize
        {
            get => volumeBounds.Size;
            set
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale || VolumeSizeOrigin == VolumeSizeOrigin.LocalScale || VolumeSizeOrigin == VolumeSizeOrigin.RendererBounds)
                {
                    Debug.LogError("The Volume Size can only be set if the VolumeSizeOrigin is Custom");
                }
                else
                {
                    volumeBounds.Size = value;
                }
            } 
        }

        public Vector3 VolumeCenter
        {
            get => volumeBounds.Center;
            set => volumeBounds.Center = value;
        }

        public Vector3 VolumePosition
        {
            get => transform.position;
            set => transform.position = value;
        }

        [SerializeField]
        private UIVolumePoint[] uiVolumeCorners = new UIVolumePoint[8];

        public UIVolumePoint[] UIVolumeCorners
        {
            get => uiVolumeCorners;
        }

        [SerializeField]
        private UIVolumePoint[] uiVolumeFaces = new UIVolumePoint[6];

        public UIVolumePoint[] UIVolumeFaces
        {
            get => uiVolumeFaces;
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

        #endregion

        [SerializeField]
        private Mesh volumeMesh;

        public Mesh VolumeMesh
        {
            get => volumeMesh;
            set => volumeMesh = value;
        }

        [SerializeField]
        private List<ChildVolumeItem> childVolumeItems = new List<ChildVolumeItem>();

        public List<ChildVolumeItem> ChildVolumeItems
        {
            get => childVolumeItems;
            private set => childVolumeItems = value;
        }

        public List<UIVolume> DirectChildUIVolumes
        {
            get => GetChildUIVolumes();
        }

        [SerializeField]
        private int childUIVolumeCount => ChildVolumeItems.Count;

        protected int currentChildCount = 0;

        protected string[] cornerNames = Enum.GetNames(typeof(CornerPoint)).ToArray();
        protected string[] faceNames = Enum.GetNames(typeof(FacePoint)).ToArray();

        public Transform rootTransform => transform.parent == null || (transform.parent.GetComponent<UIVolume>() == null) ? transform : transform.parent;

        public Transform UIVolumeParentTransform => transform != rootTransform ? transform.parent.GetComponent<UIVolume>().transform : GetComponent<UIVolume>().transform;

        public UIVolume UIVolumeParent => transform != rootTransform ? transform.parent.GetComponent<UIVolume>() : null;

        public bool IsRootUIVolume => (transform == rootTransform);

        public UnityEvent OnChildCountChanged = new UnityEvent();

        public UnityEvent OnVolumeSizeChanged = new UnityEvent();

        public virtual void Update()
        {
            SyncVolumeHierarchy();

            VolumeBounds.DrawBounds(Color.yellow);
        }

        protected virtual void OnEnable()
        {
            InitializePoints();

            volumeID = GetInstanceID();

#if UNITY_EDITOR
            EditorApplication.hierarchyChanged += CheckChildCount;
#endif

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
            currentChildCount = transform.childCount;

            CheckChildCount();
            MaintainScaleChildItems();

            CalculateVolumeBounds();
            UpdateMarginBounds();

            UpdatePoints();
        }

        private void CalculateVolumeBounds()
        {
            
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                Collider collider = gameObject.transform.GetComponent<Collider>();

                VolumeBounds.Size = collider.bounds.size;
                VolumeBounds.Center = collider.bounds.center;
                
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

            // TO DO - HANDLE EDITOR MARGIN RESIZE
            MarginBounds.Size = VolumeBounds.Size;
           
            OnVolumeSizeChanged.Invoke();
        }

        private void UpdateMarginBounds()
        {
            if (MarginBounds.Width < VolumeBounds.Width)
            {
                MarginBounds.Width = VolumeBounds.Width;
            }

            if (MarginBounds.Height < VolumeBounds.Height)
            {
                MarginBounds.Height = VolumeBounds.Height;
            }

            if (MarginBounds.Depth < VolumeBounds.Depth)
            {
                MarginBounds.Depth = VolumeBounds.Depth;
            }
        }

        protected Vector3 GetVolumeSizeOffset()
        {
            if (MarginBounds.Size != VolumeSize)
            {
                return MarginBounds.Extents;
            }
            else
            {
                return VolumeBounds.Extents;
            }
        }

        public List<UIVolume> GetChildUIVolumes()
        {
            List<UIVolume> uiVolumes = new List<UIVolume>();
            
            foreach (var item in ChildVolumeItems)
            {
                uiVolumes.Add(item.UIVolume);
            }

            return uiVolumes;
        }

        protected List<Transform> GetChildTransforms(bool includeInactiveObjects = true)
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

        private void CheckChildCount()
        {
            if (currentChildCount != childUIVolumeCount)
            {
                OnChildCountChanged.Invoke();
            }
        }

        protected void SyncChildObjects(bool force = false)
        {
            currentChildCount = transform.childCount;

            if (childUIVolumeCount < currentChildCount)
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
            else if (childUIVolumeCount > currentChildCount)
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

                VolumeSizeOrigin childVolumeSizeOrigin = item.IsUIVolume() ? item.Transform.GetComponent<UIVolume>().VolumeSizeOrigin : VolumeSizeOrigin.None;

                item.OnParentScaleChanged(childVolumeSizeOrigin);

                i++;
            }
        }

        public bool ContainsVolume(BaseVolume volume)
        {
            foreach(var vol in ChildVolumeItems)
            {
                if (vol.UIVolume == volume)
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
            return (GetCornerPoint(p1) + GetCornerPoint(p2)) * 0.5f;
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

        protected void UpdateCornerPoints()
        {
            Vector3[] positions = VolumeBounds.GetCornerPositions();

            // [0] == LeftBottomForward
            // [1] == LeftBottomBack
            // [2] == LeftTopForward
            // [3] == LeftTopBack

            // [4] == RightBottomForward
            // [5] == RightBottomBack
            // [6] == RightTopForward
            // [7] == RightTopBack

            for (int i = 0; i < UIVolumeCorners.Length; i++)
            {
                UIVolumeCorners[i].Point = positions[i];
            }
        }

        protected void UpdateFacePoints()
        {
            Vector3[] faces = VolumeBounds.GetFacePositions();

            // [0] == Top
            // [1] == Bottom
            // [2] == Left
            // [3] == Right
            // [4] == Back
            // [5] == Forward

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                UIVolumeFaces[i].Point = faces[i];
            }
        }

        public void UpdatePoints()
        {
            UpdateCornerPoints();
            UpdateFacePoints();
        }

        private void InitializePoints()
        {
            for (int i = 0; i < UIVolumeCorners.Length; i++)
            {
                UIVolumeCorners[i] = new UIVolumePoint(cornerNames[i]);
            }

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                UIVolumeFaces[i] = new UIVolumePoint(faceNames[i]);
            }
        }

        public bool CheckExternalVolumeEntered(BaseVolume volume, bool maintainScale = true)
        {
            if (volume.transform.parent != transform)
            {
                foreach (var cornerPoint in volume.UIVolumeCorners)
                {
                    if (!VolumeBounds.Contains(cornerPoint.Point))
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
                foreach (var cornerPoint in volume.UIVolumeCorners)
                {
                    if (VolumeBounds.Contains(cornerPoint.Point))
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
            UIVolume parentUIVolume = UIVolumeParentTransform.GetComponent<UIVolume>();

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
                if (item.UIVolume == null)
                {
                    Debug.LogWarning($"{item.Transform.gameObject.name} does not have a UIVolume attached and has not been included in the remaining space calculation.");
                }
                else
                {
                    occupiedAxisSpace += item.UIVolume.GetAxisDistance(volumeAxis);
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
        }

        private void DrawCornerAndFacePoints()
        {
            if (DrawCornerPoints)
            {
                Gizmos.color = Color.magenta;
                foreach (var point in VolumeBounds.GetCornerPositions())
                {

                    Gizmos.DrawSphere(point, 0.01f);

                }
            }

            if (DrawFacePoints)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in VolumeBounds.GetFacePositions())
                {
                    Gizmos.DrawSphere(point, 0.01f);
                }
            }
        }
    }
}
