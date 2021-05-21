// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI.Layout;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class UIVolumeGrid : UIVolume
    {
        [SerializeField]
        [Min(1)]
        private int rows = 3;

        /// <summary>
        /// Row number in the grid.  Y Axis subsections are rows.
        /// 
        /// ======================
        /// Row 1
        /// ======================
        /// Row 2
        /// ======================
        /// Row 3
        /// ======================
        /// </summary>
        public int Rows
        {
            get => rows;
            set
            {
                if (value >= 1)
                {
                    rows = value;
                    OnRowCountChanged.Invoke();
                }
            }
        }

        [SerializeField]
        private float minWidth = 0;

        public float MinWidth
        {
            get => minWidth;
            set => minWidth = value;
        }

        [SerializeField]
        [Min(1)]
        private int cols = 3;

        /// <summary>
        /// Col number in the grid. X Axis divided sections are cols.
        /// 
        /// ======================
        /// Col 1  | Col 2 | Col 3
        /// ======================
        /// Col 1  | Col 2 | Col 3
        /// ======================
        /// </summary>
        public int Cols
        {
            get => cols;
            set
            {
                if (value >= 1)
                {
                    cols = value;
                    OnColCountChanged.Invoke();
                }
            }
        }

        [SerializeField]
        [Min(1)]
        private int depth = 3;

        public int Depth
        {
            get => depth;
            set
            {
                if (value >= 1)
                {
                    depth = value;
                    OnDepthCountChanged.Invoke();
                }
            }
        }

        [SerializeField]
        private bool matchRowsToChildren = false;

        public bool MatchRowsToChildren
        {
            get => matchRowsToChildren;
            set => matchRowsToChildren = value;
        }

        [SerializeField]
        private bool matchColsToChildren = false;

        public bool MatchColsToChildren
        {
            get => matchColsToChildren;
            set => matchColsToChildren = value;
        }

        [SerializeField]
        private bool matchDepthToChildren = false;

        public bool MatchDepthToChildren
        {
            get => matchDepthToChildren;
            set => matchDepthToChildren = value;
        }

        [SerializeField]
        private bool disableObjectsWithoutGridPosition;

        public bool DisableObjectsWithoutGridPosition
        {
            get => disableObjectsWithoutGridPosition;
            set => disableObjectsWithoutGridPosition = value;
        }

        [SerializeField]
        private bool drawGrid = false;

        public bool DrawGrid
        {
            get => drawGrid;
            set => drawGrid = value;
        }

        [SerializeField]
        private bool includeInactiveTransforms = true;

        public bool IncludeInactiveTransforms
        {
            get => includeInactiveTransforms;
            set => includeInactiveTransforms = value;
        }


        [SerializeField]
        private bool allowCustomPositionSet = false;

        public bool AllowCustomPositionSet
        {
            get => allowCustomPositionSet;
            set => allowCustomPositionSet = value;
        }

        public float BoundsWidthIncrement => VolumeBounds.Width / Cols;
        public float BoundsHeightIncrement => VolumeBounds.Height / Rows;
        public float BoundsDepthIncrement => VolumeBounds.Depth / Depth;

        public Vector3 BoundsSize => new Vector3(BoundsWidthIncrement, BoundsHeightIncrement, BoundsDepthIncrement);

        public UnityAction UpdateGrid;

        public UnityEvent OnRowCountChanged = new UnityEvent();
        public UnityEvent OnColCountChanged = new UnityEvent();
        public UnityEvent OnDepthCountChanged = new UnityEvent();

        private Dictionary<Vector3, VolumeGridNode> gridDictionary = new Dictionary<Vector3, VolumeGridNode>();

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


        [SerializeField]
        private AxisAlignment axisAlignment = AxisAlignment.Center;

        public AxisAlignment AxisAlignment
        {
            get => axisAlignment;
            set => axisAlignment = value;
        }

        private int currentCols;

        //private List<VolumeBounds> gridPositions; 


        protected override void OnEnable()
        {
            base.OnEnable();

            //UpdateGrid += SetObjectPositions;

            //OnVolumeSizeChanged.AddListener(UpdateGrid);
        }


        protected override void Start()
        {
            if (Application.isPlaying)
            {
                //OnVolumeSizeChanged.AddListener(() => SyncObjectPositionsToGridPositions());
            }
            
        }



        public override void Update()
        {
            base.Update();

            SetObjectPositions();
        }

        private List<VolumeGridNode> CreateGrid()
        {
            Vector3 startBoundsPos = VolumeBounds.GetCornerPoint(CornerPoint.LeftTopForward);

            Vector3 initialYPosition = VolumeBounds.GetCornerPoint(CornerPoint.LeftTopForward) + (VolumeBounds.Down * BoundsHeightIncrement * 0.5f);
            Vector3 incrementYPosition = initialYPosition;

            Vector3 initialXPosition = VolumeBounds.GetCornerPoint(CornerPoint.LeftTopForward) + (VolumeBounds.Right * BoundsWidthIncrement * 0.5f);
            Vector3 incrementXPosition = initialXPosition;

            Vector3 initialZPosition = VolumeBounds.GetCornerPoint(CornerPoint.LeftTopForward) + (VolumeBounds.Back * BoundsDepthIncrement * 0.5f);
            Vector3 incrementZPosition = initialZPosition;

            gridDictionary.Clear();

            List<VolumeGridNode> bounds = new List<VolumeGridNode>();

            for (int z = 0; z < Depth; z++)
            {
                if (z == 0)
                {
                    incrementZPosition = initialZPosition;
                }

                Debug.DrawRay(incrementZPosition, VolumeBounds.Left * BoundsDepthIncrement, Color.green);


                for (int x = 0; x < Cols; x++)
                {
                    if (x == 0)
                    {
                        incrementXPosition = initialXPosition;
                    }

                    Debug.DrawRay(incrementXPosition, VolumeBounds.Up * BoundsWidthIncrement, Color.magenta);

                    for (int y = 0; y < Rows; y++)
                    {
                        if (y == 0)
                        {
                            incrementYPosition = initialYPosition;
                        }


                        Vector3 boundsCenter = startBoundsPos;

                        boundsCenter += (VolumeBounds.Down * (incrementYPosition - startBoundsPos).magnitude);
                        boundsCenter += (VolumeBounds.Right * (incrementXPosition - startBoundsPos).magnitude);
                        boundsCenter += (VolumeBounds.Back * (incrementZPosition - startBoundsPos).magnitude);

                        VolumeBounds newBounds = new VolumeBounds(BoundsSize, boundsCenter, transform);

                        //if ( y == 1 && x == 1 && z == 1)
                        {
                            newBounds.DrawBounds(Color.blue);
                        }

                        Vector3 coordinate = new Vector3(y + 1, x + 1, z + 1);

                        VolumeGridNode node = new VolumeGridNode()
                        {
                            CellBounds = newBounds,
                            Coordinates = coordinate,
                        };

                        gridDictionary.Add(coordinate, node);

                        bounds.Add(node);

                        Debug.DrawRay(incrementYPosition, VolumeBounds.Left * BoundsWidthIncrement);

                        incrementYPosition += (VolumeBounds.Down * BoundsHeightIncrement);
                        
                    }

                    incrementXPosition += (VolumeBounds.Right * BoundsWidthIncrement);
                }

                incrementZPosition += (VolumeBounds.Back * BoundsDepthIncrement);
            }

            return bounds;
        }

        //private List<Vector3> CalculateGridPositions(VolumeFlowAxis volumeFlowAxis)
        //{
        //    Vector3 centerPosition;
        //    int cellNumber;
        //    float cellSizeIncrement;
        //    Vector3 directionIncrement;
        //    List<Vector3> positions = new List<Vector3>();

        //    switch (volumeFlowAxis)
        //    {
        //        case VolumeFlowAxis.X:
        //        case VolumeFlowAxis.NegativeX:
        //            cellSizeIncrement = BoundsWidthIncrement;
        //            directionIncrement = VolumeBounds.Right;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Left) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Cols;
        //            break;
        //        case VolumeFlowAxis.Y:
        //        case VolumeFlowAxis.NegativeY:
        //            cellSizeIncrement = BoundsHeightIncrement;
        //            directionIncrement = VolumeBounds.Down;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Top) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Rows;
        //            break;
        //        case VolumeFlowAxis.Z:
        //        case VolumeFlowAxis.NegativeZ:
        //            cellSizeIncrement = BoundsDepthIncrement;
        //            directionIncrement = VolumeBounds.Forward;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Forward) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Depth;
        //            break;
        //        default:
        //            cellSizeIncrement = 0;
        //            centerPosition = Vector3.zero;
        //            cellNumber = 0;
        //            directionIncrement = Vector3.zero;
        //            break;
        //    }


        //    for (int i = 0; i < cellNumber; i++)
        //    {
        //        Debug.DrawRay(centerPosition, (Vector3.up + Vector3.right) * 0.1f, Color.white);

        //        positions.Add(centerPosition);


        //        //if (primaryFlowAxis == VolumeFlowAxis.X && secondaryFlowAxis == VolumeFlowAxis.Y)
        //        //{
        //        //    Vector3 pos = centerPosition + (VolBounds.GetUpDirection() * BoundsWidthIncrement / 2);

        //        //    Debug.DrawRay(pos, VolBounds.GetUpDirection() * 0.1f, Color.blue);

        //        //}


        //        centerPosition += (directionIncrement * cellSizeIncrement);
        //    }



        //    foreach (var pos in positions)
        //    {
        //        Vector3 cross = Vector3.Cross(VolumeBounds.Right, VolumeBounds.Back);

               


        //        Debug.DrawRay(pos, cross * 0.5f, Color.magenta);
            
        //    }


        //    for (int i = 0; i < Rows; i++)
        //    {

        //    }

        //    Vector3 startColPos = VolumeBounds.GetCornerPoint(CornerPoint.LeftBottomBack);


        //    for (int i = 0; i < Cols; i++)
        //    {
        //        if (Cols > 1)
        //        {
        //            Debug.DrawRay(startColPos, VolumeBounds.Back* VolumeBounds.Depth);

        //            startColPos += Vector3.right * BoundsWidthIncrement;
        //        }
        //    }


        //    Vector3 startDepthPos = VolumeBounds.GetCornerPoint(CornerPoint.LeftBottomBack);

        //    for (int i = 0; i < Depth; i++)
        //    {
        //        if (Depth > 1)
        //        {
        //            Debug.DrawRay(startDepthPos, VolumeBounds.Right * VolumeBounds.Width);

        //            startDepthPos += Vector3.back * BoundsDepthIncrement;
        //        }
        //    }



        //    if (volumeFlowAxis.ToString().Contains("Negative"))
        //    {
        //        positions.Reverse();
        //    }

        //    return positions;
        //}


        //private void GenerateCells()
        //{
        //    List<VolumeBounds> newGridPositions = new List<VolumeBounds>();
        //    List<Vector3> newPositions = new List<Vector3>();

        //    Vector3 boundsSize = new Vector3(BoundsWidthIncrement, BoundsHeightIncrement, BoundsDepthIncrement);

        //    List<Vector3> primaryAxisPositions = CalculateGridPositions(primaryFlowAxis);
        //    List<Vector3> secondaryAxisPositions = CalculateGridPositions(secondaryFlowAxis);
        //    List<Vector3> tertiaryAxisPositions = CalculateGridPositions(tertiaryFlowAxis);

        //    for (int tertiaryIndex = 0; tertiaryIndex < tertiaryAxisPositions.Count; tertiaryIndex++)
        //    {
        //        for (int secondaryIndex = 0; secondaryIndex < secondaryAxisPositions.Count; secondaryIndex++)
        //        {
        //            for (int primaryIndex = 0; primaryIndex < primaryAxisPositions.Count; primaryIndex++)
        //            {

        //                Vector3 primaryAxisPosition = CalculatePoint(PrimaryFlowAxis, primaryIndex);
        //                Vector3 secondaryAxisPosition = CalculatePoint(SecondaryFlowAxis, secondaryIndex);
        //                Vector3 tertiaryAxisPosition = CalculatePoint(TertiaryFlowAxis, tertiaryIndex);


        //                Vector3 cellCenterPosition = primaryAxisPosition + secondaryAxisPosition + tertiaryAxisPosition;


        //                Vector3 direction = (cellCenterPosition - VolumeBounds.Center).normalized;

        //                float magnitude = (cellCenterPosition - VolumeBounds.Center).magnitude;

        //                Vector3 pos = VolumeBounds.Center + ( (direction * magnitude));

        //                newPositions.Add(cellCenterPosition);

        //            }
        //        }
        //    }


        //    int i = 1;
        //    foreach (Vector3 pos in newPositions)
        //    {
        //        Vector3 direction = (pos - VolumeBounds.Center).normalized;

        //        float magnitude = (pos - VolumeBounds.Center).magnitude;

        //        Vector3 position = VolumeBounds.Center + (transform.rotation * (direction * magnitude));

        //        if (i == 1)
        //        {
        //            //Debug.Log("vol center: " + VolumeBounds.Center.ToString("F4"));
        //            //Debug.Log("pos: " + pos.ToString("F4"));
        //            //Debug.Log("direction: " + direction.ToString("F4"));
        //            //Debug.Log("magnitude: " + magnitude.ToString("F4"));
        //            //Debug.Log("position: " + position.ToString("F4"));

        //            Debug.DrawLine(VolumeBounds.Center, position);

        //        }



        //        Debug.DrawRay(position, Vector3.down * 0.1f, Color.green);
        //        i++;
        //    }


        //}


        //private Vector3 CalculatePoint(VolumeFlowAxis flowAxis, int index)
        //{
        //    Vector3 cellCenterPosition = Vector3.zero;

        //    //Debug.Log(flowAxis.ToString() + ": " + xAxisPoints.Count);

        //    //Vector3 direction = Vector3.zero;
        //    float increment = 0;


        //    switch (flowAxis)
        //    {
        //        case VolumeFlowAxis.X:
        //            //direction = VolumeBounds.GetRightDirection();
        //            increment = BoundsWidthIncrement;
        //            cellCenterPosition.x = xAxisPoints[index].x;
        //            break;
        //        case VolumeFlowAxis.NegativeX:
        //            cellCenterPosition.x = xAxisPointsReverse[index].x;
        //            //direction = VolumeBounds.GetRightDirection();
        //            increment = BoundsWidthIncrement;
        //            break;
        //        case VolumeFlowAxis.Y:
        //            cellCenterPosition.y = yAxisPoints[index].y;
        //            //direction = VolumeBounds.GetDownDirection();
        //            increment = BoundsHeightIncrement;
        //            break;
        //        case VolumeFlowAxis.NegativeY:
        //            cellCenterPosition.y = yAxisPointsReverse[index].y;
        //            //direction = VolumeBounds.GetDownDirection();
        //            increment = BoundsHeightIncrement;
        //            break;
        //        case VolumeFlowAxis.Z:
        //            cellCenterPosition.z = zAxisPoints[index].z;
        //            //direction = VolumeBounds.GetForwardDirection();
        //            increment = BoundsDepthIncrement;
        //            break;
        //        case VolumeFlowAxis.NegativeZ:
        //            cellCenterPosition.z = zAxisPointsReverse[index].z;
        //            //direction = VolumeBounds.GetForwardDirection();
        //            increment = BoundsDepthIncrement;
        //            break;
        //    }

        //    //cellCenterPosition = cellCenterPosition + (direction * increment * 0.5f);

        //    return cellCenterPosition;

        //}

        private void SetRowColDepthCount()
        {
            if (MatchColsToChildren)
            {
                Rows = IncludeInactiveTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchRowsToChildren)
            {
                Cols = IncludeInactiveTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchDepthToChildren)
            {
                Depth = ChildVolumeItems.Count;
            }
        }


        private Vector3 GetAlignmentPosition(VolumeBounds bounds, AxisAlignment axisAlign)
        {
            Vector3[] pos = bounds.GetFacePositions();

            if (axisAlign != AxisAlignment.Center)
            {
                int index = (int)axisAlign;

                return pos[index];
            }
            
            return bounds.Center;
        }

        //private List<VolumeBounds> GenerateAxisPositions(VolumeFlowAxis volumeFlowAxis)
        //{
        //    Vector3 centerPosition;
        //    int cellNumber;
        //    float cellSizeIncrement;
        //    List<VolumeBounds> axisBounds = new List<VolumeBounds>();
        //    Vector3 directionIncrement;

        //    switch (volumeFlowAxis)
        //    {
        //        case VolumeFlowAxis.X:
        //        case VolumeFlowAxis.NegativeX:
        //            cellSizeIncrement = BoundsWidthIncrement;
        //            //directionIncrement = Vector3.right;
        //            directionIncrement = VolumeBounds.Right;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Left) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Rows;
        //            break;
        //        case VolumeFlowAxis.Y:
        //        case VolumeFlowAxis.NegativeY:
        //            cellSizeIncrement = BoundsHeightIncrement;
        //            //directionIncrement = Vector3.down;
        //            directionIncrement = VolumeBounds.Down;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Top) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Cols;
        //            break;
        //        case VolumeFlowAxis.Z:
        //        case VolumeFlowAxis.NegativeZ:
        //            cellSizeIncrement = BoundsDepthIncrement;
        //            //directionIncrement = Vector3.forward;
        //            directionIncrement = VolumeBounds.Forward;
        //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Forward) + (directionIncrement * cellSizeIncrement * 0.5f);
        //            cellNumber = Depth;
        //            break;
        //        default:
        //            cellSizeIncrement = 0;
        //            centerPosition = Vector3.zero;
        //            cellNumber = 0;
        //            directionIncrement = Vector3.zero;
        //            break;
        //    }

        //    for (int i = 0; i < cellNumber; i++)
        //    {
        //        //Vector3 center = centerPosition;

        //        float boundsSizeX = volumeFlowAxis == VolumeFlowAxis.X || volumeFlowAxis == VolumeFlowAxis.NegativeX ? cellSizeIncrement : VolumeSize.x;
        //        float boundsSizeY = volumeFlowAxis == VolumeFlowAxis.Y || volumeFlowAxis == VolumeFlowAxis.NegativeY ? cellSizeIncrement : VolumeSize.y;
        //        float boundsSizeZ = volumeFlowAxis == VolumeFlowAxis.Z || volumeFlowAxis == VolumeFlowAxis.NegativeZ ? cellSizeIncrement : VolumeSize.z;


        //        Vector3 boundsSize = new Vector3(boundsSizeX, boundsSizeY, boundsSizeZ);

        //        //Bounds newBounds = new Bounds(center, boundsSize);

        //        VolumeBounds newBounds = new VolumeBounds(boundsSize, centerPosition, transform);


        //        axisBounds.Add(newBounds);

        //        //Debug.DrawRay(centerPosition, Vector3.up * 0.5f);

        //        centerPosition += (cellSizeIncrement * directionIncrement);
        //    }


        //    if (volumeFlowAxis.ToString().Contains("Negative"))
        //    {
        //        axisBounds.Reverse();
        //    }

        //    return axisBounds;
        //}

        //private List<VolumeBounds> CreateGrid()
        //{
        //    List<VolumeBounds> newGridPositions = new List<VolumeBounds>();

        //    SetRowColDepthCount();

        //    Vector3 boundsSize = new Vector3(BoundsWidthIncrement, BoundsHeightIncrement, BoundsDepthIncrement);

        //    List<VolumeBounds> axis1 = GenerateAxisPositions(primaryFlowAxis);
        //    List<VolumeBounds> axis2 = GenerateAxisPositions(secondaryFlowAxis);
        //    List<VolumeBounds> axis3 = GenerateAxisPositions(tertiaryFlowAxis);

        //    int count = 0;

        //    for (int tertiaryIndex = 0; tertiaryIndex < axis3.Count; tertiaryIndex++)
        //    {
        //        for (int secondaryIndex = 0; secondaryIndex < axis2.Count; secondaryIndex++)
        //        {
        //            for (int primaryIndex = 0; primaryIndex < axis1.Count; primaryIndex++)
        //            {
        //                float xPos = 0;
        //                float yPos = 0;
        //                float zPos = 0;

        //                // X Positioning
        //                if (primaryFlowAxis == VolumeFlowAxis.X)
        //                {
        //                    xPos = rowBounds[primaryIndex].Center.x;
        //                }
        //                else if (primaryFlowAxis == VolumeFlowAxis.NegativeX)
        //                {
        //                    xPos = rowBoundsReverse[primaryIndex].Center.x;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.X)
        //                {
        //                    xPos = rowBounds[secondaryIndex].Center.x;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.NegativeX)
        //                {
        //                    xPos = rowBoundsReverse[secondaryIndex].Center.x;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.X)
        //                {
        //                    xPos = rowBounds[tertiaryIndex].Center.x;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeX)
        //                {
        //                    xPos = rowBoundsReverse[tertiaryIndex].Center.x;
        //                }

        //                // Y positioning

        //                if (primaryFlowAxis == VolumeFlowAxis.Y)
        //                {
        //                    yPos = colBounds[primaryIndex].Center.y;
        //                }
        //                else if (primaryFlowAxis == VolumeFlowAxis.NegativeY)
        //                {
        //                    yPos = colBoundsReverse[primaryIndex].Center.y;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.Y)
        //                {
        //                    yPos = colBounds[secondaryIndex].Center.y;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.NegativeY)
        //                {
        //                    yPos = colBoundsReverse[secondaryIndex].Center.y;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.Y)
        //                {
        //                    yPos = colBounds[tertiaryIndex].Center.y;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeY)
        //                {
        //                    yPos = colBoundsReverse[tertiaryIndex].Center.y;
        //                }


        //                if (primaryFlowAxis == VolumeFlowAxis.Z)
        //                {
        //                    zPos = depthBounds[primaryIndex].Center.z;
        //                }
        //                if (primaryFlowAxis == VolumeFlowAxis.NegativeZ)
        //                {
        //                    zPos = depthBoundsReverse[primaryIndex].Center.z;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.Z)
        //                {
        //                    zPos = depthBounds[secondaryIndex].Center.z;
        //                }
        //                else if (secondaryFlowAxis == VolumeFlowAxis.NegativeZ)
        //                {
        //                    zPos = depthBoundsReverse[secondaryIndex].Center.z;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.Z)
        //                {
        //                    zPos = depthBounds[tertiaryIndex].Center.z;
        //                }
        //                else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeZ)
        //                {
        //                    zPos = depthBoundsReverse[tertiaryIndex].Center.z;
        //                }

        //                Vector3 boundPos = new Vector3(xPos, yPos, zPos);

        //                Vector3 direction = (boundPos - VolumeBounds.Center).normalized;

        //                float distance = Vector3.Distance(boundPos, VolumeBounds.Center);

                        

        //                if (primaryIndex == 0)
        //                {
        //                    //Debug.Log(distance);
        //                }

        //                Vector3 positionRotation = VolumeBounds.Center + (transform.rotation * (direction * distance));


        //                //Debug.DrawLine(VolBounds.Center, positionRotation);



        //                VolumeBounds newBounds = new VolumeBounds(boundsSize, positionRotation, transform);

        //               // Debug.Log(boundsSize);


        //                newGridPositions.Add(newBounds);

        //                if (Application.isPlaying)
        //                {                           
        //                    Vector3 coordinate = DetermineCoordinateOrder(primaryIndex, secondaryIndex, tertiaryIndex);

        //                    if (!gridDictionary.ContainsKey(coordinate))
        //                    {
        //                        VolumeGridNode node = new VolumeGridNode()
        //                        {
        //                            Coordinates = coordinate,
        //                            CellBounds = newBounds,
        //                            Count = count
        //                        };

        //                        gridDictionary.Add(coordinate, node);
        //                    }

        //                    count++;

        //                }
        //            }
        //        }
        //    }

        //    //gridPositions = newGridPositions;

        //    return newGridPositions;
        //}

        private void SetObjectPositions()
        {
            int col = 0;

            SetRowColDepthCount();

            List<VolumeGridNode> bounds = CreateGrid();

            foreach (ChildVolumeItem item in ChildVolumeItems)
            {
                Transform volumeItemTransform = item.Transform;

                volumeItemTransform.position = bounds[col].CellBounds.Center;//bounds.Find((node) => node.Coordinates == coordinates).CellBounds.Center;//gridDictionary[coordinates].CellBounds.Center;

                col++;
            }

        }


        private void GetCoordinate(int currentRow, int currentCol, int currentDepth)
        {
            if (PrimaryFlowAxis == VolumeFlowAxis.Y
                && SecondaryFlowAxis == VolumeFlowAxis.X
                && TertiaryFlowAxis == VolumeFlowAxis.Z)
            {

            }

            //    Vector3 centerPosition;
            //    int cellNumber;
            //    float cellSizeIncrement;
            //    List<VolumeBounds> axisBounds = new List<VolumeBounds>();
            //    Vector3 directionIncrement;

            //    switch (volumeFlowAxis)
            //    {
            //        case VolumeFlowAxis.X:
            //        case VolumeFlowAxis.NegativeX:
            //            cellSizeIncrement = BoundsWidthIncrement;
            //            //directionIncrement = Vector3.right;
            //            directionIncrement = VolumeBounds.Right;
            //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Left) + (directionIncrement * cellSizeIncrement * 0.5f);
            //            cellNumber = Rows;
            //            break;
            //        case VolumeFlowAxis.Y:
            //        case VolumeFlowAxis.NegativeY:
            //            cellSizeIncrement = BoundsHeightIncrement;
            //            //directionIncrement = Vector3.down;
            //            directionIncrement = VolumeBounds.Down;
            //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Top) + (directionIncrement * cellSizeIncrement * 0.5f);
            //            cellNumber = Cols;
            //            break;
            //        case VolumeFlowAxis.Z:
            //        case VolumeFlowAxis.NegativeZ:
            //            cellSizeIncrement = BoundsDepthIncrement;
            //            //directionIncrement = Vector3.forward;
            //            directionIncrement = VolumeBounds.Forward;
            //            centerPosition = VolumeBounds.GetFacePoint(FacePoint.Forward) + (directionIncrement * cellSizeIncrement * 0.5f);
            //            cellNumber = Depth;
            //            break;
            //        default:
            //            cellSizeIncrement = 0;
            //            centerPosition = Vector3.zero;
            //            cellNumber = 0;
            //            directionIncrement = Vector3.zero;
            //            break;
            //    }
        }

        private Vector3 DetermineCoordinateOrder(int primaryIndex, int secondaryIndex, int tertiaryIndex)
        {

            if (PrimaryFlowAxis == VolumeFlowAxis.X 
                && SecondaryFlowAxis == VolumeFlowAxis.Y 
                && TertiaryFlowAxis == VolumeFlowAxis.Z)
            {
                return new Vector3( secondaryIndex + 1, primaryIndex + 1, tertiaryIndex + 1);
            }


            if (PrimaryFlowAxis == VolumeFlowAxis.Y
                && SecondaryFlowAxis == VolumeFlowAxis.X
                && TertiaryFlowAxis == VolumeFlowAxis.Z)
            {
                return new Vector3(primaryIndex + 1, secondaryIndex + 1, tertiaryIndex + 1);
            }


            if (PrimaryFlowAxis == VolumeFlowAxis.Z
                && SecondaryFlowAxis == VolumeFlowAxis.X
                && TertiaryFlowAxis == VolumeFlowAxis.Y)
            {
                return new Vector3(tertiaryIndex + 1, secondaryIndex + 1, primaryIndex + 1);
            }

            if (PrimaryFlowAxis == VolumeFlowAxis.NegativeZ
                && SecondaryFlowAxis == VolumeFlowAxis.X
                && TertiaryFlowAxis == VolumeFlowAxis.Y)
            {
                return new Vector3(tertiaryIndex + 1, secondaryIndex + 1, Depth - primaryIndex);
            }


            if (PrimaryFlowAxis == VolumeFlowAxis.NegativeX
                && SecondaryFlowAxis == VolumeFlowAxis.Y
                && TertiaryFlowAxis == VolumeFlowAxis.Z)
            {
                return new Vector3(secondaryIndex + 1, Cols - primaryIndex, tertiaryIndex + 1);
            }


            if (PrimaryFlowAxis == VolumeFlowAxis.NegativeY
                && SecondaryFlowAxis == VolumeFlowAxis.X
                && TertiaryFlowAxis == VolumeFlowAxis.Z)
            {
                return new Vector3(Rows - primaryIndex, secondaryIndex + 1, tertiaryIndex + 1);
            }

            return Vector3.zero;
        }

        //public void SyncObjectPositionsToGridPositions()
        //{
        //    int i = 0;

        //    List<VolumeBounds> gridPositions = CreateGrid();

        //    foreach (ChildVolumeItem item in ChildVolumeItems)
        //    {
        //        Transform volumeItemTransform = item.Transform;

        //        if (!volumeItemTransform.gameObject.activeSelf && !IncludeInactiveTransforms)
        //        {
        //            //
        //        }
        //        else
        //        {
        //            if (i < gridPositions.Count)
        //            {
        //                if (Application.isPlaying)
        //                {
        //                    VolumeGridNode node = GetObjectAtCount(i);

        //                    SetNodeProperties(node, volumeItemTransform);
        //                }

        //                if (DisableObjectsWithoutGridPosition)
        //                {
        //                    if (!volumeItemTransform.gameObject.activeSelf)
        //                    {
        //                        volumeItemTransform.gameObject.SetActive(true);
        //                    }
        //                }

        //                Vector3 position = GetAlignmentPosition(gridPositions[i], AxisAlignment);

        //                if (!Application.isPlaying)
        //                {
        //                    float distance = Vector3.Distance(position, VolumeBounds.Center);
        //                    //float magnitude = distance.magnitude;


        //                    Vector3 direction = (position - VolumeBounds.Center).normalized;

        //                    //Vector3 distance = Vector3.Distance(transform.position, position);
        //                    //float magnitude = distance.magnitude;

        //                    //Vector3 direction = distance / magnitude;

                            
        //                    //volumeItemTransform.position = VolBounds.Center + (transform.rotation * ( direction * distance)); //+ (transform.rotation * volumeItemTransform.transform.localScale);
        //                    volumeItemTransform.position = position;//VolBounds.Center + (transform.rotation * ( direction * magnitude)); //+ (transform.rotation * volumeItemTransform.transform.localScale);
        //                }
        //                else
        //                {
        //                    if (!AllowCustomPositionSet)
        //                    {
        //                        volumeItemTransform.position = Vector3.Lerp(volumeItemTransform.position, position, 3 * Time.deltaTime);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (DisableObjectsWithoutGridPosition)
        //                {
        //                    if (volumeItemTransform.gameObject.activeSelf)
        //                    {
        //                        volumeItemTransform.gameObject.SetActive(false);
        //                    }
        //                }
        //            }

        //            i++;
        //        }

        //    }
        //}


        private void SetNodeProperties(VolumeGridNode node, Transform transform)
        {
            if (node != null)
            {
                node.Name = transform.gameObject.name;
                node.CellBounds.HostTransform = transform;

                if (!AllowCustomPositionSet)
                {
                    node.CellGameObjectTransform = transform;
                    node.CellGameObject = transform.gameObject;
                }
            }
        }


        private void MaintainRowWidth()
        {
            // Get the current amount of space the cols take up

            // if the increment decreases, then decrease the col count

            // Set minimum col width

            //if (BoundsWidthIncrement <= MinWidth)
            //{
            //    MatchRowsToChildren = false;
            //    //gridStack.Push();
            //    Rows--;
            //}

            //if (BoundsWidthIncrement >= MinWidth)
            //{
            //    MatchRowsToChildren = true;
            //}
        }

        //public void SetCustomPositionInternal(Vector3 previousCell, Vector3 newCell)
        //{
        //    AllowCustomPositionSet = true;

        //    SyncObjectPositionsToGridPositions();

        //    GameObject obj = GetObjectAtCoordinates(previousCell);

        //    gridDictionary[previousCell].CellGameObject = null;
        //    gridDictionary[newCell].CellGameObject = obj;

        //    gridDictionary[newCell].CellGameObject.transform.position = gridDictionary[newCell].CellBounds.Center;
                                                                           
        //}

        //public void SetCustomPositionExternal(GameObject go, Vector3 newCell)
        //{
        //    AllowCustomPositionSet = true;

        //    SyncObjectPositionsToGridPositions();

        //    gridDictionary[newCell].CellGameObject = go;

        //    gridDictionary[newCell].CellGameObject.transform.position = gridDictionary[newCell].CellBounds.Center;

        //}

        public GameObject GetObjectAtCoordinates(Vector3 coordinates)
        {
            VolumeGridNode cell = gridDictionary[coordinates];

            if (!cell.IsCellPopulated)
            {
                Debug.Log("Cell does not have GameObject");
                return null;
            }

            return cell.CellGameObject;
        }

        public Vector3 GetCoordinatesForGameObject(GameObject go)
        {
            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Value.CellGameObject == go)
                {
                    return item.Key;
                }
            }

            return Vector3.zero;
        }


        public VolumeGridNode GetObjectAtCount(int count)
        {
            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Value.Count == count)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public void PrintAvailableCoordinates()
        {
            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Value.IsCellPopulated)
                {
                    Debug.Log(item.Value.Coordinates + item.Value.CellGameObject.name);
                }
                else
                {
                    Debug.Log(item.Value.Coordinates + ": Empty");
                }
            }
        }
    }
}
