using System;
using UnityEngine;
using VirtualStage.Core;

public abstract class BaseObj : MonoBehaviour
{
    protected abstract string GizmoLabel { get; }

    private void OnDrawGizmos()
    {
        transform.Label(GizmoLabel);
    }
}