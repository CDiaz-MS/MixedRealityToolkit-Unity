// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Tap to place is a far interaction component used to place obejcts relative to a surface.
    /// </summary>
    public class TapToPlace : Solver, IMixedRealityPointerHandler
    {
        [Experimental]
        [SerializeField]
        [Tooltip("The game object that will be placed if it is selected")]
        private GameObject gameObjectToPlace;

        /// <summary>
        /// The game object that will be placed if it is selected
        /// </summary>
        public GameObject GameObjectToPlace
        {
            get => gameObjectToPlace;
            set
            {
                if (value != null && gameObjectToPlace != value)
                {
                    gameObjectToPlace = value;
                }
            }
        }

        /// <summary>
        /// Check if a collider is present on the GameObjectToPlace
        /// </summary>
        public bool ColliderPresent
        {
            get
            {
                if (GameObjectToPlace.GetComponent<Collider>() == null)
                {
                    return false;
                }

                return true;
            }
        }

        [SerializeField]
        [Tooltip("The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler." +
            "The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.")]
        private float defaultPlacementDistance = 1.5f;

        /// <summary>
        /// The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler.
        /// The GameObjectToPlace will be placed at the default placement distance if a surface is not hit by the raycast.
        /// </summary>
        public float DefaultPlacementDistance
        {
            get => defaultPlacementDistance;
            set
            {
                if (defaultPlacementDistance != value)
                {
                    defaultPlacementDistance = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("Max distance to place an object if there is a raycast hit on a surface")]
        private float maxRaycastDistance = 20.0f;

        /// <summary>
        /// The max distance to place an object if there is a raycast hit on a surface
        /// </summary>
        public float MaxRaycastDistance
        {
            get => maxRaycastDistance;
            set => maxRaycastDistance = value;
        }

        [SerializeField]
        [Tooltip("Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component.
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get => magneticSurfaces;
            set => magneticSurfaces = value;
        }

        /// <summary>
        /// Is the gameObjectToPlace currently in a state where it is being placed? This state is activated when you select
        /// the GameObjectToPlace.
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        /// <summary>
        /// If true, the raycast did hit a surface
        /// </summary>
        public bool DidHit { get; protected set; }

        [SerializeField]
        [Tooltip("The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface")]
        private float surfaceNormalOffset = 0.0f;

        /// <summary>
        /// The distance between the center of the gameobject to place and the z extents!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        public float SurfaceNormalOffset
        {
            get => surfaceNormalOffset;
            set
            {
                if (surfaceNormalOffset != value)
                {
                    surfaceNormalOffset = value;
                }
            }
        }

        [SerializeField]
        [Tooltip("If true, the GameObjectToPlace will remain upright and in line with Vector3.up")]
        private bool keepOrientationVertical = false;

        /// <summary>
        /// If true, the GameObjectToPlace will remain upright and in line with Vector3.up
        /// </summary>
        public bool KeepOrientationVertical
        {
            get => keepOrientationVertical;
            set
            {
                if (keepOrientationVertical != value)
                {
                    keepOrientationVertical = value;
                }
            }
        }


        [SerializeField]
        [Tooltip("If true, the spatial mesh will be visible while the object is in the placing state.")]
        private bool spatialMeshVisible = true;

        /// <summary>
        /// If true, the spatial mesh will be visible while the object is in the placing state.
        /// </summary>
        public bool SpatialMeshVisible
        {
            get => spatialMeshVisible;
            set
            {
                if (spatialMeshVisible != value)
                {
                    spatialMeshVisible = value; 
                }
            }
        }

        [SerializeField]
        [Tooltip("If false, the game object to place will not change its rotation according to the surface hit.  The object will" +
            "remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal" +
            "if there is a hit.")]
        private bool rotateAccordingToSurface = false;

        /// <summary>
        /// If false, the game object to place will not change its rotation according to the surface hit.  The object will 
        /// remain facing the camera while it is in the placing state.  If true, the object will rotate according to the surface normal
        /// if there is a hit."
        /// </summary>
        public bool RotateAccordingToSurface
        {
            get => rotateAccordingToSurface;
            set
            {
                if (rotateAccordingToSurface != value)
                {
                    rotateAccordingToSurface = value;
                }
            }
        }

        private const int IgnoreRaycastLayer = 2;

        private const int DefaultLayer = 0;

        // The current ray is based on the TrackedTargetType (Controller Ray, Head, Hand Joint)
        protected RayStep currentRay;

        protected RaycastHit currentHit;

        protected float previousFrameNumber = 0;

        protected bool isSpatialMeshVisibleOnStart;


        protected override void Start()
        {
            // Solver is the base class
            base.Start();

            // If tap to place is added via script, set the GameObjectToPlace as this gameobject 
            if (GameObjectToPlace == null)
            {
                GameObjectToPlace = gameObject;
            }

            if(!ColliderPresent)
            {
                Debug.LogError("The GameObjectToPlace does not have a collider attached, please attach a collider");
            }

            // There might be cases where this does not work
            SurfaceNormalOffset = GetComponent<Collider>().bounds.extents.z;

            // Check if the Spatial Mesh is already visible on start
            isSpatialMeshVisibleOnStart  = SpatialMeshVisibilityState();

            SolverHandler.UpdateSolvers = false;

            IsBeingPlaced = false;

        }

        private void StartPlacement()
        {
            // Change the game object layer to ignore a raycast, so we can get a raycast hit on a surface in front of the 
            // game object to place
            gameObject.layer = IgnoreRaycastLayer;

            if (SpatialMeshVisible)
            {
                SpatialMeshVisibilityToggle(true);
            }

            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

            SolverHandler.UpdateSolvers = true;

            IsBeingPlaced = true;
        }

        private void StopPlacement()
        {
            // Change the physics layer back to default so it can be hit by a raycast, and recieve pointer events
            gameObject.layer = DefaultLayer;

            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);

            if (SpatialMeshVisible)
            {
                SpatialMeshVisibilityToggle(false);
            }

            SolverHandler.UpdateSolvers = false;

            IsBeingPlaced = false;
        }

        /// <inheritdoc/>
        public override void SolverUpdate()
        {
            PerformRaycast();
            SetPosition();
            SetRotation();
        }

        protected virtual void PerformRaycast()
        {
            if (SolverHandler.TransformTarget != null)
            {
                // The transform target is the transform of the TrackedTargetType, i.e. Controller Ray, Head or Hand Joint
                var transform = SolverHandler.TransformTarget;

                Vector3 origin = transform.position;
                Vector3 endpoint = transform.position + transform.forward;
                currentRay.UpdateRayStep(ref origin, ref endpoint);

                // Check if the current ray hit a magnetic surface
                DidHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(currentRay, MaxRaycastDistance, MagneticSurfaces, false, out currentHit);
            }
        }

        protected virtual void SetPosition()
        {
            // Change the position of the GameObjectToPlace if there was a hit, if not then place the object at the default distance
            // relative to the TrackedTargetType position

            if (DidHit)
            {
                // take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = currentHit.point;  
                AddOffset(currentHit.normal * SurfaceNormalOffset);
                
                // Draw the normal of the raycast hit for debugging 
                Debug.DrawRay(currentHit.point, currentHit.normal * 0.5f, Color.yellow);
            }
            else
            {
                GoalPosition = SolverHandler.TransformTarget.position + (SolverHandler.TransformTarget.forward * DefaultPlacementDistance);   
            }
        }

        protected virtual void SetRotation()
        {
            Vector3 direction = currentRay.Direction;
            Vector3 surfaceNormal = currentHit.normal;

            if (KeepOrientationVertical)
            {
                direction.y = 0;
                surfaceNormal.y = 0;
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // if the object is on a surface then change the rotation according to the normal of the hit point
            if (DidHit && rotateAccordingToSurface)
            {
                // There may need to be an option to opt out of matching the normal of the raycast hit
                // Maybe make an option to always match the direction even if there is a hit on a surface
                GoalRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);

            }
            else // if there is no result from the raycast hit, rotate the object based on the TrackedTargetType ray direction 
            {
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        private void SpatialMeshVisibilityToggle(bool spatialMeshVisibility)
        {
            // If the spatial mesh is already visible on start, then we do not need to toggle the spatial mesh 
            // if the object is in the placing state
            if (!isSpatialMeshVisibleOnStart)
            {
                IMixedRealityDataProviderAccess spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

                IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers = spatialAwarenessSystem.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

                foreach (IMixedRealitySpatialAwarenessMeshObserver observer in observers)
                {
                    // If the user wants the spatial mesh visible while in the placing state
                    if (spatialMeshVisibility)
                    {
                        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Visible;
                    }
                    else
                    {
                        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
                    }
                }
            }
        }

        private bool SpatialMeshVisibilityState()
        {
            IMixedRealityDataProviderAccess spatialAwarenessSystem = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

            IReadOnlyList<IMixedRealitySpatialAwarenessMeshObserver> observers = spatialAwarenessSystem.GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

            foreach (IMixedRealitySpatialAwarenessMeshObserver observer in observers)
            {
                if (observer.DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible)
                {
                    return true;
                }
            }
            return false;
        }


        #region IMixedRealityPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {

            // When a click is called in the same second then it is a mistake and no action needs to be taken
            Debug.Log("Time Difference: " + (Time.time - previousFrameNumber));
            if ((Time.time - previousFrameNumber) < 1.0f)
            {
                Debug.Log("Double Click has been caught, no actions triggered");
                return;

            }

            if (!IsBeingPlaced)
            {
                Debug.Log("Start Placement");
                StartPlacement();

            }
            else
            {
                Debug.Log("Stop Placement");
                StopPlacement();
            }

            // Get the time of this click action
            previousFrameNumber = Time.time;

        }

        #endregion

    }
}
