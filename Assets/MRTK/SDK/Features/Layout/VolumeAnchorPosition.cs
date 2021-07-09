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

        [SerializeField]
        private bool useScaleConversion = false;

        public bool UseScaleConversion
        {
            get => useScaleConversion;
            set => useScaleConversion = value;
        }

        [SerializeField]
        private Vector3 scaleConversion = new Vector3();

        public Vector3 ScaleConversion
        {
            get => scaleConversion;
            set => scaleConversion = value;
        }

        #region MonoBehaviour Methods

        public override void Update()
        {
            base.Update();

            if (Application.isPlaying)
            {
                UpdateSizingBehaviors();
            }
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

        private void SetMatchParentVolumeSize(VolumeAxis volumeAxis, bool includeScaleFactor = true)
        {
            if (VolumeParent)
            {
                Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

                switch (volumeAxis)
                {
                    case VolumeAxis.X:
                        newScale.x = GetNewSizeMatch(volumeAxis, includeScaleFactor);
                        break;
                    case VolumeAxis.Y:
                        newScale.y = GetNewSizeMatch(volumeAxis, includeScaleFactor);
                        break;
                    case VolumeAxis.Z:
                        newScale.z = GetNewSizeMatch(volumeAxis, includeScaleFactor);
                        break;
                }

                transform.localScale = newScale;
                Volume.OnVolumeSizeChanged.Invoke();
            }
        }

        private float GetNewSizeMatch(VolumeAxis volumeAxis, bool includeScaleFactor)
        {
            float newAxisSizeValue = 0;

            float volumeParentLocalScale = GetScaleFromVolumeAxis(volumeAxis, VolumeParentTransform.localScale);
            float volumeParentLossyScale = GetScaleFromVolumeAxis(volumeAxis, VolumeParentTransform.lossyScale);
            float volumeScaleFactor = GetScaleFactorFromVolumeAxis(volumeAxis);
            float volumeParentSize = GetVolumeSizeFromVolumeAxis(volumeAxis, VolumeParent);

            switch (VolumeSizeOrigin)
            {
                case VolumeSizeOrigin.ColliderBounds:
                    newAxisSizeValue = includeScaleFactor ? 
                        (volumeParentLocalScale / volumeParentLossyScale) * volumeScaleFactor
                        : (volumeParentLocalScale / volumeParentLossyScale);
                    break;
                case VolumeSizeOrigin.LocalScale:
                    newAxisSizeValue = includeScaleFactor ? volumeParentLocalScale * volumeScaleFactor : volumeParentLocalScale;
                    break;
                case VolumeSizeOrigin.RendererBounds:
                    break;
                case VolumeSizeOrigin.Custom:
                    newAxisSizeValue = includeScaleFactor ? volumeParentSize * volumeScaleFactor : volumeParentSize;
                    break;
                case VolumeSizeOrigin.LossyScale:
                default:
                    newAxisSizeValue = includeScaleFactor ? volumeScaleFactor : 1;
                    break;
            }

            if (UseScaleConversion)
            {
                newAxisSizeValue *= ScaleConversion.x;
            }

            return newAxisSizeValue;
        }


        private float GetScaleFromVolumeAxis(VolumeAxis volumeAxis, Vector3 scale)
        {
            if (volumeAxis == VolumeAxis.X)
            {
                return scale.x;
            }
            else if (volumeAxis == VolumeAxis.Y)
            {
                return scale.y;
            }
            else
            {
                return scale.z;
            }
        }

        private float GetScaleFactorFromVolumeAxis(VolumeAxis volumeAxis)
        {
            if (volumeAxis == VolumeAxis.X)
            {
                return VolumeSizeScaleFactorX;
            }
            else if (volumeAxis == VolumeAxis.Y)
            {
                return VolumeSizeScaleFactorY;
            }
            else
            {
                return VolumeSizeScaleFactorZ;
            }
        }

        private float GetVolumeSizeFromVolumeAxis(VolumeAxis volumeAxis, BaseVolume volume)
        {
            if (volumeAxis == VolumeAxis.X)
            {
                return volume.VolumeSize.x;
            }
            else if (volumeAxis == VolumeAxis.Y)
            {
                return volume.VolumeSize.y;
            }
            else
            {
                return volume.VolumeSize.z;
            }
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
                    VolumePosition = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(VolumePosition, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
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
