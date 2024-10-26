using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class XGameObject
{
    public GameObject obj;

    public string prefabPath => AssetDatabase.GetAssetPath(obj);
    public string prefabDir => Path.GetDirectoryName(prefabPath);

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

    public XGameObject? Find(string name)
    {
        GameObject? child = obj!.transform.Find(name).gameObject;
        return child == null ? null : new XGameObject(child);
    }

    public XMesh[] GetChildMeshes(
        XGameObject? root = null,
        string prefix = "",
        List<XMesh>? meshes = null
    )
    {
        root ??= this;
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
        return meshes!.ToArray();
    }

    public XGameObject? GetChildAtPath(string[] pathSplit)
    {
        if (pathSplit.Length == 0)
        {
            return this;
        }

        foreach (XGameObject child in children)
        {
            if (child.obj.name == pathSplit[0])
            {
                List<string> newPath = new List<string>(pathSplit);
                newPath.RemoveAt(0);
                return child.GetChildAtPath(newPath.ToArray());
            }
        }
        return null;
    }

    public void ApplyMesh(Mesh mesh)
    {
        if (smr != null)
        {
            smr.sharedMesh = mesh;
        }
        if (mf != null)
        {
            mf.sharedMesh = mesh;
        }
    }

    public static bool IsPrefab(GameObject obj) =>
        PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;

    public bool isPrefab
    {
        get => XGameObject.IsPrefab(this.obj);
    }

    public XGameObject Clone()
    {
        var memoryCopy = Object.Instantiate(obj);
        memoryCopy.hideFlags = HideFlags.HideAndDontSave;
        memoryCopy.name = obj.name + " (Clone)";
        memoryCopy.transform.parent = null;
        memoryCopy.SetActive(false);
        return new XGameObject(memoryCopy);
    }

    public XGameObject? SavePrefab(string suffix = "vrcrypted", string dir = "")
    {
        if (!isPrefab)
        {
            return null;
        }

        string newName = $"{obj.name}_{suffix}.prefab";
        string newPath = Path.Combine(dir, newName);
        GameObject? prefab = PrefabUtility.SaveAsPrefabAsset(obj, newPath);
        if (prefab == null)
        {
            Debug.LogError("Failed to save prefab");
            return null;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);

        Debug.Log($"New prefab saved as {newPath}");
        return new XGameObject(prefab);
    }
}
