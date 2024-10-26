using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class XGameObject
{
    public GameObject obj;

    public XGameObject(GameObject obj)
    {
        if (obj == null)
        {
            throw new System.ArgumentNullException(nameof(obj));
        }
        this.obj = obj;
    }

    public SkinnedMeshRenderer? smr
    {
        get
        {
            obj!.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer? _smr);
            return _smr;
        }
    }

    public MeshFilter? mf
    {
        get
        {
            obj!.TryGetComponent<MeshFilter>(out MeshFilter? _mf);
            return _mf;
        }
    }

    public XMesh? xMesh
    {
        get
        {
            if (smr != null)
            {
                return new XMesh(smr.sharedMesh);
            }
            if (mf != null)
            {
                return new XMesh(mf.sharedMesh);
            }
            return null;
        }
    }

    public XGameObject? FindChild(string name)
    {
        GameObject? child = obj!.transform.Find(name).gameObject;
        return child == null ? null : new XGameObject(child);
    }

    public XGameObject[] children
    {
        get
        {
            List<XGameObject> children = new List<XGameObject>();
            foreach (Transform child in obj!.transform)
            {
                children.Add(new XGameObject(child.gameObject));
            }
            return children.ToArray();
        }
    }

    private XMesh[] GetChildMeshes(XGameObject root, string prefix = "", List<XMesh>? meshes = null)
    {
        meshes ??= new List<XMesh>();

        foreach (XGameObject child in root.children)
        {
            var path = $"{prefix}/{child.obj.name}";
            if (child.xMesh != null)
            {
                XMesh x = child.xMesh;
                x.path = path;
                meshes.Add(x);
            }
            GetChildMeshes(child, path, meshes);
        }
        return meshes.ToArray();
    }

    public XMesh[] meshes
    {
        get { return GetChildMeshes(this); }
    }

    public static bool IsPrefab(GameObject obj) => PrefabUtility.IsPartOfPrefabAsset(obj);

    public bool isPrefab
    {
        get => XGameObject.IsPrefab(this.obj);
    }
}
