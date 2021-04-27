// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Input;
using UnityPhysics = UnityEngine.Physics;

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
        private VolumeSizeOrigin volumeSizeOrigin;

        /// <summary>
        /// The sizing entry point for this Volume.
        /// </summary>
        public VolumeSizeOrigin VolumeSizeOrigin
        {
            get => volumeSizeOrigin;
            private set => volumeSizeOrigin = value;
        }

        [SerializeField]
        private Bounds volumeBounds; 

        /// <summary>
        /// 
        /// </summary>
        public Bounds VolumeBounds
        {
            get => volumeBounds;
            set => volumeBounds = value;
        }

        [SerializeField]
        private Bounds marginBounds;

        /// <summary>
        /// 
        /// </summary>
        public Bounds MarginBounds
        {
            // The margin bounds are going to be the volume bounds += 
            get => marginBounds;
            set => marginBounds = value;
        }

        [SerializeField]
        private Bounds paddingBounds;

        /// <summary>
        /// 
        /// </summary>
        public Bounds PaddingBounds
        {
            // Padding bounds are the volume bound -=
            get => paddingBounds;
            set => paddingBounds = value;
        }


        /// <summary>
        /// The size of the volume
        /// </summary>
        public Vector3 VolumeSize
        {
            get => volumeBounds.size;
            set => volumeBounds.size = value;
        }

        public Vector3 VolumePosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
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

        [SerializeField]
        private bool drawCornerPoints = false;

        public bool DrawCornerPoints
        {
            get => drawCornerPoints;
            set => drawCornerPoints = value;
        }

        [SerializeField]
        private bool drawFacePoints = false;

        public bool DrawFacePoints
        {
            get => drawFacePoints;
            set => drawFacePoints = value;
        }

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
        }

        protected virtual void OnEnable()
        {
            InitializePoints();

            volumeID = GetInstanceID();

#if UNITY_EDITOR
            EditorApplication.hierarchyChanged += CheckChildCount;
#endif

            OnChildCountChanged.AddListener(() => SyncChildObjects());
        }

        public virtual void SyncVolumeHierarchy()
        {
            currentChildCount = transform.childCount;

            CheckChildCount();
            MaintainScaleChildItems();

            CalculateVolumeBounds();

            UpdatePoints();
        }

        private void CalculateVolumeBounds()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                Bounds colliderBounds = gameObject.transform.GetComponent<Collider>().bounds;

                volumeBounds.size = colliderBounds.size;

                float x = volumeBounds.size.x + (volumeBounds.size.x * Mathf.Pow(colliderBounds.size.x, 1f));
                float y = volumeBounds.size.y + Mathf.Pow(colliderBounds.size.y, 1f);
                float z = volumeBounds.size.z + Mathf.Pow(colliderBounds.size.z, 1f);

                volumeBounds.size = new Vector3(x, y, z);
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
            {
                volumeBounds.size = gameObject.transform.localScale;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
            {
                volumeBounds.size = gameObject.transform.lossyScale;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.RendererBounds)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();

                if (renderers.Length != 0)
                {
                    volumeBounds = renderers[0].bounds;
                    foreach (Renderer r in renderers) { volumeBounds.Encapsulate(r.bounds); }
                }

            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
            {
                if (UIVolumeParent != null)
                {
                    //Bounds expanded = UIVolumeParent.VolumeBounds.ExpandToContain(VolumeBounds);
                    //UIVolumeParent.volumeBounds = expanded;
                }
            }


            if (VolumeSizeOrigin != VolumeSizeOrigin.RendererBounds)
            {
                volumeBounds.center = transform.position;
            }


            OnVolumeSizeChanged.Invoke();
        }

        protected Vector3 GetVolumeSizeOffset()
        {
            return volumeBounds.extents;
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
            return new Vector3(GetAxisDistance(VolumeAxis.X), GetAxisDistance(VolumeAxis.Y), GetAxisDistance(VolumeAxis.Z));
        }

        public Vector3 GetFacePoint(FacePoint name)
        {
            return Array.Find(UIVolumeFaces, (point) => point.PointName == name.ToString()).Point;
        }

        public Vector3 GetCornerPoint(CornerPoint name)
        {
            return Array.Find(UIVolumeCorners, (point) => point.PointName == name.ToString()).Point;
        }

        public Vector3 GetCornerMidPoint(CornerPoint p1, CornerPoint p2)
        {
            return (GetCornerPoint(p1) + GetCornerPoint(p2)) * 0.5f;
        }

        public float GetAxisDistance(VolumeAxis axis)
        {
            if (axis == VolumeAxis.X)
            {
                return Vector3.Distance(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
            }
            else if (axis == VolumeAxis.Y)
            {
                return Vector3.Distance(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));
            }
            else
            {
                return Vector3.Distance(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));
            }
        }

        protected void UpdateCornerPoints()
        {
            Vector3[] positions = new Vector3[8];

            VolumeBounds.GetCornerPositions(ref positions);

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
                switch (UIVolumeCorners[i].PointName)
                {
                    case "LeftBottomForward":
                        UIVolumeCorners[i].Point = positions[0];
                        break;
                    case "LeftBottomBack":
                        UIVolumeCorners[i].Point = positions[1];
                        break;
                    case "LeftTopForward":
                        UIVolumeCorners[i].Point = positions[2];
                        break;
                    case "LeftTopBack":
                        UIVolumeCorners[i].Point = positions[3];
                        break;
                    case "RightBottomForward":
                        UIVolumeCorners[i].Point = positions[4];
                        break;
                    case "RightBottomBack":
                        UIVolumeCorners[i].Point = positions[5];
                        break;
                    case "RightTopForward":
                        UIVolumeCorners[i].Point = positions[6];
                        break;
                    case "RightTopBack":
                        UIVolumeCorners[i].Point = positions[7];
                        break;

                }
            }
        }

        protected void UpdateFacePoints()
        {
            Vector3[] faces = new Vector3[6];

            VolumeBounds.GetFacePositions(ref faces);

            // [0] == Top
            // [1] == Bottom
            // [2] == Left
            // [3] == Right
            // [4] == Back
            // [5] == Forward

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                switch (UIVolumeFaces[i].PointName)
                {
                    case "Top":
                        UIVolumeFaces[i].Point = faces[0];
                        break;
                    case "Bottom":
                        UIVolumeFaces[i].Point = faces[1];
                        break;
                    case "Left":
                        UIVolumeFaces[i].Point = faces[2];
                        break;
                    case "Right":
                        UIVolumeFaces[i].Point = faces[3];
                        break;
                    case "Back":
                        UIVolumeFaces[i].Point = faces[4];
                        break;
                    case "Forward":
                        UIVolumeFaces[i].Point = faces[5];
                        break;
                }
            }
        }

        public void UpdatePoints()
        {
            UpdateCornerPoints();
            UpdateFacePoints();
        }

        protected string[] NameParse(string name)
        {
            string pointNameStringSpaces = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");

            string[] words = pointNameStringSpaces.Split(' ');

            return words;
        }

        protected Vector3 CalculateVolumeSizeOffsetParent()
        {
            bool isParentColliderContainer = UIVolumeParentTransform.GetComponent<Collider>() != null;

            float xOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.x : UIVolumeParentTransform.localScale.x * 0.5f;
            float yOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.y : UIVolumeParentTransform.localScale.y * 0.5f;
            float zOffset = isParentColliderContainer ? UIVolumeParentTransform.GetColliderBounds().extents.z : UIVolumeParentTransform.localScale.z * 0.5f;

            Vector3 offset = new Vector3(xOffset, yOffset, zOffset);

            return offset;
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
            childVolumeItem.MaintainScale = maintainScale;
        
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

        protected virtual void OnDrawGizmos()
        {
            DrawVolumeContainer();

            DrawCornerAndFacePoints();
        }

        private void DrawVolumeContainer()
        {
            if (VolumeSizeOrigin != VolumeSizeOrigin.RendererBounds)
            {
                // Supports rotations 
                //Gizmos.matrix = transform.localToWorldMatrix;
            }

            Gizmos.color = IsRootUIVolume ? Color.green : Color.cyan;

            Gizmos.DrawWireCube(volumeBounds.center, VolumeSize);
        }

        private void DrawCornerAndFacePoints()
        {
            if (DrawCornerPoints)
            {
                Gizmos.color = Color.magenta;
                foreach (var point in UIVolumeCorners)
                {
                    Gizmos.DrawSphere(point.Point, 0.01f);
                }
            }

            if (DrawFacePoints)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in UIVolumeFaces)
                {
                    Gizmos.DrawSphere(point.Point, 0.01f);
                }
            }
        }

        // Test point calculation relative to a defined transform
        private void DrawBoundsCornersAndFaces()
        {
            Vector3[] positions = new Vector3[8];

            VolumeBounds.GetCornerPositions(transform, ref positions);

            // [0] == LeftBottomForward
            // [1] == LeftBottomBack
            // [2] == LeftTopForward
            // [3] == LeftTopBack

            // [4] == RightBottomForward
            // [5] == RightBottomBack
            // [6] == RightTopForward
            // [7] == RightTopBack

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(positions[0], 0.01f);
            Gizmos.DrawSphere(positions[1], 0.01f);
            Gizmos.DrawSphere(positions[2], 0.01f);
            Gizmos.DrawSphere(positions[3], 0.01f);

            Gizmos.DrawSphere(positions[4], 0.01f);
            Gizmos.DrawSphere(positions[5], 0.01f);
            Gizmos.DrawSphere(positions[6], 0.01f);
            Gizmos.DrawSphere(positions[7], 0.01f);

            Vector3[] faces = new Vector3[6];

            VolumeBounds.GetFacePositions(transform, ref faces);

            // [0] == Top
            // [1] == Bottom
            // [2] == Left
            // [3] == Right
            // [4] == Back
            // [5] == Forward

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(faces[0], 0.01f);
            Gizmos.DrawSphere(faces[1], 0.01f);
            Gizmos.DrawSphere(faces[2], 0.01f);
            Gizmos.DrawSphere(faces[3], 0.01f);
            Gizmos.DrawSphere(faces[4], 0.01f);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(faces[5], 0.01f);
        }

        //private void DrawMargins()
        //{
        //    Gizmos.color = Color.yellow;

        //    // Left Margin
        //    Vector3 leftMarginStartPoint = GetFacePoint(FacePoint.Left);
        //    float volumeWidth = Vector3.Distance(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
        //    Gizmos.DrawLine(leftMarginStartPoint, leftMarginStartPoint + (Vector3.right * (LeftMargin * volumeWidth)));

        //    // Top Margin
        //    Vector3 topMarginStartPoint = GetFacePoint(FacePoint.Top);
        //    float volumeHeight = Vector3.Distance(GetFacePoint(FacePoint.Top), GetFacePoint(FacePoint.Bottom));
        //    Gizmos.DrawLine(topMarginStartPoint, topMarginStartPoint + (Vector3.down * (TopMargin * volumeHeight)));

        //    // Forward Margin
        //    Vector3 forwardMarginStartPoint = GetFacePoint(FacePoint.Forward);
        //    float volumeDepth = Vector3.Distance(GetFacePoint(FacePoint.Forward), GetFacePoint(FacePoint.Back));
        //    Gizmos.DrawLine(forwardMarginStartPoint, forwardMarginStartPoint + (Vector3.forward * (ForwardMargin * volumeDepth)));
        //}
    }
}
