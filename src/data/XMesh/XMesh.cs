using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public partial class XMesh
{
    public string path;
    public string name;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector4[] tangents;
    public XColor[] colors;
    public BoneWeight[] boneWeights;
    public Matrix4x4[] bindposes;
    public XBlendShape[] blendShapes;
    public List<XSubMeshDescriptor> subMeshes = new List<XSubMeshDescriptor>();
    public Vector2[] uv;
    public Vector2[] uv2;
    public Vector2[] uv3;
    public Vector2[] uv4;
    public Vector2[] uv5;
    public Vector2[] uv6;
    public Vector2[] uv7;
    public Vector2[] uv8;

    [DebuggerHidden]
    public Mesh SaveAsset(string? path = "Assets/VRCrypt/data")
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Combine(path, $"{hash}.asset");
        var mesh = ToMesh();
        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return mesh;
    }

    [DebuggerHidden]
    public static Mesh? LoadAsset(string path) => AssetDatabase.LoadAssetAtPath<Mesh>(path);

    public static bool? IsFbx(Mesh mesh) =>
        AssetDatabase.GetAssetPath(mesh)?.EndsWith(".fbx") ?? false;
}
