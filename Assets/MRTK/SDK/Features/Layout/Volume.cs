// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class Volume : BaseVolume
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

        // Scale animation when switching volumes
        private Coroutine scaleCoroutine;
        private float scaleStartTime;

        #region MonoBehaviour Methods

        protected virtual void Start() { }

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

            if (axis == VolumeAxis.X)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.x = includeScaleFactor ? (UIVolumeParentTransform.localScale.x / UIVolumeParentTransform.transform.lossyScale.x) * volumeSizeScaleFactorX : (UIVolumeParentTransform.localScale.x / UIVolumeParentTransform.transform.lossyScale.x);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.x *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale || VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
                {
                    newScale.x = includeScaleFactor ? UIVolumeParentTransform.localScale.x * volumeSizeScaleFactorX : UIVolumeParentTransform.localScale.x;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Width = includeScaleFactor ? UIVolumeParent.VolumeSize.x * volumeSizeScaleFactorX : UIVolumeParent.VolumeSize.x;
                }
            }

            if (axis == VolumeAxis.Y)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.y = includeScaleFactor ? (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y) * volumeSizeScaleFactorY : (UIVolumeParentTransform.localScale.y / UIVolumeParentTransform.transform.lossyScale.y);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.y *= 31.25f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.y = includeScaleFactor ? UIVolumeParentTransform.localScale.y * volumeSizeScaleFactorY : UIVolumeParentTransform.localScale.y;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Height = includeScaleFactor ? UIVolumeParent.VolumeSize.y * volumeSizeScaleFactorY : UIVolumeParent.VolumeSize.y;
                }
            }

            if (axis == VolumeAxis.Z)
            {
                if (VolumeSizeOrigin == VolumeSizeOrigin.ColliderBounds)
                {
                    newScale.z = includeScaleFactor ? (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z) * volumeSizeScaleFactorZ : (UIVolumeParentTransform.localScale.z / UIVolumeParentTransform.transform.lossyScale.z);

                    // The scale of buttons is 1 but the actual size is 32mm x 32mm and a conversion is needed
                    // Check if the prefab is a flexible button
                    if (gameObject.name.Contains("Flexible"))
                    {
                        newScale.z *= 62.5f;
                    }
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.z = includeScaleFactor ? UIVolumeParentTransform.localScale.z * volumeSizeScaleFactorZ : UIVolumeParentTransform.localScale.z;
                }
                else if (VolumeSizeOrigin == VolumeSizeOrigin.Custom)
                {
                    VolumeBounds.Depth = includeScaleFactor ? UIVolumeParent.VolumeSize.z * volumeSizeScaleFactorZ : UIVolumeParent.VolumeSize.z;
                }
            }

            transform.localScale = newScale;
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
            if (UseAnchorPositioning && !IsRootUIVolume)
            {
                Vector3 newPosition = Vector3.zero;
                Vector3 marginDifference = GetMarginDifference() * 0.5f;

                switch (XAnchorPosition)
                {
                    case XAnchorPosition.Left:
                        newPosition = UIVolumeParent.GetFacePoint(FacePoint.Left) + (UIVolumeParent.VolumeBounds.Right * (VolumeBounds.Extents.x + marginDifference.x));
                        break;
                    case XAnchorPosition.Center:
                        newPosition = UIVolumeParent.VolumeCenter;
                        break;
                    case XAnchorPosition.Right:
                        newPosition = UIVolumeParent.GetFacePoint(FacePoint.Right) + (UIVolumeParent.VolumeBounds.Left * (VolumeBounds.Extents.x + marginDifference.x));
                        break;
                }

                switch (YAnchorPosition)
                {
                    case YAnchorPosition.Top:
                        newPosition += (UIVolumeParent.VolumeBounds.Up * UIVolumeParent.VolumeBounds.Extents.y) + (UIVolumeParent.VolumeBounds.Down * (VolumeBounds.Extents.y + marginDifference.y));
                        break;
                    case YAnchorPosition.Center:
                        break;
                    case YAnchorPosition.Bottom:
                        newPosition += (UIVolumeParent.VolumeBounds.Down * UIVolumeParent.VolumeBounds.Extents.y) + (UIVolumeParent.VolumeBounds.Up * (VolumeBounds.Extents.y + marginDifference.y));
                        break;
                }

                switch (ZAnchorPosition)
                {
                    case ZAnchorPosition.Forward:
                        newPosition += (UIVolumeParent.VolumeBounds.Forward * UIVolumeParent.VolumeBounds.Extents.z) + (UIVolumeParent.VolumeBounds.Back * (VolumeBounds.Extents.z + marginDifference.z));
                        break;
                    case ZAnchorPosition.Center:
                        break;
                    case ZAnchorPosition.Back:
                        newPosition += (UIVolumeParent.VolumeBounds.Back * UIVolumeParent.VolumeBounds.Extents.z) + (UIVolumeParent.VolumeBounds.Forward * (VolumeBounds.Extents.z + marginDifference.z));
                        break;
                }

                if (IsValidVector(newPosition))
                {
                    VolumePosition = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(VolumePosition, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
                }
            }
        }

        public void SwitchChildVolumes(Volume child, Volume targetVolume)
        {
            Volume volumeToSwitch = DirectChildUIVolumes.Find((item) => item == child);
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

        private void PrintMatrix()
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
