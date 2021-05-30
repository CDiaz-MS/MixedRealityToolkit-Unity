// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererBounds : MonoBehaviour
{
    public Bounds bounds;

    void OnDrawGizmos()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;

        // Add all the renderer bounds in the hierarchy 
        foreach (Renderer r in renderers) { bounds.Encapsulate(r.bounds); }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
