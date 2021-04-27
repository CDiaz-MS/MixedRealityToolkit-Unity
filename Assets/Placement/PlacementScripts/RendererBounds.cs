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
        //Gizmos.matrix = transform.localToWorldMatrix;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        foreach (Renderer r in renderers) { bounds.Encapsulate(r.bounds); }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
