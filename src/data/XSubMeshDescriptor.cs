using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class XSubMeshDescriptor
{
    public XBounds bounds;

    public MeshTopology topology;

    public int indexStart;
    public int indexCount;
    public int baseVertex;
    public int firstVertex;
    public int vertexCount;

    public XSubMeshDescriptor(SubMeshDescriptor data)
    {
        bounds = new XBounds(data.bounds);
        topology = data.topology;
        indexStart = data.indexStart;
        indexCount = data.indexCount;
        baseVertex = data.baseVertex;
        firstVertex = data.firstVertex;
        vertexCount = data.vertexCount;
    }

    public SubMeshDescriptor ToSubMeshDescriptor()
    {
        return new SubMeshDescriptor
        {
            bounds = bounds.ToBounds(),
            topology = topology,
            indexStart = indexStart,
            indexCount = indexCount,
            baseVertex = baseVertex,
            firstVertex = firstVertex,
            vertexCount = vertexCount,
        };
    }
}
