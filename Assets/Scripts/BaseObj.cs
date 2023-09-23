using System;
using UnityEngine;
using VirtualStage.Core;

public abstract class BaseObj : MonoBehaviour
{
    [SerializeField]
    private string _gizmoName;
    protected abstract string GizmoLabel { get; }

    private void OnDrawGizmos()
    {
        transform.Label($"【{_gizmoName}】\n{GizmoLabel}");
    }
}