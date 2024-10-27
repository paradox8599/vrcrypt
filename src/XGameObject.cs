using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class XGameObject
{
    public static bool IsPrefab(GameObject obj) =>
        PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;

    public GameObject obj;
    public string prefabPath => AssetDatabase.GetAssetPath(obj);
    public string prefabDir => Path.GetDirectoryName(prefabPath);
    public string path
    {
        // recursively find all parents and accumulately prepend their names
        get
        {
            var _path = obj.name;
            var parent = obj.transform.parent;
            while (parent != null)
            {
                _path = $"{parent.name}/{_path}";
                parent = parent.parent;
            }
            return _path;
        }
    }

    public XGameObject(GameObject obj)
    {
        if (obj == null)
        {
            throw new System.ArgumentNullException(nameof(obj));
        }
        this.obj = obj;
    }

    public static XGameObject? New(GameObject? obj)
    {
        return obj == null ? null : new XGameObject(obj);
    }

    public SkinnedMeshRenderer? smr => obj.GetComponent<SkinnedMeshRenderer>();
    public MeshFilter? mf => obj.GetComponent<MeshFilter>();
    public XMesh? xMesh =>
        smr != null ? new XMesh(smr.sharedMesh)
        : mf != null ? new XMesh(mf.sharedMesh)
        : null;

    public List<XGameObject> GetAllChildren(Transform? root = null)
    {
        root ??= obj.transform;
        List<XGameObject> children = new List<XGameObject>();
        foreach (Transform child in root)
        {
            children.Add(new XGameObject(child.gameObject));
            children.AddRange(GetAllChildren(child));
        }
        return children;
    }

    public List<XGameObject> GetAllChildrenWithMeshes() =>
        GetAllChildren().FindAll(x => x.xMesh != null);

    public List<XMesh> GetAllMeshes() => GetAllChildrenWithMeshes().ConvertAll(x => x.xMesh!);

    public Mesh? ApplyMesh(Mesh mesh) =>
        smr != null ? (smr.sharedMesh = mesh)
        : mf != null ? (mf.sharedMesh = mesh)
        : null;

    public bool isPrefab => XGameObject.IsPrefab(this.obj);

    public XGameObject MemoryClone()
    {
        var memoryCopy = Object.Instantiate(obj);
        memoryCopy.hideFlags = HideFlags.HideAndDontSave;
        memoryCopy.name = obj.name + " (Clone)";
        memoryCopy.transform.parent = null;
        memoryCopy.SetActive(false);
        return new XGameObject(memoryCopy);
    }

    // With Side Effects

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
