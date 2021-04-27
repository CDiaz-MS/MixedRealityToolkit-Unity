// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    [ExecuteAlways]
    public class UIVolume : BaseVolume
    {
        [SerializeField]
        private AnchorLocation anchorLocation;

        public AnchorLocation AnchorLocation
        {
            get => anchorLocation;
            set => anchorLocation = value;
        }

        [SerializeField]
        private VolumeItemSmoothing anchorPositionSmoothing = new VolumeItemSmoothing();

        public VolumeItemSmoothing AnchorPositionSmoothing
        {
            get => anchorPositionSmoothing;
            set => anchorPositionSmoothing = value;
        }

        #region Distribute Properties

        // X Axis
        [SerializeField]
        private bool useCustomStartPosition = false;

        public bool UseCustomStartPosition
        {
            get => useCustomStartPosition;
            set => useCustomStartPosition = value;
        }

        // X Axis
        [SerializeField]
        private Vector3 distributionStartPosition;

        public Vector3 DistributionStartPosition
        {
            get => distributionStartPosition;
            set => distributionStartPosition = value;
        }

        // X Axis
        [SerializeField]
        private bool xAxisDynamicDistribute = false;

        public bool XAxisDynamicDistribute
        {
            get => xAxisDynamicDistribute;
            set => xAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float leftMargin = 0;

        // X Axis
        public float LeftMargin
        {
            get => leftMargin;
            set => leftMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillX = new DistributeAxisFill(VolumeAxis.X);

        public DistributeAxisFill DistributeContainerFillX
        {
            get => distributeContainerFillX;
            set => distributeContainerFillX = value;
        }

        // Y Axis
        [SerializeField]
        private bool yAxisDynamicDistribute = false;

        public bool YAxisDynamicDistribute
        {
            get => yAxisDynamicDistribute;
            set => yAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float topMargin = 0;

        public float TopMargin
        {
            get => topMargin;
            set => topMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillY = new DistributeAxisFill(VolumeAxis.Y);

        public DistributeAxisFill DistributeContainerFillY
        {
            get => distributeContainerFillY;
            set => distributeContainerFillY = value;
        }

        [SerializeField]
        private bool zAxisDynamicDistribute = false;

        public bool ZAxisDynamicDistribute
        {
            get => zAxisDynamicDistribute;
            set => zAxisDynamicDistribute = value;
        }

        [SerializeField]
        private float forwardMargin = 0;

        public float ForwardMargin
        {
            get => forwardMargin;
            set => forwardMargin = value;
        }

        [SerializeField]
        private DistributeAxisFill distributeContainerFillZ = new DistributeAxisFill(VolumeAxis.Z);

        public DistributeAxisFill DistributeContainerFillZ
        {
            get => distributeContainerFillZ;
            set => distributeContainerFillZ = value;
        }

        [SerializeField]
        private float backMargin = 0;

        public float BackMargin
        {
            get => backMargin;
            set => backMargin = value;
        }

        [SerializeField]
        private VolumeItemSmoothing distributeSmoothing = new VolumeItemSmoothing();

        public VolumeItemSmoothing DistributeSmoothing
        {
            get => distributeSmoothing;
            set => distributeSmoothing = value;
        }

        #endregion

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
        private bool useAnchorPositioning = true;

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

        public Vector3 VolumeCenter => transform.position;

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
            UpdateDistribution();

            UpdateVolumeSizeMatch();

            UpdateAnchorLocation(AnchorLocation);
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
                else if (VolumeSizeOrigin == VolumeSizeOrigin.LocalScale)
                {
                    newScale.x = includeScaleFactor ? UIVolumeParentTransform.localScale.x * volumeSizeScaleFactorX : UIVolumeParentTransform.localScale.x;
                }
                else // Text Mesh Pro
                {
                    RectTransform rectTransform = transform as RectTransform;

                    float parentChildVolumeRatio = GetTextVolumeRatio(axis);

                    if (parentChildVolumeRatio != 0)
                    {
                        rectTransform.sizeDelta = new Vector2((rectTransform.sizeDelta.x * parentChildVolumeRatio) * volumeSizeScaleFactorX, rectTransform.sizeDelta.y);
                    }
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
                else // Text Mesh Pro
                {
                    RectTransform rectTransform = transform as RectTransform;

                    float parentChildVolumeRatio = GetTextVolumeRatio(axis);

                    if (parentChildVolumeRatio != 0)
                    {
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (rectTransform.sizeDelta.y * parentChildVolumeRatio) * volumeSizeScaleFactorY);
                    }
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

        private float GetTextVolumeRatio(VolumeAxis axis)
        {
            RectTransform rectTransform = transform as RectTransform;

            float rectSize;

            if (axis == VolumeAxis.X)
            {
                rectSize = rectTransform.rect.width;
            }
            else if (axis == VolumeAxis.Y)
            {
                rectSize = rectTransform.rect.height;
            }
            else
            {
                // Z axis scaling is not supported for rect transforms
                return 0;
            }

            float childVolumeAxisDistance = GetAxisDistance(axis);

            float parentVolumeAxisDistance = UIVolumeParentTransform.GetComponent<UIVolume>().GetAxisDistance(axis);

            float parentChildVolumeRatio = parentVolumeAxisDistance / childVolumeAxisDistance;

            return parentChildVolumeRatio;
        }

        public void UpdateAnchorLocation(AnchorLocation anchorLocation)
        {
            if (UseAnchorPositioning && !IsRootUIVolume)
            {
                Vector3 volumeSizeOffsetParent = UIVolumeParent.GetVolumeSizeOffset();
                Vector3 volumeSizeOffset = GetVolumeSizeOffset();

                Vector3 newPosition = Vector3.zero;

                string[] anchorParsed = NameParse(anchorLocation.ToString());

                // Y Position
                if (anchorParsed[0] == "Top")
                {
                    newPosition.y = UIVolumeParentTransform.position.y + (volumeSizeOffsetParent.y) + -(volumeSizeOffset.y);
                }
                else if (anchorParsed[0] == "Center")
                {
                    newPosition.y = UIVolumeParentTransform.position.y;
                }
                else
                {
                    newPosition.y = UIVolumeParentTransform.position.y + -(volumeSizeOffsetParent.y) + (volumeSizeOffset.y);
                }

                // X Position
                if (anchorParsed[1] == "Left")
                {
                    newPosition.x = UIVolumeParentTransform.position.x + -(volumeSizeOffsetParent.x) + (volumeSizeOffset.x);
                }
                else if (anchorParsed[1] == "Center")
                {
                    newPosition.x = UIVolumeParentTransform.position.x;
                }
                else
                {
                    newPosition.x = UIVolumeParentTransform.position.x + (volumeSizeOffsetParent.x) + -(volumeSizeOffset.x);
                }

                // Z Position
                if (anchorParsed[2] == "Forward")
                {
                    newPosition.z = UIVolumeParentTransform.position.z + -(volumeSizeOffsetParent.z) + (volumeSizeOffset.z);
                }
                else if (anchorParsed[2] == "Center")
                {
                    newPosition.z = UIVolumeParentTransform.position.z;
                }
                else
                {
                    newPosition.z = UIVolumeParentTransform.position.z + (volumeSizeOffsetParent.z) + -(volumeSizeOffset.z);
                }

                if (newPosition.IsValidVector())
                {
                    transform.position = AnchorPositionSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(transform.position, newPosition, AnchorPositionSmoothing.LerpTime * Time.deltaTime) : newPosition;
                }
            }
        }

        #region Axis Distribution

        public void Distribute(VolumeAxis axis)
        {
            if (gameObject.transform.childCount > 0)
            {
                List<ChildVolumeItem> itemsToDistribute = ChildVolumeItems;

                // Remove the back plate from the distribution calculation
                foreach (var item in itemsToDistribute.ToList())
                {
                    if (BackPlateObject != null)
                    {
                        if (item.Transform == BackPlateObject.transform)
                        {
                            itemsToDistribute.Remove(item);
                        }
                    }

                    if (!item.Transform.gameObject.activeSelf)
                    {
                        itemsToDistribute.Remove(item);
                    }
                }

                float placementIncrement = GetAxisDistance(axis) / itemsToDistribute.Count;
                float startPlacement;

                Vector3 volumeSizeOffset = itemsToDistribute[0].UIVolume != null ? itemsToDistribute[0].UIVolume.GetVolumeSizeOffset() : itemsToDistribute[0].Transform.GetColliderBounds().extents;

                Vector3 volumeAxisDistance = GetAxisDistances();

                if (!UseCustomStartPosition)
                {
                    if (axis == VolumeAxis.X)
                    {
                        startPlacement = GetFacePoint(FacePoint.Left).x + volumeSizeOffset.x + (LeftMargin * volumeAxisDistance.x);
                    }
                    else if (axis == VolumeAxis.Y)
                    {
                        startPlacement = GetFacePoint(FacePoint.Top).y - volumeSizeOffset.y - (TopMargin * volumeAxisDistance.y);
                    }
                    else // Z
                    {
                        startPlacement = GetFacePoint(FacePoint.Forward).z + volumeSizeOffset.z + (ForwardMargin * volumeAxisDistance.z);
                    }
                }
                else
                {
                    startPlacement = DistributionStartPosition.x + volumeSizeOffset.x + (LeftMargin * volumeAxisDistance.x);
                }


                foreach (var item in itemsToDistribute)
                {
                    //  Anchor positioning cannot be active for distribution placement
                    if (item.UIVolume != null)
                    {
                        item.UIVolume.UseAnchorPositioning = false;
                    }

                    float newPositionX = axis == VolumeAxis.X ? startPlacement : transform.position.x;
                    float newPositionY = axis == VolumeAxis.Y ? startPlacement : transform.position.y;
                    float newPositionZ = axis == VolumeAxis.Z ? startPlacement : transform.position.z;

                    Vector3 newPosition = new Vector3(newPositionX, newPositionY, newPositionZ);

                    if (newPosition.IsValidVector())
                    {
                        item.Transform.position = DistributeSmoothing.Smoothing && Application.isPlaying ? Vector3.Lerp(item.Transform.position, newPosition, DistributeSmoothing.LerpTime * Time.deltaTime) : newPosition;
                    }

                    // Reverse the Y Axis, start placement from the top of the container
                    if (axis == VolumeAxis.Y)
                    {
                        startPlacement -= placementIncrement;
                    }
                    else
                    {
                        startPlacement += placementIncrement;
                    }
                }

                UpdateContainerFill(axis, itemsToDistribute);

                UIVolumeParentTransform.GetComponent<UIVolume>().UpdateCornerPoints();
                UIVolumeParentTransform.GetComponent<UIVolume>().UpdateFacePoints();
            }
        }

        private void UpdateDistribution()
        {
            if (XAxisDynamicDistribute)
            {
                Distribute(VolumeAxis.X);
            }

            if (YAxisDynamicDistribute)
            {
                Distribute(VolumeAxis.Y);
            }

            if (ZAxisDynamicDistribute)
            {
                Distribute(VolumeAxis.Z);
            }

            UpdatePoints();
        }

        private void UpdateContainerFill(VolumeAxis distributeAxis, List<ChildVolumeItem> childItems)
        {
            foreach (var item in childItems)
            {
                if (item.UIVolume != null)
                {
                    if (distributeAxis == VolumeAxis.X)
                    {
                        distributeContainerFillX.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
                    }

                    if (distributeAxis == VolumeAxis.Y)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
                    }

                    if (distributeAxis == VolumeAxis.Z)
                    {
                        distributeContainerFillY.UpdateDistributeContainerFillAxis(distributeAxis, item.UIVolume, childItems.Count);
                    }
                }
            }
        }

        internal void ResetContainerFillProperties(VolumeAxis axis)
        {
            if (axis == VolumeAxis.X)
            {
                distributeContainerFillX.ResetContainerFillProperties(this);
            }

            if (axis == VolumeAxis.Y)
            {
                distributeContainerFillY.ResetContainerFillProperties(this);
            }

            if (axis == VolumeAxis.Z)
            {
                distributeContainerFillZ.ResetContainerFillProperties(this);
            }
        }

        #endregion

        public void SwitchChildVolumes(UIVolume child, UIVolume targetVolume)
        {
            UIVolume volumeToSwitch = DirectChildUIVolumes.Find((item) => item == child);

            Vector3 pos = targetVolume.transform.position;

            volumeToSwitch.transform.SetParent(targetVolume.transform, true);

            Vector3 velocity = Vector3.zero;

            child.transform.position = Vector3.SmoothDamp(child.transform.position, pos, ref velocity ,10 * Time.deltaTime);

            if (VolumeSizeOrigin == VolumeSizeOrigin.LossyScale)
            {
                float diffX = targetVolume.transform.lossyScale.x / transform.lossyScale.x;
                float diffY = targetVolume.transform.lossyScale.y / transform.lossyScale.y;
                float diffZ = targetVolume.transform.lossyScale.z / transform.lossyScale.z;

                Vector3 diff = new Vector3(diffX, diffY, diffZ);

                Vector3 childScale = child.transform.localScale;

                Vector3 targetScale = Vector3.Scale(childScale, diff);

                scaleCoroutine = StartCoroutine(TransitionScale(child.transform, childScale,targetScale));
            }

            SyncChildObjects();
        }

        private IEnumerator TransitionScale(Transform currentTransform, Vector3 currentScale, Vector3 targetScale)
        {
            scaleStartTime = Time.time;
            float t = 0;
            while (t <= 1)
            {
                t += Time.deltaTime;
                currentTransform.localScale = Vector3.Lerp(currentScale, targetScale, t*1.5f);
                yield return null;
            }
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
