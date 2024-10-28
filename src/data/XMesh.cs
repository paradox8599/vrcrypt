using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

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

    public string[] pathSplit => path.TrimStart('/').Split('/');

    // mesh data

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

    // public Vector3[] normals;
    // public XBounds bounds;

    // Constructor & Deserialzions

    public XMesh(Mesh mesh)
    {
        path = string.Empty;
        name = mesh.name;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        tangents = mesh.tangents;
        colors = XColor.FromColors(mesh.colors);
        boneWeights = mesh.boneWeights;
        bindposes = mesh.bindposes;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        // normals = mesh.normals;
        // bounds = new XBounds(mesh.bounds);

        uv = mesh.uv;
        uv2 = mesh.uv2;
        uv3 = mesh.uv3;
        uv4 = mesh.uv4;
        uv5 = mesh.uv5;
        uv6 = mesh.uv6;
        uv7 = mesh.uv7;
        uv8 = mesh.uv8;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            subMeshes.Add(new XSubMeshDescriptor(mesh.GetSubMesh(i)));
        }

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
        Mesh mesh = new Mesh();
        mesh.name = name;

        mesh.vertices = vertices;

        mesh.triangles = triangles;
        mesh.tangents = tangents;
        mesh.colors = XColor.ToColors(colors);
        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindposes;

        mesh.SetSubMeshes(subMeshes.ConvertAll(x => x.ToSubMeshDescriptor()));

        mesh.uv = uv;
        mesh.uv2 = uv2;
        mesh.uv3 = uv3;
        mesh.uv4 = uv4;
        mesh.uv5 = uv5;
        mesh.uv6 = uv6;
        mesh.uv7 = uv7;
        mesh.uv8 = uv8;

        for (int i = 0; i < blendShapes.Length; i++)
        {
            blendShapes[i].ApplyToMesh(mesh);
        }

        // mesh.bounds = bounds.ToBounds();
        // mesh.normals = normals;
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
