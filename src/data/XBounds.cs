using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class XBounds
{
    public Vector3 center;
    public Vector3 extents;

    [DebuggerHidden]
    public XBounds(Bounds bounds)
    {
        center = bounds.center;
        extents = bounds.extents;
    }

    [DebuggerHidden]
    public Bounds ToBounds()
    {
        return new Bounds { center = center, extents = extents };
    }
}
