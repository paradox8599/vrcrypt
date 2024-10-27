using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class XMeshList
{
    public List<XMesh> meshes;

    public XMeshList(List<XMesh> meshes)
    {
        this.meshes = meshes;
    }
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
    public SubMeshDescriptor[] subMeshes;
    public List<List<Vector2>> uvs = new List<List<Vector2>>();
    public XBlendShape[] blendShapes;

    // Constructor & Deserialzions

    [DebuggerHidden]
    public XMesh(Mesh mesh)
    {
        path = string.Empty;
        // Basic mesh data
        name = mesh.name ?? string.Empty;
        vertices = mesh.vertices ?? new Vector3[0];
        triangles = mesh.triangles ?? new int[0];
        normals = mesh.normals ?? new Vector3[0];
        tangents = mesh.tangents ?? new Vector4[0];
        colors = mesh.colors ?? new Color[0];
        boneWeights = mesh.boneWeights ?? new BoneWeight[0];
        bindposes = mesh.bindposes ?? new Matrix4x4[0];
        boundsCenter = mesh.bounds.center;
        boundsExtents = mesh.bounds.extents;

        // UV channels
        for (int i = 0; i < 8; i++)
        {
            var o = new List<Vector2>();
            mesh.GetUVs(i, o);
            uvs.Add(o);
        }

        // Submeshes
        subMeshes = new SubMeshDescriptor[mesh.subMeshCount];
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            subMeshes[i] = mesh.GetSubMesh(i);
        }

        // Blend shapes
        List<XBlendShape> blendShapes = new List<XBlendShape>();
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            XBlendShape blendShape = new XBlendShape(mesh, i);
            mesh.GetBlendShapeFrameVertices(
                i,
                blendShape.frameCount - 1,
                blendShape.deltaVertices,
                blendShape.deltaNormals,
                blendShape.deltaTangents
            );
            blendShapes.Add(blendShape);
        }
        this.blendShapes = blendShapes.ToArray();
    }

    [DebuggerHidden]
    public static XMesh FromBytes(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<XMesh>(jsonString);
    }

    public Mesh? LoadAsset() => AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

    // Serialize

    [DebuggerHidden]
    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh
        {
            name = this.name,

            // Basic mesh data
            vertices = this.vertices,
            triangles = this.triangles,
            normals = this.normals,
            tangents = this.tangents,
            colors = this.colors,
            boneWeights = this.boneWeights,
            bindposes = this.bindposes,
            bounds = new Bounds(this.boundsCenter, this.boundsExtents * 2),

            // UV channels
            uv = this.uvs[0].ToArray(),
            uv2 = this.uvs[1].ToArray(),
            uv3 = this.uvs[2].ToArray(),
            uv4 = this.uvs[3].ToArray(),
            uv5 = this.uvs[4].ToArray(),
            uv6 = this.uvs[5].ToArray(),
            uv7 = this.uvs[6].ToArray(),
            uv8 = this.uvs[7].ToArray(),

            // Submeshes
            subMeshCount = this.subMeshes.Length,
        };

        for (int i = 0; i < this.subMeshes.Length; i++)
        {
            mesh.SetSubMesh(i, this.subMeshes[i]);
        }

        // Blend shapes
        for (int i = 0; i < this.blendShapes.Length; i++)
        {
            var blendShape = this.blendShapes[i];
            mesh.AddBlendShapeFrame(
                blendShape.name,
                blendShape.frameWeight,
                blendShape.deltaVertices,
                blendShape.deltaNormals,
                blendShape.deltaTangents
            );
        }

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

    // Side Effects

    public void SaveAsset()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        AssetDatabase.CreateAsset(ToMesh(), assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
