// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Layout
{
    public enum CornerPoint
    {
        LeftBottomForward = 0,
        LeftBottomBack,
        LeftTopForward ,
        LeftTopBack,
        RightBottomForward,
        RightBottomBack,
        RightTopForward,
        RightTopBack,

        // [0] == LeftBottomForward
        // [1] == LeftBottomBack
        // [2] == LeftTopForward
        // [3] == LeftTopBack

        // [4] == RightBottomForward
        // [5] == RightBottomBack
        // [6] == RightTopForward
        // [7] == RightTopBack
    }
}
