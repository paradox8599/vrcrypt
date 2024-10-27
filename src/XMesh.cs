using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class XMeshes
{
    public List<XMesh> meshes;

    public XMeshes(List<XMesh> meshes)
    {
        this.meshes = meshes;
    }

    public static XMeshes FromJson(string data) => JsonUtility.FromJson<XMeshes>(data);

    public static string ToJson(List<XMesh> meshes) => new XMeshes(meshes).ToJson();

    public string ToJson() => JsonUtility.ToJson(this);
}

[System.Serializable]
public class XMesh
{
    public static string savePath = "Assets/VRCrypt/data";

    public static string AssetPathFromHash(string hash, string ext = "asset") =>
        Path.Combine(savePath, $"{hash}.{ext}");

    public static Mesh? LoadAsset(string hash) =>
        AssetDatabase.LoadAssetAtPath<Mesh>(AssetPathFromHash(hash));

    private string? _hash = null;
    public string hash => _hash ??= ToHash();
    public string assetPath => AssetPathFromHash(hash);
    public string filePath => AssetPathFromHash(hash, "json");

    public string path;
    public string[] pathSplit => path.TrimStart('/').Split('/');

    // mesh data

    public string name;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector4[] tangents;
    public Color[] colors;
    public BoneWeight[] boneWeights;
    public Matrix4x4[] bindposes;
    public Vector3 boundsCenter;
    public Vector3 boundsExtents;
    public SubMeshDescriptor[]? subMeshes;
    public List<Vector2[]>? uvs = new List<Vector2[]>();
    public XBlendShape[] blendShapes;

    // Constructor & Deserialzions

    public XMesh(Mesh mesh)
    {
        UnityEngine.Debug.Log("XMesh");
        if (mesh.vertices.Length == 0)
        {
            UnityEngine.Debug.LogError("No vertices");
        }
        if (mesh.triangles.Length == 0)
        {
            UnityEngine.Debug.LogError("No triangles");
        }
        if (mesh.normals.Length == 0)
        {
            UnityEngine.Debug.LogError("No normals");
        }
        if (mesh.colors.Length == 0)
        {
            UnityEngine.Debug.LogError("No colors");
        }
        if (mesh.boneWeights.Length == 0)
        {
            UnityEngine.Debug.LogError("No boneWeights");
        }
        if (mesh.bindposes.Length == 0)
        {
            UnityEngine.Debug.LogError("No bindposes");
        }
        if (mesh.blendShapeCount == 0)
        {
            UnityEngine.Debug.LogError("No blendShapes");
        }
        if (mesh.subMeshCount == 0)
        {
            UnityEngine.Debug.LogError("No subMeshes");
        }

        path = string.Empty;
        name = mesh.name;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        normals = mesh.normals;
        tangents = mesh.tangents;
        colors = mesh.colors;
        boneWeights = mesh.boneWeights;
        bindposes = mesh.bindposes;
        boundsCenter = mesh.bounds.center;
        boundsExtents = mesh.bounds.extents;

        subMeshes = new SubMeshDescriptor[mesh.subMeshCount];
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            subMeshes[i] = mesh.GetSubMesh(i);
        }

        uvs = new List<Vector2[]>();
        uvs.Add(mesh.uv);
        uvs.Add(mesh.uv2);
        uvs.Add(mesh.uv3);
        uvs.Add(mesh.uv4);
        uvs.Add(mesh.uv5);
        uvs.Add(mesh.uv6);
        uvs.Add(mesh.uv7);
        uvs.Add(mesh.uv8);

        blendShapes = new XBlendShape[mesh.blendShapeCount];
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            blendShapes[i] = new XBlendShape(mesh, i);
        }
    }

    [DebuggerHidden]
    public static XMesh FromBytes(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<XMesh>(jsonString);
    }

    public Mesh? LoadAsset() => AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

    // Serialize

    public Mesh ToMesh()
    {
        UnityEngine.Debug.Log("ToMesh: " + name);
        if (vertices.Length == 0)
        {
            UnityEngine.Debug.LogError("No vertices");
        }
        if (triangles.Length == 0)
        {
            UnityEngine.Debug.LogError("No triangles");
        }
        if (normals.Length == 0)
        {
            UnityEngine.Debug.LogError("No normals");
        }
        if (colors.Length == 0)
        {
            UnityEngine.Debug.LogError("No colors");
        }
        if (boneWeights.Length == 0)
        {
            UnityEngine.Debug.LogError("No boneWeights");
        }
        if (bindposes.Length == 0)
        {
            UnityEngine.Debug.LogError("No bindposes");
        }
        if (blendShapes.Length == 0)
        {
            UnityEngine.Debug.LogError("No blendShapes");
        }
        if (subMeshes == null || subMeshes.Length == 0)
        {
            UnityEngine.Debug.LogError("No subMeshes");
        }
        Mesh mesh = new Mesh();
        mesh.name = name;

        mesh.vertices = vertices;

        mesh.triangles = triangles;

        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.colors = colors;
        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindposes;
        mesh.bounds = new Bounds(boundsCenter, boundsExtents * 2);

        mesh.SetSubMeshes(subMeshes);
        // mesh.subMeshCount = subMeshes != null ? subMeshes.Length : 0;
        // for (int i = 0; subMeshes != null && i < subMeshes.Length; i++)
        // {
        //     mesh.SetSubMesh(i, subMeshes[i]);
        // }

        if (uvs != null)
        {
            for (int i = 0; i < uvs.Count; i++)
            {
                mesh.SetUVs(i, uvs[i]);
            }
        }

        for (int i = 0; i < blendShapes.Length; i++)
        {
            blendShapes[i].ApplyToMesh(mesh);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    [DebuggerHidden]
    public byte[] ToBytes() => Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));

    [DebuggerHidden]
    public byte[] ToHashBytes()
    {
        using SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(ToBytes());
    }

    [DebuggerHidden]
    public string ToHash()
    {
        StringBuilder builder = new StringBuilder();
        foreach (byte b in ToHashBytes())
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }

    public XMesh ToRandomized()
    {
        var mesh = ToMesh();
        var vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = Random.onUnitSphere;
        }
        mesh.vertices = vertices;
        var x = new XMesh(mesh);
        x.path = path;
        return x;
    }

    // Side Effects

    public void SaveAsset()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        var mesh = ToMesh();
        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
