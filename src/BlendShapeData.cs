using UnityEngine;

[System.Serializable]
public class BlendShapeData
{
    public string name;
    public Vector3[] deltaVertices;
    public Vector3[] deltaNormals;
    public Vector3[] deltaTangents;
    public float frameWeight;
    public int frameCount;

    public BlendShapeData(Mesh mesh, int i)
    {
        this.name = mesh.GetBlendShapeName(i);
        this.frameCount = mesh.GetBlendShapeFrameCount(i);
        this.deltaVertices = new Vector3[mesh.vertexCount];
        this.deltaNormals = new Vector3[mesh.vertexCount];
        this.deltaTangents = new Vector3[mesh.vertexCount];
        this.frameWeight = mesh.GetBlendShapeFrameWeight(i, frameCount - 1);
    }
}
