// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeFlex : VolumeGrid
    { 
        //[SerializeField]
        //private float minimumXDistanceBetween = 0.3f;

        //public float MinimumXDistanceBetween
        //{
        //    get => minimumXDistanceBetween;
        //    set => minimumXDistanceBetween = value;
        //}

        //[SerializeField]
        //private float maximumXDistanceBetween = 0.3f;

        //public float MaximumXDistanceBetween
        //{
        //    get => maximumXDistanceBetween;
        //    set => maximumXDistanceBetween = value;
        //}

        //[SerializeField]
        //private float minimumXRemainingSpace = 0.3f;

        //public float MinimumXRemainingSpace
        //{
        //    get => minimumXRemainingSpace;
        //    set => minimumXRemainingSpace = value;
        //}

        //[SerializeField]
        //private float maximumXRemainingSpace = 0.3f;

        //public float MaximumXRemainingSpace
        //{
        //    get => maximumXRemainingSpace;
        //    set => maximumXRemainingSpace = value;
        //}

        //[SerializeField]
        //private float yOffsetMargin = 0.1f;

        //public float YOffsetMargin
        //{
        //    get => yOffsetMargin;
        //    set
        //    {
        //        yOffsetMargin = value;
        //    }
        //}

        //[SerializeField]
        //private float minXContainerDistance = 3f;

        //public float MinXContainerDistance
        //{
        //    get => minXContainerDistance;
        //    set => minXContainerDistance = value;
        //}

        //[SerializeField]
        //private float maxXContainerDistance = 3f;

        //public float MaxXContainerDistance
        //{
        //    get => maxXContainerDistance;
        //    set => maxXContainerDistance = value;
        //}

        //protected override void OnDrawGizmos()
        //{
        //    base.OnDrawGizmos();

        //    if (DirectChildUIVolumes.Count >= 2)
        //    {
        //        for (int i = 0; i < DirectChildUIVolumes.Count; i++)
        //        {
        //            Vector3 point1;
        //            Vector3 point2;

        //            point1 = DirectChildUIVolumes[i].GetFacePoint(FacePoint.Right);

        //            if ((i + 1) != DirectChildUIVolumes.Count)
        //            {
        //                point2 = DirectChildUIVolumes[i + 1].GetFacePoint(FacePoint.Left);
        //            }
        //            else
        //            {
        //                point2 = point1;
        //            }

        //            Gizmos.DrawLine(point1, point2);
        //        }
        //    }
        //}

        //protected override void Start()
        //{
        //    base.Start();
        //}

        //public override void Update()
        //{
        //    base.Update();

        //    //float xDistance = GetCurrentXDistance();

        //    //float yOffset = DirectChildUIVolumes[0].GetAxisDistance(VolumeAxis.Y) + YOffsetMargin;

        //    //float currentContainerWidth = GetAxisDistance(VolumeAxis.X);

        //    //float remainingSpace = GetRemainingSpace(VolumeAxis.X, GetChildUIVolumes());

        //    //if (remainingSpace >= MinimumXRemainingSpace && remainingSpace <= MaximumXRemainingSpace)
        //    //{
        //    //    Distribute(VolumeAxis.X);
        //    //}
        //    //else if (remainingSpace <= MinimumXRemainingSpace)
        //    //{
        //    //    List<Volume> transformsRow1 = DirectChildUIVolumes.Take(DirectChildUIVolumes.Count / 2).ToList();

        //    //    RowDistribution(VolumeAxis.X, transformsRow1);

        //    //    List<Volume> transformsRow2 = DirectChildUIVolumes.Skip(DirectChildUIVolumes.Count / 2).Take(DirectChildUIVolumes.Count / 2).ToList();

        //    //    RowDistribution(VolumeAxis.X, transformsRow2, true);
        //    //}
        //}

        //public float GetCurrentXDistance()
        //{
        //    // Get the x distance between the current objects
        //    if (DirectChildUIVolumes.Count >= 2)
        //    {
        //        List<Vector3> distances = new List<Vector3>();

        //        for (int i = 0; i < DirectChildUIVolumes.Count; i++)
        //        {
        //            Vector3 point1;
        //            Vector3 point2;

        //            point1 = DirectChildUIVolumes[i].GetFacePoint(FacePoint.Right);
                    
        //            if ((i + 1) != DirectChildUIVolumes.Count)
        //            {
        //                point2 = DirectChildUIVolumes[i + 1].GetFacePoint(FacePoint.Left);
        //            }
        //            else
        //            {
        //                point2 = point1;
        //            }

        //            float distanceX = Vector3.Distance(point1, point2);

        //            Vector3 distance = new Vector3(distanceX, 0, 0);

        //            distances.Add(distance);
        //        }

        //        return distances[0].x;
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}

        //public void RowDistribution(VolumeAxis axis, List<Volume> childrenToDistribute, bool YSpace = false)
        //{
        //    //Vector3 allChildSize = GetAxisSurfaceArea(childrenToDistribute);

        //    //// Offset the first item to appear in the container
        //    //Bounds bounds = childrenToDistribute[0].transform.GetColliderBounds();

        //    //Vector3 startPosition;
        //    //float axisPriorityDistance = GetAxisDistance(axis);
        //    //float remainingSpace;

        //    //if (axis == VolumeAxis.X)
        //    //{
        //    //    startPosition = GetFacePoint(FacePoint.Left) + (Vector3.right * bounds.extents.x);

        //    //    if (SecondaryAxis == VolumeAxis.Y && YSpace)
        //    //    {
        //    //        startPosition.y -= YOffsetMargin;
        //    //    }
                
        //    //    remainingSpace = axisPriorityDistance - allChildSize.x;
        //    //}
        //    //else if (axis == VolumeAxis.Y)
        //    //{
        //    //    startPosition = GetFacePoint(FacePoint.Top) + (Vector3.up * bounds.extents.y);
        //    //    remainingSpace = axisPriorityDistance - allChildSize.y;
        //    //}
        //    //else
        //    //{
        //    //    startPosition = GetFacePoint(FacePoint.Forward) + (Vector3.forward * bounds.extents.z);
        //    //    remainingSpace = axisPriorityDistance - allChildSize.z;
        //    //}

        //    //float increment = (allChildSize.x + remainingSpace) / childrenToDistribute.Count;

        //    //foreach (Volume child in childrenToDistribute)
        //    //{
        //    //    if (child.UseAnchorPositioning)
        //    //    {
        //    //        child.UseAnchorPositioning = false;
        //    //    }

        //    //    Vector3 newPosition = startPosition;

        //    //    if (newPosition.IsValidVector())
        //    //    {
        //    //        child.transform.position = DistributeSmoothing.Smoothing && Application.isPlaying ?
        //    //            Vector3.Lerp(child.transform.position, newPosition, DistributeSmoothing.LerpTime * Time.deltaTime)
        //    //            : newPosition;
        //    //    }

        //    //    if (axis == VolumeAxis.X)
        //    //    {
        //    //        startPosition.x += increment;
        //    //    }
        //    //    else if (axis == VolumeAxis.Y)
        //    //    {
        //    //        startPosition.y += increment;
        //    //    }
        //    //    else
        //    //    {
        //    //        startPosition.z += increment;
        //    //    }
        //    //}
        //}

        //private Vector3 GetAxisSurfaceArea(List<Volume> surfaceAreaList)
        //{
        //    List<Vector3> axisSizes = new List<Vector3>();

        //    foreach (Volume childVolume in surfaceAreaList)
        //    {
        //        // Make sure each transform has this volume as the parent
        //        if (childVolume.UIVolumeParentTransform != transform)
        //        {
        //            Debug.LogError($"{childVolume.UIVolumeParentTransform.gameObject} does not have {gameObject.name} as the parent");
        //        }

        //        Vector3 volumeSize = new Vector3(childVolume.GetAxisDistance(VolumeAxis.X), childVolume.GetAxisDistance(VolumeAxis.Y), childVolume.GetAxisDistance(VolumeAxis.Z));

        //        axisSizes.Add(volumeSize);
        //    }

        //    Vector3 allChildSize = new Vector3(0, 0, 0);

        //    // Get total child width based on container distance
        //    axisSizes.ForEach((item) => allChildSize.x += item.x);
        //    axisSizes.ForEach((item) => allChildSize.y += item.y);
        //    axisSizes.ForEach((item) => allChildSize.z += item.z);

        //    return allChildSize;
        //}
        
        //public float GetRemainingSpace(VolumeAxis axis, List<Volume> uiVolume)
        //{
        //    if (axis == VolumeAxis.X)
        //    {
        //        return GetAxisDistance(VolumeAxis.X) - GetAxisSurfaceArea(uiVolume).x;
        //    }
        //    else if(axis == VolumeAxis.Y)
        //    {
        //        return GetAxisDistance(VolumeAxis.Y) - GetAxisSurfaceArea(uiVolume).y;
        //    }
        //    else
        //    {
        //        return GetAxisDistance(VolumeAxis.Z) - GetAxisSurfaceArea(uiVolume).z;
        //    }
        //}
    }
}
