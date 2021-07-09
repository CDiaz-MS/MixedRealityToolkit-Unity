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
        private CornerPoint startCornerPoint = CornerPoint.LeftTopForward;

        public CornerPoint StartCornerPoint
        {
            get => startCornerPoint;
            set => startCornerPoint = value;
        }

        [SerializeField]
        private VolumeAxis primaryFlowAxis = VolumeAxis.X;

        public VolumeAxis PrimaryFlowAxis
        {
            get => primaryFlowAxis;
            set => primaryFlowAxis = value;
        }

        [SerializeField]
        private VolumeAxis secondaryFlowAxis = VolumeAxis.Y;

        public VolumeAxis SecondaryFlowAxis
        {
            get => secondaryFlowAxis;
            set => secondaryFlowAxis = value;
        }

        [SerializeField]
        private VolumeAxis tertiaryFlowAxis = VolumeAxis.Z;

        public VolumeAxis TertiaryFlowAxis
        {
            get => tertiaryFlowAxis;
            set => tertiaryFlowAxis = value;
        }

        public List<Vector3> positions = new List<Vector3>();

        private int index = 0;

        private int Place(VolumeAxis currentAxis)
        {
            while (index < Volume.ChildVolumeCount)
            {
                // Calcualte next position, if it is good then add it to the positions list
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


        private Tuple<bool,Vector3> IsNextPositionInBounds(VolumeAxis currentAxis, Vector3 previousObjectPosition, ChildVolumeItem previousObject, ChildVolumeItem currentObject)
        {
            Vector3 newPosition = previousObjectPosition;

            if (PrimaryFlowAxis == VolumeAxis.X && SecondaryFlowAxis == VolumeAxis.Y)
            {
                float xIncrement = GetOffset(currentAxis, previousObject, currentObject);

                if (StartCornerPoint.ToString().Contains("Left"))
                {
                    newPosition += VolumeBounds.Right * xIncrement;
                }
                else if (StartCornerPoint.ToString().Contains("Right"))
                {
                    newPosition += VolumeBounds.Left * xIncrement;
                }
            }
            else if (PrimaryFlowAxis == VolumeAxis.Y && SecondaryFlowAxis == VolumeAxis.X)
            {
                float yIncrement = GetOffset(currentAxis, previousObject, currentObject);

                if (StartCornerPoint.ToString().Contains("Top"))
                {
                    newPosition += VolumeBounds.Down * yIncrement;
                }
                else if (StartCornerPoint.ToString().Contains("Bottom"))
                {
                    newPosition += VolumeBounds.Up * yIncrement;
                }
            }

            //switch (currentAxis)
            //{
            //    case VolumeAxis.X:
            //    case VolumeAxis.Y:
            //        float xIncrement = GetOffset(currentAxis, previousObject);
            //        newPosition += Vector3.right * xIncrement;
            //        break;

            //        //float yIncrement = GetOffset(currentAxis, previousObject);
            //        //newPosition -= Vector3.up * yIncrement;
            //        //break;
            //    case VolumeAxis.Z:
            //        float zIncrement = GetOffset(currentAxis, previousObject);
            //        newPosition += Vector3.forward * zIncrement;
            //        break;
            //}

            // TO DO: Consider rotation 
            if (Volume.VolumeBounds.Contains(newPosition))
            {
                return Tuple.Create(true, newPosition);
            }

            newPosition = Vector3.zero;
            return Tuple.Create(false, newPosition);
        }

        private float GetOffset(VolumeAxis currentAxis, ChildVolumeItem previousObject, ChildVolumeItem currentObject)
        {
            Transform currentObjectTransform = currentObject.Transform;
            Transform previousObjectTransform = previousObject.Transform;

            bool useBaseVolumeSizes = previousObject.Volume && currentObject.Volume;

            float offset = 0;

            switch (currentAxis)
            {
                case VolumeAxis.X:
                    float xDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.x + currentObject.Volume.VolumeSize.x)
                        : (previousObjectTransform.lossyScale.x + currentObjectTransform.lossyScale.x);
                    xDistance *= 0.5f;
                    offset = xDistance + Spacing.x;
                    break;
                case VolumeAxis.Y:
                    float yDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.y + currentObject.Volume.VolumeSize.y)
                        : (previousObjectTransform.lossyScale.y + currentObjectTransform.lossyScale.y);
                    yDistance *= 0.5f;
                    offset = yDistance + Spacing.y;
                    break;
                case VolumeAxis.Z:
                    float zDistance = useBaseVolumeSizes ? (previousObject.Volume.VolumeSize.z + currentObject.Volume.VolumeSize.z)
                        : (previousObjectTransform.lossyScale.z + currentObjectTransform.lossyScale.z);
                    zDistance *= 0.5f;
                    offset = zDistance + Spacing.z;
                    break;
            }

            return offset;
        }

        private void InitializePlacing()
        {
            Vector3 start = Volume.GetCornerPoint(StartCornerPoint);

            positions.Add(start);
            index++;

            while (index < Volume.ChildVolumeCount)
            {
                Place(PrimaryFlowAxis);

                if (PrimaryFlowAxis == VolumeAxis.X && SecondaryFlowAxis == VolumeAxis.Y)
                {
                    if (StartCornerPoint.ToString().Contains("Top"))
                    {
                        start += VolumeBounds.Down * Spacing.y;
                    }
                    else if (StartCornerPoint.ToString().Contains("Bottom"))
                    {
                        start += VolumeBounds.Up * Spacing.y;
                    }
                }

                if (PrimaryFlowAxis == VolumeAxis.Y && SecondaryFlowAxis == VolumeAxis.X)
                {
                    if (StartCornerPoint.ToString().Contains("Left"))
                    {
                        start += VolumeBounds.Right * Spacing.x;
                    }
                    else if (StartCornerPoint.ToString().Contains("Right"))
                    {
                        start += VolumeBounds.Left * Spacing.x;
                    }
                }

                // Try Y Axis increment
                //if (Volume.VolumeBounds.Contains(start))
                {
                    // New Row
                    positions.Add(start);

                    //Place(SecondaryFlowAxis);

                    index++;
                }

                //start += Vector3.forward * depthIncrement;

                //// Try Z Axis increment
                //if (Volume.VolumeBounds.Contains(start))
                //{
                //    positions.Add(start);
                //    index++;
                //}
            }

            index = 0;
        }

        public override void Update()
        {
            if (Application.isPlaying)
            {
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
