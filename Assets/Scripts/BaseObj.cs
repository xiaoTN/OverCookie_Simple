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

    public int Id
    {
        get
        {
            return GetInstanceID();
        }
    }

    public string GUID;

    private void Reset()
    {
        GUID = Guid.NewGuid().ToString();
    }

    private void CreateMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = BaseColor;
        GetComponent<MeshRenderer>().material = mat;
        AssetDatabase.CreateAsset(mat,$"Assets/ArtResources/Materials/{gameObject.name}.mat");
        AssetDatabase.Refresh();
    }
    [FormerlySerializedAs("Name")]
    [FormerlySerializedAs("_gizmoName")]
    [SerializeField]
    private string _name;

    public string SingleName
    {
        get
        {
            return $"{_name} {Id}";
        }
    }
    protected abstract string GizmoLabel { get; }

    protected virtual void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = BaseColor;
    }

    private void OnDrawGizmos()
    {
        transform.Label($"【{SingleName}】\n{GizmoLabel}");
    }

    protected void Log(string message)
    {
        Debug.Log($"[{SingleName}] {message}",this);
    }
    protected void LogWarning(string message)
    {
        Debug.LogWarning($"[{SingleName}] {message}",this);
    }

    protected void LogError(string message)
    {
        Debug.LogError($"[{SingleName}] {message}",this);
    }
}