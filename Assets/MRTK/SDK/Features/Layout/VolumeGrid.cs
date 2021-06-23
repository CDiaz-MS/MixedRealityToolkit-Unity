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
    public class VolumeGrid : BaseCustomVolume
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
        [Tooltip("Update the grid as properties are changed. If true, the grid will update " +
            " with each change in the inspector during edit mode and during play mode. " +
            " If false, the grid will not update as properties are changed.")]
        private bool updateGrid = true;

        /// <summary>
        /// Update the grid as properties are changed. If true, the grid will update
        /// with each change in the inspector during edit mode and during play mode.
        /// If false, the grid will not update as properties are changed.
        /// </summary>
        public bool UpdateGrid
        {
            get => updateGrid;
            set => updateGrid = value;
        }

        public float BoundsWidthIncrement => UseCustomCellWidth ? CellWidth : VolumeBounds.Width / Cols;
        public float BoundsHeightIncrement => UseCustomCellHeight ? CellHeight : VolumeBounds.Height / Rows;
        public float BoundsDepthIncrement => UseCustomCellDepth ? CellDepth : VolumeBounds.Depth / Depth;

        public Vector3 BoundsSize => new Vector3(BoundsWidthIncrement, BoundsHeightIncrement, BoundsDepthIncrement);

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

        public List<VolumeGridNode> grid = new List<VolumeGridNode>();//> CreateGrid();

        [SerializeField]
        private UnityEvent onRowCountChanged = new UnityEvent();

        public UnityEvent OnRowCountChanged
        {
            get => onRowCountChanged;
        }

        [SerializeField]
        private UnityEvent onColCountChanged = new UnityEvent();

        public UnityEvent OnColCountChanged
        {
            get => onColCountChanged;
        }

        [SerializeField]
        private UnityEvent onDepthCountChanged = new UnityEvent();

        public UnityEvent OnDepthCountChanged
        {
            get => onDepthCountChanged;
        }

        [SerializeField]
        private float smoothingSpeed = 3f;

        public float SmoothingSpeed
        {
            get => smoothingSpeed;
            set => smoothingSpeed = value;
        }

        [SerializeField]
        private bool useSmoothing = true;

        public bool UseSmoothing
        {
            get => useSmoothing;
            set => useSmoothing = value;
        }

        private Vector3 startingGridCalculationPosition;

        public Vector3 StartingGridCalculationPosition
        {
            get => startingGridCalculationPosition;
            set => startingGridCalculationPosition = value;
        }

        public override void Update()
        {
            base.Update();

            if (UpdateGrid && Application.isPlaying)
            {
                CreateGridSetPositions();
            }

            DrawGridCells();
        }

        public virtual void CreateGridSetPositions()
        {
            StartingGridCalculationPosition = VolumeBounds.GetCornerPoint(CornerPoint.LeftTopForward);

            CreateGrid();
            SetObjectPositions();
        }

        public void DrawGridCells()
        {
            if (DrawGrid)
            {
                foreach (VolumeGridNode cell in grid)
                {
                    cell.CellBounds.DrawBounds(Color.blue);
                }
            }
        }

        protected virtual List<VolumeGridNode> CreateGrid()
        {
            SetRowColDepthCount();

            gridDictionary.Clear();

            Vector3 startBoundsPos = StartingGridCalculationPosition;

            Vector3 initialYPosition = StartingGridCalculationPosition + (VolumeBounds.Down * BoundsHeightIncrement * 0.5f);
            Vector3 incrementYPosition = initialYPosition;

            Vector3 initialXPosition = StartingGridCalculationPosition + (VolumeBounds.Right * BoundsWidthIncrement * 0.5f);
            Vector3 incrementXPosition = initialXPosition;

            Vector3 initialZPosition = StartingGridCalculationPosition + (VolumeBounds.Back * BoundsDepthIncrement * 0.5f);
            Vector3 incrementZPosition = initialZPosition;

            List<VolumeGridNode> nodes = new List<VolumeGridNode>();

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

                        gridDictionary.Add(coordinate, node);
                       
                        nodes.Add(node);

                        incrementYPosition += (VolumeBounds.Down * BoundsHeightIncrement);
                    }

                    incrementXPosition += (VolumeBounds.Right * BoundsWidthIncrement);
                }

                incrementZPosition += (VolumeBounds.Back * BoundsDepthIncrement);
            }

            grid = nodes;

            return nodes;
        }

        private void SetRowColDepthCount()
        {
            if (MatchColsToChildren)
            {
                Cols = PlaceDisabledTransforms ? ChildVolumeItems.Count : Volume.GetActiveChildCount();
            }

            if (MatchRowsToChildren)
            { 
                Rows = PlaceDisabledTransforms ? ChildVolumeItems.Count : Volume.GetActiveChildCount();
            }

            if (MatchDepthToChildren)
            {
                Depth = PlaceDisabledTransforms ? ChildVolumeItems.Count : Volume.GetActiveChildCount();
            }
        }

        public Vector3[] CalculateObjectFlowPositions()
        {
            int gridSize = Rows * Cols * Depth;
            Vector3[] gridPositions = new Vector3[gridSize];

            int primaryAxisCount = AxisFlowIndex(PrimaryFlowAxis);
            int secondaryAxisCount = AxisFlowIndex(SecondaryFlowAxis);
            int tertiaryAxisCount = AxisFlowIndex(TertiaryFlowAxis);

            int gridPositionsIndex = 0;

            if (AreFlowAxesUnique())
            {
                for (int tertiaryIndex = 0; tertiaryIndex < tertiaryAxisCount; tertiaryIndex++)
                {
                    for (int secondaryIndex = 0; secondaryIndex < secondaryAxisCount; secondaryIndex++)
                    {
                        for (int primaryIndex = 0; primaryIndex < primaryAxisCount; primaryIndex++)
                        {
                            Vector3 coordinate = GetCellCoordinates(tertiaryIndex, secondaryIndex, primaryIndex);
                            VolumeGridNode cell = GetCell(grid, coordinate);
                            Vector3 newPosition = GetCellPosition(cell, AxisAlignment);

                            gridPositions[gridPositionsIndex] = newPosition;

                            gridPositionsIndex++;
                        }
                    }
                }
            }

            return gridPositions;
        }

        protected void SetObjectPositions()
        {
            Vector3[] positions = CalculateObjectFlowPositions();

            if (PlaceDisabledTransforms && !DisableObjectsWithoutGridPosition)
            {
                int i = 0;

                foreach (var item in ChildVolumeItems)
                {
                    if (item.Transform)
                    {
                        VolumeGridNode cell = FindGridCellFromCenterPosition(positions[i]);

                        SetNodeProperties(cell, item.Transform);

                        if (Application.isPlaying && UseSmoothing)
                        {
                            item.Transform.position = Vector3.Lerp(item.Transform.position, positions[i], Time.deltaTime * SmoothingSpeed);
                        }
                        else
                        {
                            if (i < ChildVolumeItems.Count && i < positions.Length)
                            {
                                item.Transform.position = positions[i];
                            }
                        }

                        i++;
                    }
                }
            }

            if (!PlaceDisabledTransforms)
            {
                int i = 0;

                foreach (var item in ChildVolumeItems)
                {
                    if (item.Transform)
                    {
                        if (item.Transform.gameObject.activeSelf)
                        {
                            VolumeGridNode cell = FindGridCellFromCenterPosition(positions[i]);

                            SetNodeProperties(cell, item.Transform);

                            if (Application.isPlaying && UseSmoothing)
                            {
                                item.Transform.position = Vector3.Lerp(item.Transform.position, positions[i], Time.deltaTime * SmoothingSpeed);
                            }
                            else
                            {
                                if (i < ChildVolumeItems.Count && i < positions.Length)
                                {
                                    item.Transform.position = positions[i];
                                }
                            }

                            i++;
                        }
                    }
                }
            }

            if (DisableObjectsWithoutGridPosition)
            {
                DisableGridOverflowObjects();
            }
        }

        private VolumeGridNode FindGridCellFromCenterPosition(Vector3 position)
        {
            VolumeGridNode node = null;

            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Value.CellBounds.Center == position)
                {
                    return item.Value;
                }
            }

            return node;
        }

        private void DisableGridOverflowObjects()
        {
            int gridCapacity = (Rows * Cols * Depth);
            int numberOfChildren = ChildVolumeItems.Count;

            if (gridCapacity < numberOfChildren)
            {
                for (int i = gridCapacity; i < numberOfChildren; i++)
                {
                    ChildVolumeItems[i].Transform?.gameObject.SetActive(false);
                }
            }
        }

        private bool AreFlowAxesUnique()
        {
            char primaryAxisName = PrimaryFlowAxis.ToString().Last();
            char secondaryAxisName = SecondaryFlowAxis.ToString().Last();
            char tertiaryAxisName = TertiaryFlowAxis.ToString().Last();

            if (primaryAxisName == secondaryAxisName)
            {
                Debug.LogWarning($"The values for PrimaryFlowAxis and SecondaryFlowAxis are both on the {primaryAxisName} axis, ensure each flow axis property is on a unique axis.");
                return false;
            }
            else if (primaryAxisName == tertiaryAxisName)
            {
                Debug.LogWarning($"The values for PrimaryFlowAxis and TertiaryFlowAxis are both on the {primaryAxisName} axis, ensure each flow axis property is on a unique axis.");
                return false;
            }
            else if (secondaryAxisName == tertiaryAxisName)
            {
                Debug.LogWarning($"The values for SecondaryFlowAxis and TertiaryFlowAxis are both on the {secondaryAxisName} axis, ensure each flow axis property is on a unique axis.");
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
            if (node != null)
            {
                int index = (int)axisAlign;

                return AxisAlignment == AxisAlignment.Center ? node.CellBounds.Center : node.CellBounds.GetFacePoint((FacePoint)index);
            }

            return Vector3.zero;
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
            if (node != null && transform != null)
            {
                node.Name = transform.gameObject.name;
                node.CellGameObject = transform.gameObject;
                node.CellGameObjectTransform = transform;

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

        public List<VolumeGridNode> GetGameObjectsAtRow(int row)
        {
            List<VolumeGridNode> nodes = new List<VolumeGridNode>();

            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Key.x == row)
                {
                    nodes.Add(item.Value);
                }

            }

            return nodes;
        }

        public List<VolumeGridNode> GetGameObjectsAtCol(int col)
        {
            List<VolumeGridNode> nodes = new List<VolumeGridNode>();

            foreach (KeyValuePair<Vector3, VolumeGridNode> item in gridDictionary)
            {
                if (item.Key.x == col)
                {
                    nodes.Add(item.Value);
                }
            }

            return nodes;
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
