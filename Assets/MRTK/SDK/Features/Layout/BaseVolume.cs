// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

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
        private Mesh volumeMesh;

        public Mesh VolumeMesh
        {
            get => volumeMesh;
            set => volumeMesh = value;
        }

        [SerializeField]
        private Vector3 volumeSize;

        /// <summary>
        /// The size of the volume
        /// </summary>
        public Vector3 VolumeSize
        {
            get => volumeSize;
            set => volumeSize = value;
        }

        [SerializeField]
        private Vector3 volumePosition;

        public Vector3 VolumePosition
        {
            get => volumePosition;
            set => volumePosition = value;
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
        private List<ChildVolumeItem> childVolumeItems = new List<ChildVolumeItem>();

        public List<ChildVolumeItem> ChildVolumeItems
        {
            get => childVolumeItems;
            private set => childVolumeItems = value;
        }

        [SerializeField]
        private List<UIVolume> directChildUIVolumes => GetChildUIVolumes();

        public List<UIVolume> DirectChildUIVolumes
        {
            get => directChildUIVolumes;
        }

        public UnityEvent OnChildCountChanged = new UnityEvent();

        [SerializeField]
        private int childUIVolumeCount => ChildVolumeItems.Count;

        protected int currentChildCount = 0;

        protected string[] cornerNames = Enum.GetNames(typeof(CornerPoint)).ToArray();
        protected string[] faceNames = Enum.GetNames(typeof(FacePoint)).ToArray();

        public Transform rootTransform => transform.parent == null || (transform.parent.GetComponent<UIVolume>() == null) ? transform : transform.parent;

        public Transform UIVolumeParentTransform => transform != rootTransform ? transform.parent.GetComponent<UIVolume>().transform : GetComponent<UIVolume>().transform;

        public virtual void Update()
        {
            SyncVolumeHierarchy();
        }

        protected virtual void OnEnable()
        {
            InitializePoints();
            SetVolumeSizeOrigin();
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

            SetVolumeSize();

            UpdatePoints();
        }

        public void SetVolumeSizeOrigin()
        {
            // Determine if the volume is: 
            // 1. Empty Container - an empty game object
            // 2. Mesh - object with box collider attached
            // 3. Text -  object with text mesh pro attached 

            if (GetComponent<Collider>() != null)
            {
                // Use the collider bounds as the source for the sizing  
                VolumeSizeOrigin = VolumeSizeOrigin.ColliderBounds;
            }
            else if (GetComponent<TextMeshPro>() != null)
            {
                // Use a combination of local scale, rect transform width, text mesh pro margin
                // as the source for the sizing with text mesh pro attached
                VolumeSizeOrigin = VolumeSizeOrigin.TextMeshPro;
            }
            else
            {
                // Use the local scale as the source for the sizing for an empty game object
                VolumeSizeOrigin = VolumeSizeOrigin.LocalScale;
            }
        }

        public Vector3 SetVolumeSize()
        {
            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                VolumeSize = gameObject.transform.GetColliderBounds().size;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                RectTransform rectTransfrom = transform as RectTransform;

                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float xOffset = rectTransfrom.lossyScale.x;
                float widthOffset = rectTransfrom.rect.width;
                float scaleWithOffset = xOffset * widthOffset;
                float marginOffset = -textPro.margin.z * xOffset;
                float finalOffsetX = scaleWithOffset + marginOffset;

                float yOffset = rectTransfrom.lossyScale.y;
                float heightOffset = rectTransfrom.rect.height;
                float scaleHeightOffset = yOffset * heightOffset;
                float marginOffsetY = -textPro.margin.y * yOffset;
                float finalOffsetY = scaleHeightOffset + marginOffsetY;

                Vector3 volumeTextSize = new Vector3(finalOffsetX, finalOffsetY, 0);

                VolumeSize = volumeTextSize;
            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
            {
                VolumeSize = gameObject.transform.localScale;
            }

            return VolumeSize;
        }

        protected Vector3 CalculateVolumeSizeOffset()
        {
            Vector3 volumeSizeOffset = Vector3.zero;

            if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
            {
                volumeSizeOffset.x = transform.GetColliderBounds().extents.x;
                volumeSizeOffset.y = transform.GetColliderBounds().extents.y;
                volumeSizeOffset.z = transform.GetColliderBounds().extents.z;

            }
            else if (VolumeSizeOrigin == VolumeSizeOrigin.TextMeshPro)
            {
                // A text mesh pro object uses a rect transform 
                RectTransform rectTransfrom = transform as RectTransform;
                TextMeshPro textPro = gameObject.GetComponent<TextMeshPro>();

                float xScaleOffset = rectTransfrom.lossyScale.x;
                float widthOffset = rectTransfrom.rect.width * 0.5f;
                float xScaleWidthOffset = xScaleOffset * widthOffset;
                float marginRightOffset = -textPro.margin.z * xScaleOffset;
                volumeSizeOffset.x = xScaleWidthOffset + marginRightOffset;

                float yScaleOffset = rectTransfrom.lossyScale.y;
                float heightOffset = rectTransfrom.rect.height * 0.5f;
                volumeSizeOffset.y = yScaleOffset * heightOffset;
            }
            else
            {
                volumeSizeOffset.x = transform.localScale.x * 0.5f;
                volumeSizeOffset.y = transform.localScale.y * 0.5f;
                volumeSizeOffset.z = transform.localScale.z * 0.5f;
            }

            return volumeSizeOffset;
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

        protected List<Transform> GetChildTransforms(bool includeInactiveObjects = false)
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

        private void SyncChildObjects()
        {
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

        public Vector3 GetAxisDistances()
        {
            return new Vector3(GetAxisDistance(Axis.X), GetAxisDistance(Axis.Y), GetAxisDistance(Axis.Z));
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

        public float GetAxisDistance(Axis axis)
        {
            if (axis == Axis.X)
            {
                return Vector3.Distance(GetFacePoint(FacePoint.Left), GetFacePoint(FacePoint.Right));
            }
            else if (axis == Axis.Y)
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
            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            for (int i = 0; i < UIVolumeCorners.Length; i++)
            {
                string[] pointNameParse = NameParse(UIVolumeCorners[i].PointName);

                float positionX = pointNameParse[0] == "Left" ? transform.position.x - (volumeSizeOffset.x) : transform.position.x + (volumeSizeOffset.x);
                float positionY = pointNameParse[1] == "Top" ? transform.position.y + (volumeSizeOffset.y) : transform.position.y - (volumeSizeOffset.y);
                float positionZ = pointNameParse[2] == "Forward" ? transform.position.z - (volumeSizeOffset.z) : transform.position.z + (volumeSizeOffset.z);

                UIVolumeCorners[i].Point = new Vector3(positionX, positionY, positionZ);
            }
        }

        protected void UpdateFacePoints()
        {
            Vector3 volumeSizeOffset = CalculateVolumeSizeOffset();

            for (int i = 0; i < UIVolumeFaces.Length; i++)
            {
                float positionX = transform.position.x;
                float positionY = transform.position.y;
                float positionZ = transform.position.z;

                if (UIVolumeFaces[i].PointName == "Left")
                {
                    positionX = transform.position.x - (volumeSizeOffset.x);
                }
                else if (UIVolumeFaces[i].PointName == "Right")
                {
                    positionX = transform.position.x + (volumeSizeOffset.x);
                }
                else if (UIVolumeFaces[i].PointName == "Top")
                {
                    positionY = transform.position.y + volumeSizeOffset.y;
                }
                else if (UIVolumeFaces[i].PointName == "Bottom")
                {
                    positionY = transform.position.y - volumeSizeOffset.y;
                }
                else if (UIVolumeFaces[i].PointName == "Forward")
                {
                    positionZ = transform.position.z - volumeSizeOffset.z;
                }
                else
                {
                    positionZ = transform.position.z + volumeSizeOffset.z;
                }

                UIVolumeFaces[i].Point = new Vector3(positionX, positionY, positionZ);
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
    }
}
