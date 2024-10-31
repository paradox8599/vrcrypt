using System.Diagnostics;
using UnityEngine;

public partial class XMesh
{
  [DebuggerHidden]
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

    mesh.RecalculateBounds();
    mesh.RecalculateNormals();

    return mesh;
  }
}
