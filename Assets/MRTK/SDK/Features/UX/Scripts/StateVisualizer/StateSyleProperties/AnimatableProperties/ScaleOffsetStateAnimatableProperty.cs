﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// The ScaleOffset animatable property adds/sets keyframes for the "localScale" property in an animation clip.
    /// </summary>
    public class ScaleOffsetStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The scale offset added to the current scale of the target object.")]
        private Vector3 scaleoffset;

        /// <summary>
        /// The scale offset added to the current scale of the target object.
        /// </summary>
        public Vector3 ScaleOffset
        {
            get => scaleoffset;
            set => scaleoffset = value;
        }

        public ScaleOffsetStateAnimatableProperty()
        {
            AnimatablePropertyName = "ScaleOffset";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                AnimationCurve curveX = AnimationCurve.EaseInOut(0, Target.transform.localScale.x, AnimationDuration, Target.transform.localScale.x + ScaleOffset.x);
                AnimationCurve curveY = AnimationCurve.EaseInOut(0, Target.transform.localScale.y, AnimationDuration, Target.transform.localScale.y + ScaleOffset.y);
                AnimationCurve curveZ = AnimationCurve.EaseInOut(0, Target.transform.localScale.z, AnimationDuration, Target.transform.localScale.z + ScaleOffset.z);

                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.x", curveX);
                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.y", curveY);
                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.z", curveZ);
            }
        }

        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                animationClip.SetCurve(targetPath, typeof(Transform), "localScale", null);
            }
        }
    }
}