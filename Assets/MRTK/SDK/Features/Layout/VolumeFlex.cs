// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeFlex : BaseCustomVolume
    {
        [SerializeField]
        private Vector3 spacing = new Vector3(0.01f, 0.01f, 0.01f);

        public Vector3 Spacing
        {
            get => spacing;
            set => spacing = value;
        }

        [SerializeField]
        private StartingFlexPositionMode startingFlexPositionMode = StartingFlexPositionMode.CornerPoint;

        public StartingFlexPositionMode StartingFlexPositionMode
        {
            get => startingFlexPositionMode;
            set => startingFlexPositionMode = value;
        }

        [SerializeField]
        private CornerPoint startCornerPoint = CornerPoint.LeftTopForward;

        public CornerPoint StartCornerPoint
        {
            get => startCornerPoint;
            set => startCornerPoint = value;
        }

        [SerializeField]
        private FacePoint startFacePoint = FacePoint.Left;

        public FacePoint StartFacePoint
        {
            get => startFacePoint;
            set => startFacePoint = value;
        }

        [SerializeField]
        private float startingXPositionOffset = 0;

        public float StartingXPositionOffset
        {
            get => startingXPositionOffset;
            set => startingXPositionOffset = value;
        }

        [SerializeField]
        private float startingYPositionOffset = 0;

        public float StartingYPositionOffset
        {
            get => startingYPositionOffset;
            set => startingYPositionOffset = value;
        }

        [SerializeField]
        private float startingZPositionOffset = 0;

        public float StartingZPositionOffset
        {
            get => startingZPositionOffset;
            set => startingZPositionOffset = value;
        }

        [SerializeField]
        private VolumeFlowAxis primaryFlowAxis = VolumeFlowAxis.X;

        public VolumeFlowAxis PrimaryFlowAxis
        {
            get => primaryFlowAxis;
            set => primaryFlowAxis = value;
        }

        [SerializeField]
        private VolumeFlowAxis secondaryFlowAxis = VolumeFlowAxis.Y;

        public VolumeFlowAxis SecondaryFlowAxis
        {
            get => secondaryFlowAxis;
            set => secondaryFlowAxis = value;
        }

        [SerializeField]
        private VolumeFlowAxis tertiaryFlowAxis = VolumeFlowAxis.Z;

        public VolumeFlowAxis TertiaryFlowAxis
        {
            get => tertiaryFlowAxis;
            set => tertiaryFlowAxis = value;
        }

        public List<Vector3> positions = new List<Vector3>();

        private int index = 0;

        private int Place(VolumeFlowAxis currentAxis)
        {
            while (index < Volume.ChildVolumeCount)
            {
                Tuple<bool, Vector3> nextPositionTuple = IsNextPositionInBounds(currentAxis, positions[index - 1], ChildVolumeItems[index - 1], ChildVolumeItems[index]);

                bool isNextPositionInBounds = nextPositionTuple.Item1;
                Vector3 nextPosition = nextPositionTuple.Item2;

                if (isNextPositionInBounds)
                {
                    positions.Add(nextPosition);
                    index++;
                }
                else
                {
                    break;                    
                }
            }

            return index;
        }

        private Tuple<bool,Vector3> IsNextPositionInBounds(VolumeFlowAxis currentAxis, Vector3 previousObjectPosition, ChildVolumeItem previousObject, ChildVolumeItem currentObject)
        {
            Vector3 newPosition = previousObjectPosition;

            float offset = GetOffset(currentAxis, previousObject, currentObject);

            switch (PrimaryFlowAxis)
            {
                case VolumeFlowAxis.X:
                    newPosition += VolumeBounds.Right * offset;
                    break;
                case VolumeFlowAxis.NegativeX:
                    newPosition += VolumeBounds.Left * offset;
                    break;
                case VolumeFlowAxis.Y:
                    newPosition += VolumeBounds.Down * offset;
                    break;
                case VolumeFlowAxis.NegativeY:
                    newPosition += VolumeBounds.Up * offset;
                    break;
                case VolumeFlowAxis.Z:
                    newPosition += VolumeBounds.Forward * offset;
                    break;
                case VolumeFlowAxis.NegativeZ:
                    newPosition += VolumeBounds.Back * offset;
                    break;
            }

            // TO DO: Consider rotation 
            if (Volume.VolumeBounds.Contains(newPosition))
            {
                return Tuple.Create(true, newPosition);
            }

            newPosition = Vector3.zero;
            return Tuple.Create(false, newPosition);
        }

        private float GetOffset(VolumeFlowAxis currentAxis, ChildVolumeItem previousObject, ChildVolumeItem currentObject)
        {
            Transform currentObjectTransform = currentObject.Transform;
            Transform previousObjectTransform = previousObject.Transform;

            bool useBaseVolumeSizes = previousObject.Volume && currentObject.Volume;

            float offset = 0;

            // TO DO - INCLUDE MARGIN
            switch (currentAxis)
            {
                case VolumeFlowAxis.X:
                    float xDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.x + currentObject.Volume.VolumeSize.x) 
                        : (previousObjectTransform.lossyScale.x + currentObjectTransform.lossyScale.x);
                    xDistance *= 0.5f;
                    offset = xDistance + Spacing.x;
                    break;
                case VolumeFlowAxis.Y:
                    float yDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.y + currentObject.Volume.VolumeSize.y)
                        : (previousObjectTransform.lossyScale.y + currentObjectTransform.lossyScale.y);
                    yDistance *= 0.5f;
                    offset = yDistance + Spacing.y;
                    break;
                case VolumeFlowAxis.Z:
                    float zDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.z + currentObject.Volume.VolumeSize.z)
                        : (previousObjectTransform.lossyScale.z + currentObjectTransform.lossyScale.z);
                    zDistance *= 0.5f;
                    offset = zDistance + Spacing.z;
                    break;
            }

            return offset;
        }


        private Vector3 GetStartPosition()
        {
            if (StartingFlexPositionMode == StartingFlexPositionMode.CornerPoint)
            {
                return Volume.GetCornerPoint(StartCornerPoint);
            }
            else if (StartingFlexPositionMode == StartingFlexPositionMode.FacePoint)
            {
                return Volume.GetFacePoint(StartFacePoint);
            }
            else
            {
                Vector3 positionOffset = new Vector3(StartingXPositionOffset, StartingYPositionOffset, StartingZPositionOffset);
                return Volume.VolumeCenter + positionOffset;
            }
        }

        private void InitializePlacing()
        {
            Vector3 start = GetStartPosition();

            positions.Add(start);
            index++;

            int tertiaryAxisIndex = 0;

            while (index < Volume.ChildVolumeCount)
            {
                Place(PrimaryFlowAxis);

                Vector3 newStartingPinPosition = GetNextPinPosition(start, SecondaryFlowAxis);

                if (!Volume.VolumeBounds.Contains(newStartingPinPosition))
                {
                    Vector3 zStartingPos = GetNextPinPosition(positions[tertiaryAxisIndex], TertiaryFlowAxis);
                    tertiaryAxisIndex = index;

                    if (!Volume.VolumeBounds.Contains(zStartingPos))
                    {
                        break;
                    }
                    else
                    {
                        positions.Add(zStartingPos);

                        start = zStartingPos;
                    }
                }
                else
                {
                    positions.Add(newStartingPinPosition);

                    start = newStartingPinPosition;
                }

                index++;
            }

            index = 0;
        }

        private Vector3 GetNextPinPosition(Vector3 groupStartPosition, VolumeFlowAxis currentFlowAxis)
        {
            Vector3 newPinPosition = groupStartPosition;

            switch (currentFlowAxis)
            {
                case VolumeFlowAxis.X:
                    newPinPosition += VolumeBounds.Right * Spacing.x;
                    break;
                case VolumeFlowAxis.NegativeX:
                    newPinPosition += VolumeBounds.Left * Spacing.x;
                    break;
                case VolumeFlowAxis.Y:
                    newPinPosition += VolumeBounds.Down * Spacing.y;
                    break;
                case VolumeFlowAxis.NegativeY:
                    newPinPosition += VolumeBounds.Up * Spacing.y;
                    break;
                case VolumeFlowAxis.Z:
                    newPinPosition += VolumeBounds.Forward * Spacing.z;
                    break;
                case VolumeFlowAxis.NegativeZ:
                    newPinPosition += VolumeBounds.Back * Spacing.z;
                    break;
            }

            return newPinPosition;
        }

        public override void Update()
        {
            if (Application.isPlaying)
            {
                // Add events
                UpdateFlex();
            }
        }

        public void UpdateFlex()
        {
            positions.Clear();

            InitializePlacing();

            Volume.SetChildVolumePositions(positions.ToArray());
        }
    }
}
