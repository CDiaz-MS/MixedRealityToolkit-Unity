// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    
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
                // ========================== Check on this =====================================
                if (value != null && gameObjectToPlace != value)
                {
                    gameObjectToPlace = value;

                    // Make sure new game object has a collider
                    if (!ColliderPresent)
                    {
                        Debug.LogError("GameObjectToPlace does not have a collider attached, please add a collider to GameObjectToPlace");
                    }
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
            "The GameObjectToPlace will be placed at this distance if a surface is not hit by the raycast.")]
        private float defaultPlacementDistance = 1.5f;

        /// <summary>
        /// The default distance (in meters) an object will be placed relative to the TrackedTargetType forward in the SolverHandler.
        /// The GameObjectToPlace will be placed at this distance if a surface is not hit by the raycast.
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
        /// Is the gameObjectToPlace currently in a state where it is being placed? This state is activated when you select and hold 
        /// the GameObjectToPlace.
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        [SerializeField]
        [Tooltip("The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface")]
        private float surfaceNormalOffset = 0.1f;

        /// <summary>
        /// The distance between the center of the gameobject to place and a surface along the surface normal, if the raycast hits a surface
        /// </summary>
        public float SurfaceNormalOffset
        {
            get => surfaceNormalOffset;
            set
            {
                if (surfaceNormalOffset != value)
                {
                    surfaceNormalOffset = value;
                    // Get info from the box collider to calculate the offset

                    // ========================== Check on this =====================================

                }
            }
        }

        [SerializeField]
        [Tooltip("If true, the GameObjectToPlace will remain upright and parallel to Vector3.up")]
        private bool keepOrientationVertical = false;

        /// <summary>
        /// If true, the GameObjectToPlace will remain upright and parallel to Vector3.up
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

        const int IgnoreRaycastLayer = 2;

        const int DefaultLayer = 0;

        // If true, the raycast did hit a surface
        protected bool didHit;

        // The current ray is based on the TrackedTargetType (Controller Ray, Head, Hand Joint)
        protected RayStep currentRay;

        protected RaycastHit currentHit;

        protected override void Start()
        {
            base.Start();

            // If tap to place is added via script, set the GameObjectToPlace as this gameobject 
            if (GameObjectToPlace == null)
            {
                GameObjectToPlace = gameObject;
            }

            IsBeingPlaced = false;

            // Set the default target transform for the game object to place based on the controller ray!!!!!!!!!!!!
            SolverHandler.TrackedTargetType = TrackedObjectType.ControllerRay;

            SolverHandler.UpdateSolvers = false;
        }

        private void StartPlacement()
        {

            if (GameObjectToPlace != null)
            {

                // Make sure there is a collider present on the object to ignore the raycast
                if (ColliderPresent)
                {
                    // move the gameobject to the 2nd layer to ignore a raycast, so we can get a raycast hit on a surface in front of the 
                    // game object to place
                    gameObject.layer = IgnoreRaycastLayer;

                }

                SolverHandler.UpdateSolvers = true;

                IsBeingPlaced = true;

            }
        }

        private void StopPlacement()
        {
            // Change the physics layer back to default so it can be hit by a raycast, and recieve pointer events
            gameObject.layer = DefaultLayer;

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
                didHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(currentRay, MaxRaycastDistance, MagneticSurfaces, false, out currentHit);
            }
        }

        protected virtual void SetPosition()
        {
            // Change the position of the GameObjectToPlace if there was a hit, if not then place the object at the default distance
            // relative to the TrackedTargetType position

            if (didHit)
            {
                // take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = currentHit.point + (currentHit.normal * SurfaceNormalOffset);
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

            // if the object is on a surface then change the rotation according to the normal of the hit point
            if (didHit)
            {
                GoalRotation = Quaternion.LookRotation(-surfaceNormal, Vector3.up);
            }
            else // if there is no result from the raycast hit, rotate the object based on the TrackedTargetType ray direction 
            {
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        #region IMixedRealityPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            // Update solvers
            Debug.Log("Start Placement");
            StartPlacement();
        }

        /// <inheritdoc/>
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        /// <inheritdoc/>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            Debug.Log("Stop Placement");
            StopPlacement();
        }

        /// <inheritdoc/>
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

        #endregion

    }
}
