// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
#endif

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public class VolumeGrid : Volume
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
        private bool useCustomCellHeight = false;

        public bool UseCustomCellHeight
        {
            get => useCustomCellHeight;
            set => useCustomCellHeight = value;
        }

        [SerializeField]
        private float cellHeight = 0.1f;

        public float CellHeight
        {
            get => cellHeight;
            set => cellHeight = value;
        }

        [SerializeField]
        private bool useCustomCellWidth = false;

        public bool UseCustomCellWidth
        {
            get => useCustomCellWidth;
            set => useCustomCellWidth = value;
        }

        [SerializeField]
        private float cellWidth = 0.1f;

        public float CellWidth
        {
            get => cellWidth;
            set => cellWidth = value;
        }

        [SerializeField]
        private bool useCustomCellDepth = false;

        public bool UseCustomCellDepth
        {
            get => useCustomCellDepth;
            set => useCustomCellDepth = value;
        }


        [SerializeField]
        private float cellDepth = 0.1f;

        public float CellDepth
        {
            get => cellDepth;
            set => cellDepth = value;
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
        private bool placeDisabledTransforms = true;

        public bool PlaceDisabledTransforms
        {
            get => placeDisabledTransforms;
            set => placeDisabledTransforms = value;
        }


        [SerializeField]
        private bool allowCustomPositionSet = false;

        public bool AllowCustomPositionSet
        {
            get => allowCustomPositionSet;
            set => allowCustomPositionSet = value;
        }

        [SerializeField]
        private bool keepGridFilled = true;

        public bool KeepGridFilled
        {
            get => keepGridFilled;
            set => keepGridFilled = value;
        }

        public float BoundsWidthIncrement => UseCustomCellWidth ? CellWidth : VolumeBounds.Width / Cols;
        public float BoundsHeightIncrement => UseCustomCellHeight ? CellHeight : VolumeBounds.Height / Rows;
        public float BoundsDepthIncrement => UseCustomCellDepth ? CellDepth : VolumeBounds.Depth / Depth;

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

        private List<VolumeGridNode> grid => CreateGrid(); 

        public override void Update()
        {
            base.Update();

            SetObjectPositions();

            DrawGridCells();
        }


        private void DrawGridCells()
        {
            if (DrawGrid)
            {
                foreach (VolumeGridNode cell in grid)
                {
                    cell.CellBounds.DrawBounds(Color.blue);
                }
            }
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

            List<VolumeGridNode> bounds = new List<VolumeGridNode>();

            for (int z = 0; z < Depth; z++)
            {
                if (z == 0)
                {
                    incrementZPosition = initialZPosition;
                }

                for (int x = 0; x < Cols; x++)
                {
                    if (x == 0)
                    {
                        incrementXPosition = initialXPosition;
                    }

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

                        Vector3 coordinate = new Vector3(y + 1, x + 1, z + 1);

                        VolumeGridNode node = new VolumeGridNode()
                        {
                            CellBounds = newBounds,
                            Coordinates = coordinate,
                        };

                        if (!gridDictionary.ContainsKey(coordinate))
                        {
                            gridDictionary.Add(coordinate, node);
                        }
                        
                        bounds.Add(node);

                        incrementYPosition += (VolumeBounds.Down * BoundsHeightIncrement);
                    }

                    incrementXPosition += (VolumeBounds.Right * BoundsWidthIncrement);
                }

                incrementZPosition += (VolumeBounds.Back * BoundsDepthIncrement);
            }

            return bounds;
        }

        private void SetRowColDepthCount()
        {
            if (MatchColsToChildren)
            {
                Cols = PlaceDisabledTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchRowsToChildren)
            {
                Rows = PlaceDisabledTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }

            if (MatchDepthToChildren)
            {
                Depth = PlaceDisabledTransforms ? ChildVolumeItems.Count : GetChildTransforms(false).Count;
            }
        }

        public void SetObjectPositions()
        {
            SetRowColDepthCount();

            int primaryAxisIndex = AxisFlowIndex(PrimaryFlowAxis);
            int secondaryAxisIndex = AxisFlowIndex(SecondaryFlowAxis);
            int tertiaryAxisIndex = AxisFlowIndex(TertiaryFlowAxis);

            int m = 0;

            if (AreFlowAxesUnique())
            {
                for (int i = 0; i < tertiaryAxisIndex; i++)
                {
                    for (int j = 0; j < secondaryAxisIndex; j++)
                    {
                        for (int k = 0; k < primaryAxisIndex; k++)
                        {
                            if (m < ChildVolumeItems.Count)
                            {
                                Transform volumeItemTransform = ChildVolumeItems[m].Transform;
                                
                                if (!PlaceDisabledTransforms)
                                {
                                    if (!volumeItemTransform.gameObject.activeSelf)
                                    {
                                        while (!ChildVolumeItems[m].Transform.gameObject.activeSelf)
                                        {
                                            if (m >= ChildVolumeItems.Count - 1)
                                            {
                                                break;
                                            }

                                            m++;
                                        }

                                        volumeItemTransform = ChildVolumeItems[m].Transform;
                                    }
                                }

                                Vector3 coordinate = GetCellCoordinates(i, j, k);
                                VolumeGridNode cell = GetCell(grid, coordinate);
                                Vector3 newPosition = GetCellPosition(cell, AxisAlignment);

                                SetNodeProperties(cell, volumeItemTransform);
                                
                                if (volumeItemTransform.transform.position != newPosition)
                                {
                                    SetObjectPosition(volumeItemTransform, newPosition, true);
                                }

                                m++;
                            }
                        }
                    }
                }

                if (DisableObjectsWithoutGridPosition)
                {
                    DisableGridOverflowObjects();
                }
            }
        }

        private void DisableGridOverflowObjects()
        {
            int gridCapacity = (Rows * Cols * Depth);
            int numberOfChildren = ChildVolumeItems.Count;

            if (gridCapacity < numberOfChildren)
            {
                for (int i = gridCapacity; i < numberOfChildren; i++)
                {
                    ChildVolumeItems[i].Transform.gameObject.SetActive(false);
                }
            }
        }

        protected virtual void SetObjectPosition(Transform transform, Vector3 newPosition, bool includeSmoothing)
        {
            if (Application.isPlaying && includeSmoothing)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 3f);
            }
            else
            {
                // In edit mode, do not include smoothing 
                transform.position = newPosition;
            }  
        }

        private bool AreFlowAxesUnique()
        {
            char primaryAxisName = PrimaryFlowAxis.ToString().Last();
            char secondaryAxisName = SecondaryFlowAxis.ToString().Last();
            char tertiaryAxisName = TertiaryFlowAxis.ToString().Last();

            if (primaryAxisName == secondaryAxisName)
            {
                Debug.LogError($"The values for PrimaryFlowAxis and SecondaryFlowAxis are both on the {primaryAxisName} axis, ensure each flow axis property is on a unique axis.");
                return false;
            }
            else if (primaryAxisName == tertiaryAxisName)
            {
                Debug.LogError($"The values for PrimaryFlowAxis and TertiaryFlowAxis are both on the {primaryAxisName} axis, ensure each flow axis property is on a unique axis.");
                return false;
            }
            else if (secondaryAxisName == tertiaryAxisName)
            {
                Debug.LogError($"The values for SecondaryFlowAxis and TertiaryFlowAxis are both on the {secondaryAxisName} axis, ensure each flow axis property is on a unique axis.");
                return false;
            }

            return true;
        }

        private Vector3 GetCellCoordinates(int i, int j, int k)
        {
            return GetCellCoordinateValue(PrimaryFlowAxis, k) + GetCellCoordinateValue(SecondaryFlowAxis, j) + GetCellCoordinateValue(TertiaryFlowAxis, i);
        }

        private Vector3 GetCellCoordinateValue(VolumeFlowAxis axis, int index)
        {
            Vector3 coordinate = Vector3.zero;

            switch (axis)
            {
                case VolumeFlowAxis.X:
                    coordinate.y = index + 1;
                    break;
                case VolumeFlowAxis.NegativeX:
                    coordinate.y = Cols - index;
                    break;
                case VolumeFlowAxis.Y:
                    coordinate.x = index + 1;
                    break;
                case VolumeFlowAxis.NegativeY:
                    coordinate.x = Rows - index;
                    break;
                case VolumeFlowAxis.Z:
                    coordinate.z = index + 1;
                    break;
                case VolumeFlowAxis.NegativeZ:
                    coordinate.z = Depth - index;
                    break;
            }

            return coordinate;
        }

        private Vector3 GetCellPosition(VolumeGridNode node, AxisAlignment axisAlign)
        {
            if (AxisAlignment == AxisAlignment.Center)
            {
                return node.CellBounds.Center;
            }
            else
            {
                int index = (int)axisAlign;
                return node.CellBounds.GetFacePoint((FacePoint)index);
            }
        }

        public VolumeGridNode GetCell(List<VolumeGridNode> nodes, Vector3 coordinates)
        {
            VolumeGridNode node = nodes.Find((item) => item.Coordinates == coordinates);

            return node;
        }

        private int AxisFlowIndex(VolumeFlowAxis axis)
        {
            int index = 0;
            
            switch (axis)
            {
                case VolumeFlowAxis.X:
                case VolumeFlowAxis.NegativeX:
                    index = Cols;
                    break;
                case VolumeFlowAxis.Y:
                case VolumeFlowAxis.NegativeY:
                    index = Rows;
                    break;
                case VolumeFlowAxis.Z:
                case VolumeFlowAxis.NegativeZ:
                    index = Depth;
                    break;
            }

            return index;
        }

        protected virtual void SetNodeProperties(VolumeGridNode node, Transform transform)
        {
            if (node != null)
            {
                node.Name = transform.gameObject.name;
                node.CellBounds.HostTransform = transform;
                node.CellGameObject = transform.gameObject;

                gridDictionary[node.Coordinates] = node;
            }
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
