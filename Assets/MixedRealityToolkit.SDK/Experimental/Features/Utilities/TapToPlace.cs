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
    public class TapToPlace : Solver, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        [SerializeField]
        [Tooltip("The Game Object that will be moved by the tap gesture if selected")]
        private GameObject gameObjectToPlace;

        /// <summary>
        /// The Game Object that will be moved by the tap gesture if selected
        /// </summary>
        public GameObject GameObjectToPlace
        {
            get => gameObjectToPlace;
            set
            {
                if (value != null && gameObjectToPlace != value )
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

        //[SerializeField]
        //[Tooltip("A collider must be present on the game object for tap to place to work")]
        //private bool colliderPresent = false;

        /// <summary>
        /// A collider must be present on the game object for tap to place to work
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

        //[SerializeField]
        //[Tooltip("On Application start, start the placement of the gameObject")]
        //private bool autoStart = false;

        ///// <summary>
        ///// On Application start, start the placement of the gameObject
        ///// </summary>
        //public bool AutoStart
        //{
        //    get => autoStart;
        //    set => autoStart = value;
        //}

        [SerializeField]
        [Tooltip("If spatial awareness is not enabled, the game object will be placed at a default distance (in meters)")]
        private float defaultPlacementDistance = 1.5f;

        /// <summary>
        /// If spatial awareness is not enabled, the game object will be placed at a default distance (in meters).
        /// If there is no result from the raycast, the gameobejct will be place a default distance relative to the
        /// Tracked Target Type, in the Solver Handler which is the source of the raycast.
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
        [Tooltip("Max distance for raycast to check for surfaces")]
        private float maxRaycastDistance = 20.0f;

        /// <summary>
        /// Max distance for raycast to check for a raycast hit on a surface
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
        /// Contains unity physics layers and the spatial awarness physics layer which is the spatial mesh.
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get => magneticSurfaces; 
            set => magneticSurfaces = value;
        }

        [SerializeField]
        [Tooltip("Keywords for the speech provider to place an object")]
        private List<string> keywords;

        public List<string> Keywords
        {
            get => keywords; 
            set => keywords = value; 
        }

        /// <summary>
        /// Is the gameObjectToPlace currently in a state where it is being placed?
        /// </summary>
        public bool IsBeingPlaced { get; protected set; }

        // auto-start, if false disables SolverHandler?


        [SerializeField]
        [Tooltip("If the object is on a surface, what is the offset from the surface?")]
        private float surfaceNormalOffset = 0.1f;

        /// <summary>
        /// Offset from surface along surface normal
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

                }
            }
        }

        [SerializeField]
        [Tooltip("If true, ensures object is kept vertical for TrackedTarget, SurfaceNormal")]
        private bool keepOrientationVertical = true;

        /// <summary>
        /// If true, ensures object is kept vertical for TrackedTarget, SurfaceNormal, and Blended Orientation Modes
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

        protected bool didHit;

        protected RaycastHit currentHit;

        protected RayStep currentRay;

        protected override void Start()
        {
            base.Start();

            if (GameObjectToPlace == null)
            {
                GameObjectToPlace = gameObject;
            }

            IsBeingPlaced = false;

            // Default tap to place behavior is based on the controller ray
            SolverHandler.TrackedTargetType = TrackedObjectType.ControllerRay;

            // Check status of auto start, not sure about it 
            SolverHandler.UpdateSolvers = false;

            //if (AutoStart)
            //{
            //    StartPlacement();
            //}
        }

        // We need to override OnEnable because we inherit from the solvers
        protected override void OnEnable()
        {
            // Add keywords
           // keywords.Add("Select");
            //keywords.Add("Place");
            
        }

        private void OnDisable()
        {

        }

        public void StartPlacement()
        {
            // If we are placing the gameobject and the gameobject has a collider
            // if the gameobject we are placing has a collider
            //   we need to move the object to the ignore raycast while we are placing the object
            // But then if the gameobject does not have a collider then how will we select and place it?

            if (ColliderPresent)
            {
                // move the gameobject to the 2nd layer to ignore a raycast for now
                gameObject.layer = IgnoreRaycastLayer;

            }


            SolverHandler.UpdateSolvers = true;


            // Enable pointer events to be detected
            //CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);

            // turn off colliders under GameObject?
            // Make sure the gameobject has a collider

            IsBeingPlaced = true;
        }

        public void StopPlacement()
        {
            // Change the physics layer back to default so it can be hit by a raycast
            gameObject.layer = DefaultLayer;

            // Stop updating the solvers
            SolverHandler.UpdateSolvers = false;

            //CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
            //CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);

            // turn on colliders under GameObject? Need to save set
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
                // Source of the calculations
                // Head ray
                // Controller ray
                // Ray casted from a specific joint 
                var transform = SolverHandler.TransformTarget;

                if (SolverHandler.TrackedTargetType == TrackedObjectType.Head)
                {

                }
                
                Vector3 origin = transform.position;
                Vector3 endpoint = transform.position + transform.forward;
                currentRay.UpdateRayStep(ref origin, ref endpoint);

                // Check if the raycast hit something, if true the raycast hit something which means it went
                // throught the layer masks in priority and got a hit
                didHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(currentRay, MaxRaycastDistance, MagneticSurfaces, false, out currentHit);
            }
        }

        protected virtual void SetPosition()
        {
            // Change the position of the object if there was a hit, if not then place the object at the default distance
            // relative to whatever the tracking type is. So if the controller ray, take the forward and add default distance

            if (didHit)
            {
                // take the current hit point and add an offset relative to the surface to avoid half of the object in the surface
                GoalPosition = currentHit.point + (SurfaceNormalOffset * currentHit.normal);
            }
            else // if the raycast did not hit anything set it to the forward of the solver handler source
            {
                if (SolverHandler.TransformTarget != null)
                {
                    GoalPosition = SolverHandler.TransformTarget.position + (SolverHandler.TransformTarget.forward * DefaultPlacementDistance);
                }
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
            else
            {
                GoalRotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            // if the object is not on a surface, look at opposite of the source ray


            
        }

        #region IMixedRealityPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(MixedRealityPointerEventData eventData) {
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

        #region IMixedRealitySpeechHandler

        /// <inheritdoc/>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (enabled && IsBeingPlaced && Keywords.Contains(eventData.Command.Keyword.ToLower()))
            {
                StopPlacement();
            }
        }

        #endregion
    }
}
