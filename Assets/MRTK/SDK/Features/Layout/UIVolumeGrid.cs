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
        private bool allowCustomPositionSet = true;

        public bool AllowCustomPositionSet
        {
            get => allowCustomPositionSet;
            set => allowCustomPositionSet = value;
        }

        private List<Bounds> rowBounds => GenerateAxisPositions(VolumeFlowAxis.X);
        private List<Bounds> colBounds => GenerateAxisPositions(VolumeFlowAxis.Y);
        private List<Bounds> depthBounds => GenerateAxisPositions(VolumeFlowAxis.Z);

        private List<Bounds> rowBoundsReverse => GenerateAxisPositions(VolumeFlowAxis.NegativeX);
        private List<Bounds> colBoundsReverse => GenerateAxisPositions(VolumeFlowAxis.NegativeY);
        private List<Bounds> depthBoundsReverse => GenerateAxisPositions(VolumeFlowAxis.NegativeZ);

        public float BoundsWidthIncrement => VolumeSize.x / Rows;
        public float BoundsHeightIncrement => VolumeSize.y / Cols;
        public float BoundsDepthIncrement => VolumeSize.z / Depth;

        public UnityAction UpdateGrid;

        private List<VolumeGridNode> volumeGridNodes = new List<VolumeGridNode>();

        private Stack<GameObject> gridStack = new Stack<GameObject>();

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


        protected override void OnEnable()
        {
            base.OnEnable();

            //UpdateGrid += SetObjectPositions;

            //OnVolumeSizeChanged.AddListener(UpdateGrid);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (DrawGrid)
            {
                List<Bounds> grid = CreateGrid();

                for (int i = 0; i < grid.Count; i++)
                {
                    Bounds bounds = grid[i];

                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(bounds.center, 0.01f);

                    Gizmos.color = Color.yellow;
                }
            }
        }

        private void SetRowColDepthCount()
        {
            if (MatchRowsToChildren)
            {
                Rows = IncludeInactiveTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchColsToChildren)
            {
                Cols = IncludeInactiveTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchDepthToChildren)
            {
                Depth = ChildVolumeItems.Count;
            }
        }


        private Vector3 GetAlignmentPosition(Bounds bounds, AxisAlignment axisAlign)
        {
            Vector3[] pos = new Vector3[6];
            bounds.GetFacePositions(ref pos);

            if (axisAlign != AxisAlignment.Center)
            {
                int index = (int)axisAlign;

                return pos[index];
            }
            
            return bounds.center;
        }

        private List<Bounds> GenerateAxisPositions(VolumeFlowAxis volumeFlowAxis)
        {
            Vector3 centerPosition;
            int cellNumber;
            float cellSizeIncrement;
            List<Bounds> axisBounds = new List<Bounds>();
            Vector3 directionIncrement;

            switch (volumeFlowAxis)
            {
                case VolumeFlowAxis.X:
                case VolumeFlowAxis.NegativeX:
                    cellSizeIncrement = BoundsWidthIncrement;
                    directionIncrement = Vector3.right;
                    centerPosition = GetFacePoint(FacePoint.Left) + (directionIncrement * cellSizeIncrement * 0.5f);
                    cellNumber = Rows;
                    break;
                case VolumeFlowAxis.Y:
                case VolumeFlowAxis.NegativeY:
                    cellSizeIncrement = BoundsHeightIncrement;
                    directionIncrement = Vector3.down;
                    centerPosition = GetFacePoint(FacePoint.Top) + (directionIncrement * cellSizeIncrement * 0.5f);
                    cellNumber = Cols;
                    break;
                case VolumeFlowAxis.Z:
                case VolumeFlowAxis.NegativeZ:
                    cellSizeIncrement = BoundsDepthIncrement;
                    directionIncrement = Vector3.forward;
                    centerPosition = GetFacePoint(FacePoint.Forward) + (directionIncrement * cellSizeIncrement * 0.5f);
                    cellNumber = Depth;
                    break;
                default:
                    cellSizeIncrement = 0;
                    centerPosition = Vector3.zero;
                    cellNumber = 0;
                    directionIncrement = Vector3.zero;
                    break;
            }

            for (int i = 0; i < cellNumber; i++)
            {
                Vector3 center = centerPosition;

                float boundsSizeX = volumeFlowAxis == VolumeFlowAxis.X || volumeFlowAxis == VolumeFlowAxis.NegativeX ? cellSizeIncrement : VolumeSize.x;
                float boundsSizeY = volumeFlowAxis == VolumeFlowAxis.Y || volumeFlowAxis == VolumeFlowAxis.NegativeY ? cellSizeIncrement : VolumeSize.y;
                float boundsSizeZ = volumeFlowAxis == VolumeFlowAxis.Z || volumeFlowAxis == VolumeFlowAxis.NegativeZ ? cellSizeIncrement : VolumeSize.z;

                Vector3 boundsSize = new Vector3(boundsSizeX, boundsSizeY, boundsSizeZ);

                Bounds newBounds = new Bounds(center, boundsSize);

                axisBounds.Add(newBounds);

                centerPosition += (cellSizeIncrement * directionIncrement);
            }


            if (volumeFlowAxis.ToString().Contains("Negative"))
            {
                axisBounds.Reverse();
            }

            return axisBounds;
        }

        public override void Update()
        {
            base.Update();

            SetObjectPositions();

            MaintainRowWidth();

            currentCols = Cols;
        }

        private List<Bounds> CreateGrid()
        {
            List<Bounds> gridPositions = new List<Bounds>();

            SetRowColDepthCount();

            Vector3 boundsSize = new Vector3(BoundsWidthIncrement, BoundsHeightIncrement, BoundsDepthIncrement);

            List<Bounds> axis1 = GenerateAxisPositions(primaryFlowAxis);
            List<Bounds> axis2 = GenerateAxisPositions(secondaryFlowAxis);
            List<Bounds> axis3 = GenerateAxisPositions(tertiaryFlowAxis);

            int count = 0;

            for (int i = 0; i < axis3.Count; i++)
            {
                for (int j = 0; j < axis2.Count; j++)
                {
                    for (int k = 0; k < axis1.Count; k++)
                    {
                        float xPos = 0;
                        float yPos = 0;
                        float zPos = 0;

                        // X Positioning
                        if (primaryFlowAxis == VolumeFlowAxis.X)
                        {
                            xPos = rowBounds[k].center.x;
                        }
                        else if (primaryFlowAxis == VolumeFlowAxis.NegativeX)
                        {
                            xPos = rowBoundsReverse[k].center.x;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.X)
                        {
                            xPos = rowBounds[j].center.x;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.NegativeX)
                        {
                            xPos = rowBoundsReverse[j].center.x;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.X)
                        {
                            xPos = rowBounds[i].center.x;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeX)
                        {
                            xPos = rowBoundsReverse[i].center.x;
                        }

                        // Y positioning

                        if (primaryFlowAxis == VolumeFlowAxis.Y)
                        {
                            yPos = colBounds[k].center.y;
                        }
                        else if (primaryFlowAxis == VolumeFlowAxis.NegativeY)
                        {
                            yPos = colBoundsReverse[k].center.y;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.Y)
                        {
                            yPos = colBounds[j].center.y;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.NegativeY)
                        {
                            yPos = colBoundsReverse[j].center.y;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.Y)
                        {
                            yPos = colBounds[i].center.y;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeY)
                        {
                            yPos = colBoundsReverse[i].center.y;
                        }


                        if (primaryFlowAxis == VolumeFlowAxis.Z)
                        {
                            zPos = depthBounds[k].center.z;
                        }
                        if (primaryFlowAxis == VolumeFlowAxis.NegativeZ)
                        {
                            zPos = depthBoundsReverse[k].center.z;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.Z)
                        {
                            zPos = depthBounds[j].center.z;
                        }
                        else if (secondaryFlowAxis == VolumeFlowAxis.NegativeZ)
                        {
                            zPos = depthBoundsReverse[j].center.z;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.Z)
                        {
                            zPos = depthBounds[i].center.z;
                        }
                        else if (tertiaryFlowAxis == VolumeFlowAxis.NegativeZ)
                        {
                            zPos = depthBoundsReverse[i].center.z;
                        }

                        Vector3 boundPos = new Vector3(xPos, yPos, zPos);

                        //boundPos = transform.TransformPoint(boundPos);

                        Bounds newBounds = new Bounds(boundPos, boundsSize);

                        gridPositions.Add(newBounds);

                        if (Application.isPlaying)
                        {
                            Vector3 coordinate = new Vector3(k+1, j+1, i+1);

                            if (!gridDictionary.ContainsKey(coordinate))
                            {
                                VolumeGridNode node = new VolumeGridNode()
                                {
                                    Coordinates = coordinate,
                                    CellBounds = newBounds,
                                    Count = count
                                };

                                gridDictionary.Add(coordinate, node);
                            }

                            count++;

                        }
                    }
                }
            }

            return gridPositions;
        }


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


        public VolumeGridNode GetObjectAtCount(int count)
        {
            foreach(KeyValuePair<Vector3,VolumeGridNode> item in gridDictionary)
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
                Debug.Log(item.Value.Coordinates);
            }
        }

        private void SetObjectPositions()
        {
            List<Bounds> gridPositions = CreateGrid();

            int i = 0;
            foreach (ChildVolumeItem item in ChildVolumeItems)
            {
                Transform volumeItemTransform = item.Transform;

                if (!volumeItemTransform.gameObject.activeSelf && !IncludeInactiveTransforms)
                {
                    //
                }
                else
                {
                    if (i < gridPositions.Count)
                    {
                        if (Application.isPlaying)
                        {
                            VolumeGridNode node = GetObjectAtCount(i);
                            
                            if (node != null)
                            {
                                node.Name = volumeItemTransform.gameObject.name;
                                node.CellGameObjectTransform = volumeItemTransform;
                                node.CellGameObject = volumeItemTransform.gameObject;
                            }
                        }

                        if (DisableObjectsWithoutGridPosition)
                        {
                            if (!volumeItemTransform.gameObject.activeSelf)
                            {
                                volumeItemTransform.gameObject.SetActive(true);
                            }
                        }

                        Vector3 position = GetAlignmentPosition(gridPositions[i], AxisAlignment);

                        if (!Application.isPlaying)
                        {
                            volumeItemTransform.position = position;

                        }
                        else
                        {
                            volumeItemTransform.position = Vector3.Lerp(volumeItemTransform.position, position, 3 * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if (DisableObjectsWithoutGridPosition)
                        {
                            if (volumeItemTransform.gameObject.activeSelf)
                            {
                                volumeItemTransform.gameObject.SetActive(false);
                            }
                        }
                    }

                    i++;
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
            //    gridStack.Push();
            //    Rows--;
            //}

            //if (BoundsWidthIncrement >= MinWidth)
            //{
            //    MatchRowsToChildren = true;
            //}
        }
    }
}
