using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class XBlendShape
{
    public string name;
    public XBlendShapeFrame[] frames;

    [DebuggerHidden]
    public XBlendShape(Mesh mesh, int blendShapeIndex)
    {
        name = mesh.GetBlendShapeName(blendShapeIndex);
        int frameCount = mesh.GetBlendShapeFrameCount(blendShapeIndex);
        frames = new XBlendShapeFrame[frameCount];

        for (int i = 0; i < frameCount; i++)
        {
            frames[i] = new XBlendShapeFrame(mesh, blendShapeIndex, i);
        }
    }

    [DebuggerHidden]
    public void ApplyToMesh(Mesh mesh)
    {
        for (int i = 0; i < frames.Length; i++)
        {
            mesh.AddBlendShapeFrame(
                name,
                frames[i].weight,
                frames[i].deltaVertices,
                frames[i].deltaNormals,
                frames[i].deltaTangents
            );
        }
    }
}

[Serializable]
public class XBlendShapeFrame
{
    public float weight;
    public Vector3[] deltaVertices;
    public Vector3[] deltaNormals;
    public Vector3[] deltaTangents;

    [DebuggerHidden]
    public XBlendShapeFrame(Mesh mesh, int blendShapeIndex, int frameIndex)
    {
        weight = mesh.GetBlendShapeFrameWeight(blendShapeIndex, frameIndex);
        deltaVertices = new Vector3[mesh.vertexCount];
        deltaNormals = new Vector3[mesh.vertexCount];
        deltaTangents = new Vector3[mesh.vertexCount];
        mesh.GetBlendShapeFrameVertices(
            blendShapeIndex,
            frameIndex,
            deltaVertices,
            deltaNormals,
            deltaTangents
        );
    }
}
