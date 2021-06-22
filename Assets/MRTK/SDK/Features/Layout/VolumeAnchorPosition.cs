using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeAnchorPosition : BaseCustomVolume
    {
        [SerializeField]
        private XAnchorPosition xAnchorPosition;

        public XAnchorPosition XAnchorPosition
        {
            get => xAnchorPosition;
            set => xAnchorPosition = value;
        }

        [SerializeField]
        private YAnchorPosition yAnchorPosition;

        public YAnchorPosition YAnchorPosition
        {
            get => yAnchorPosition;
            set => yAnchorPosition = value;
        }

        [SerializeField]
        private ZAnchorPosition zAnchorPosition;

        public ZAnchorPosition ZAnchorPosition
        {
            get => zAnchorPosition;
            set => zAnchorPosition = value;
        }

        [SerializeField]
        private VolumeItemSmoothing anchorPositionSmoothing = new VolumeItemSmoothing();

        public VolumeItemSmoothing AnchorPositionSmoothing
        {
            get => anchorPositionSmoothing;
            set => anchorPositionSmoothing = value;
        }

        #region Volume Size Properties

        [SerializeField]
        private bool fillToParentY = false;

        public bool FillToParentY
        {
            get => fillToParentY;
            set => fillToParentY = value;
        }

        [SerializeField]
        private bool fillToParentX = false;

        public bool FillToParentX
        {
            get => fillToParentX;
            set => fillToParentX = value;
        }

        [SerializeField]
        private bool fillToParentZ = false;

        public bool FillToParentZ
        {
            get => fillToParentZ;
            set => fillToParentZ = value;
        }

        [SerializeField]
        private float volumeSizeScaleFactorX = 1;

        public float VolumeSizeScaleFactorX
        {
            get => volumeSizeScaleFactorX;
            set => volumeSizeScaleFactorX = value;
        }

        [SerializeField]
        private float volumeSizeScaleFactorY = 1;

        public float VolumeSizeScaleFactorY
        {
            get => volumeSizeScaleFactorY;
            set => volumeSizeScaleFactorY = value;
        }

        [SerializeField]
        private float volumeSizeScaleFactorZ = 1;

        public float VolumeSizeScaleFactorZ
        {
            get => volumeSizeScaleFactorZ;
            set => volumeSizeScaleFactorZ = value;
        }

        #endregion

        [SerializeField]
        private bool useAnchorPositioning = false;

        public bool UseAnchorPositioning
        {
            get => useAnchorPositioning;
            set => useAnchorPositioning = value;
        }

        [SerializeField]
        private GameObject backPlateObject;

        public GameObject BackPlateObject
        {
            get => backPlateObject;
            set => backPlateObject = value;
        }

        #region MonoBehaviour Methods

        public override void Update()
        {
            base.Update();

            UpdateSizingBehaviors();
        }

        public void UpdateSizingBehaviors()
        {
            UpdateVolumeSizeMatch();

            UpdateAnchorPosition();

            if (BackPlateObject != null)
            {
                UpdateTextBackPlateSize();
            }
        }

        #endregion

        private void UpdateVolumeSizeMatch()
        {
            if (FillToParentX)
            {
                SetMatchParentVolumeSize(VolumeAxis.X);
            }

            if (FillToParentY)
            {
                SetMatchParentVolumeSize(VolumeAxis.Y);
            }

            if (FillToParentZ)
            {
                SetMatchParentVolumeSize(VolumeAxis.Z);
            }
        }

        private void SetMatchParentVolumeSize(VolumeAxis axis, bool includeScaleFactor = true)
        {
            Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

            Transform VolumeParentTransform = Volume.VolumeParentTransform;

            if (axis == VolumeAxis.X)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.x = includeScaleFactor ? (VolumeParentTransform.localScale.x / VolumeParentTransform.transform.lossyScale.x) * volumeSizeScaleFactorX : (VolumeParentTransform.localScale.x / VolumeParentTransform.transform.lossyScale.x);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.x *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.x = includeScaleFactor ? VolumeParentTransform.localScale.x * volumeSizeScaleFactorX : VolumeParentTransform.localScale.x;
                
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
                {
                    newScale.x = includeScaleFactor ? 1 * volumeSizeScaleFactorX: 1;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Width = includeScaleFactor ? VolumeParent.VolumeSize.x * volumeSizeScaleFactorX : VolumeParent.VolumeSize.x;
                }
            }

            if (axis == VolumeAxis.Y)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.y = includeScaleFactor ? (VolumeParentTransform.localScale.y / VolumeParentTransform.transform.lossyScale.y) * volumeSizeScaleFactorY : (VolumeParentTransform.localScale.y / VolumeParentTransform.transform.lossyScale.y);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.y *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.y = includeScaleFactor ? VolumeParentTransform.localScale.y * volumeSizeScaleFactorY : VolumeParentTransform.localScale.y;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
                {
                    newScale.y = includeScaleFactor ? 1 * volumeSizeScaleFactorY : 1;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Height = includeScaleFactor ? VolumeParent.VolumeSize.y * volumeSizeScaleFactorY : VolumeParent.VolumeSize.y;
                }
            }

            if (axis == VolumeAxis.Z)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.z = includeScaleFactor ? (VolumeParentTransform.localScale.z / VolumeParentTransform.transform.lossyScale.z) * volumeSizeScaleFactorZ : (VolumeParentTransform.localScale.z / VolumeParentTransform.transform.lossyScale.z);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.z *= 62.5f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.z = includeScaleFactor ? VolumeParentTransform.localScale.z * volumeSizeScaleFactorZ : VolumeParentTransform.localScale.z;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
                {
                    newScale.z = includeScaleFactor ? 1 * volumeSizeScaleFactorZ : 1;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Depth = includeScaleFactor ? VolumeParent.VolumeSize.z * volumeSizeScaleFactorZ : VolumeParent.VolumeSize.z;
                }
            }

            transform.localScale = newScale;
            Volume.OnVolumeSizeChanged.Invoke();
        }

        public void EqualizeVolumeSizeToParent()
        {
            var axisValues = Enum.GetValues(typeof(VolumeAxis));

            foreach (VolumeAxis axis in axisValues)
            {
                SetMatchParentVolumeSize(axis, false);
            }
        }

        public void UpdateAnchorPosition()
        {
            if (UseAnchorPositioning && !Volume.IsRootVolume)
            {
                Vector3 newPosition = Vector3.zero;
                Vector3 marginDifference = Volume.GetMarginDifference() * 0.5f;

                switch (XAnchorPosition)
                {
                    case XAnchorPosition.Left:
                        newPosition = VolumeParent.GetFacePoint(FacePoint.Left) + (VolumeParent.VolumeBounds.Right * (VolumeBounds.Extents.x + marginDifference.x));
                        break;
                    case XAnchorPosition.Center:
                        newPosition = VolumeParent.VolumeCenter;
                        break;
                    case XAnchorPosition.Right:
                        newPosition = VolumeParent.GetFacePoint(FacePoint.Right) + (VolumeParent.VolumeBounds.Left * (VolumeBounds.Extents.x + marginDifference.x));
                        break;
                }

                switch (YAnchorPosition)
                {
                    case YAnchorPosition.Top:
                        newPosition += (VolumeParent.VolumeBounds.Up * VolumeParent.VolumeBounds.Extents.y) + (VolumeParent.VolumeBounds.Down * (VolumeBounds.Extents.y + marginDifference.y));
                        break;
                    case YAnchorPosition.Center:
                        break;
                    case YAnchorPosition.Bottom:
                        newPosition += (VolumeParent.VolumeBounds.Down * VolumeParent.VolumeBounds.Extents.y) + (VolumeParent.VolumeBounds.Up * (VolumeBounds.Extents.y + marginDifference.y));
                        break;
                }

                switch (ZAnchorPosition)
                {
                    case ZAnchorPosition.Forward:
                        newPosition += (VolumeParent.VolumeBounds.Forward * VolumeParent.VolumeBounds.Extents.z) + (VolumeParent.VolumeBounds.Back * (VolumeBounds.Extents.z + marginDifference.z));
                        break;
                    case ZAnchorPosition.Center:
                        break;
                    case ZAnchorPosition.Back:
                        newPosition += (VolumeParent.VolumeBounds.Back * VolumeParent.VolumeBounds.Extents.z) + (VolumeParent.VolumeBounds.Forward * (VolumeBounds.Extents.z + marginDifference.z));
                        break;
                }

                if (IsValidVector(newPosition))
                {
                    //VolumePosition = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(VolumePosition, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
                    
                    //if (VolumeSizeOrigin != VolumeSizeOrigin.RendererBounds)
                    {
                        VolumePosition = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(VolumePosition, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
                    }
                    //else
                    //{
                    //    //VolumePosition = newPosition - (Vector3.left * (newPosition - VolumeCenter).x); //- (newPosition - VolumeCenter);
                    //}
                }
            }
        }

        public bool IsTextBackPlateCompatible()
        {
            if (gameObject.GetComponent<TextMeshPro>() != null)
            {
                return true;
            }

            return false;
        }

        public void UpdateTextBackPlateSize()
        {
            if (IsTextBackPlateCompatible() && BackPlateObject != null)
            {
                TMP_Text text = transform.GetComponent<TMP_Text>();

                Vector2 textSize = text.GetPreferredValues();

                BackPlateObject.transform.localPosition = text.textBounds.center + (VolumeBounds.Back * 0.5f);
                BackPlateObject.transform.rotation = transform.rotation;

                BackPlateObject.transform.localScale = (textSize);
                BackPlateObject.transform.localScale += Vector3.forward * 0.01f;
                BackPlateObject.transform.localScale += Vector3.one;
            }
        }

        public bool IsValidVector(Vector3 vector)
        {
            return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
                   !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
        }
    }
}
