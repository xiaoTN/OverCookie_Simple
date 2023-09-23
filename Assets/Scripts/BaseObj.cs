using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VirtualStage.Core;

public abstract class BaseObj : MonoBehaviour
{
    [InlineButton(nameof(CreateMaterial))]
    public Color BaseColor;

    private void CreateMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = BaseColor;
        GetComponent<MeshRenderer>().material = mat;
        AssetDatabase.CreateAsset(mat,$"Assets/ArtResources/Materials/{gameObject.name}.mat");
        AssetDatabase.Refresh();
    }
    [FormerlySerializedAs("_gizmoName")]
    public string Name;
    protected abstract string GizmoLabel { get; }

    protected virtual void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = BaseColor;
    }

    private void OnDrawGizmos()
    {
        transform.Label($"【{Name}】\n{GizmoLabel}");
    }
}