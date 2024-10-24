using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class XMesh
{
    public string path;
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

    // UV channels
    public Vector2[] uv;
    public Vector2[] uv2;
    public Vector2[] uv3;
    public Vector2[] uv4;
    public Vector2[] uv5;
    public Vector2[] uv6;
    public Vector2[] uv7;
    public Vector2[] uv8;

    // Blend shapes
    public BlendShapeData[] blendShapes;

    [DebuggerHidden]
    public XMesh(Mesh mesh)
    {
        path = string.Empty;
        // Basic mesh data
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

        // UV channels
        uv = mesh.uv;
        uv2 = mesh.uv2;
        uv3 = mesh.uv3;
        uv4 = mesh.uv4;
        uv5 = mesh.uv5;
        uv6 = mesh.uv6;
        uv7 = mesh.uv7;
        uv8 = mesh.uv8;

        // Submeshes
        subMeshes = new SubMeshDescriptor[mesh.subMeshCount];
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            subMeshes[i] = mesh.GetSubMesh(i);
        }

        // Blend shapes
        List<BlendShapeData> blendShapes = new List<BlendShapeData>();
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            BlendShapeData blendShape = new BlendShapeData(mesh, i);
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
    public static XMesh FromMesh(Mesh mesh)
    {
        return new XMesh(mesh);
    }

    [DebuggerHidden]
    public static XMesh FromBytes(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<XMesh>(jsonString);
    }

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
            uv = this.uv,
            uv2 = this.uv2,
            uv3 = this.uv3,
            uv4 = this.uv4,
            uv5 = this.uv5,
            uv6 = this.uv6,
            uv7 = this.uv7,
            uv8 = this.uv8,

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

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        return mesh;
    }

    [DebuggerHidden]
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    [DebuggerHidden]
    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToJson());
    }

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
}
