using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class XGameObject
{
    [DebuggerHidden]
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
            var _path = new List<string>();
            _path.Add(obj.name);
            var parent = obj.transform.parent;
            while (parent != null)
            {
                _path.Insert(0, parent.name);
                parent = parent.parent;
            }
            // remove the root gameobject
            _path.RemoveAt(0);
            return string.Join("/", _path);
        }
    }

    [DebuggerHidden]
    public XGameObject(GameObject obj)
    {
        this.obj = obj;
    }

    [DebuggerHidden]
    public static XGameObject? New(GameObject? obj)
    {
        return obj == null ? null : new XGameObject(obj);
    }

    public SkinnedMeshRenderer? smr => obj.GetComponent<SkinnedMeshRenderer>();
    public MeshFilter? mf => obj.GetComponent<MeshFilter>();
    public XMesh? xMesh
    {
        get
        {
            var mesh =
                smr != null ? new XMesh(smr.sharedMesh)
                : mf != null ? new XMesh(mf.sharedMesh)
                : null;
            if (mesh == null)
                return null;
            mesh.path = path;
            return mesh;
        }
    }

    [DebuggerHidden]
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

    [DebuggerHidden]
    public List<XGameObject> GetAllChildrenWithMeshes() =>
        GetAllChildren().FindAll(x => x.xMesh != null);

    [DebuggerHidden]
    public List<XMesh> GetAllMeshes() => GetAllChildrenWithMeshes().ConvertAll(x => x.xMesh!)!;

    [DebuggerHidden]
    public Mesh? ApplyMesh(Mesh mesh) =>
        smr != null ? (smr.sharedMesh = mesh)
        : mf != null ? (mf.sharedMesh = mesh)
        : null;

    public bool isPrefab => XGameObject.IsPrefab(this.obj);

    [DebuggerHidden]
    public XGameObject InMemoryClone(bool hide = false)
    {
        var memoryCopy = UnityEngine.Object.Instantiate(obj.gameObject);
        if (hide)
        {
            memoryCopy.hideFlags = HideFlags.HideInHierarchy;
            memoryCopy.SetActive(false);
        }
        memoryCopy.name = obj.name;
        memoryCopy.transform.SetParent(null, false);
        return new XGameObject(memoryCopy);
    }

    // With Side Effects

    [DebuggerHidden]
    public XGameObject? SavePrefab(string dir, string suffix = "_vrcrypted")
    {
        obj.name = $"{obj.name}{suffix}";
        string newPath = Path.Combine(dir, $"{obj.name}.prefab");
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

    [DebuggerHidden]
    public void SaveRandomized(string targetPrefabDir)
    {
        targetPrefabDir = Path.Combine(targetPrefabDir, "vrcrypted");
        // Generate random meshes
        var allmeshes = GetAllMeshes();
        var xMeshes = new List<XMesh>();
        foreach (var x in allmeshes)
        {
            var m = new List<XMesh>();
            m.Add(x);
            var output = FFI.Meshes.Randomize(m, 0.01f);
            xMeshes.Add(output.meshes[0]);
        }

        var meshDir = Path.Combine(targetPrefabDir, "meshes");

        // replace meshes
        foreach (var g in GetAllChildrenWithMeshes())
        {
            var xMesh = xMeshes.Find(x => x.path == g.path);
            if (xMesh == null)
            {
                Debug.LogError($"Mesh not found for: {g.path}");
            }
            xMesh!.SaveAsset(meshDir);

            var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
            if (meshAsset == null)
            {
                Debug.LogError($"Saved mesh assest not found: {g.path}");
            }
            g.ApplyMesh(meshAsset!);
        }

        SavePrefab(targetPrefabDir);
    }

    [DebuggerHidden]
    public void EncryptMeshesAndSave(string targetPrefabDir, string key, float factor = 0.1f)
    {
        targetPrefabDir = Path.Combine(targetPrefabDir, "vrcrypt_encrypted");
        // Generate random meshes
        var allmeshes = GetAllMeshes();
        var xMeshes = new List<XMesh>();
        foreach (var x in allmeshes)
        {
            var m = new List<XMesh>();
            m.Add(x);
            var output = FFI.Meshes.Encrypt(m, key, factor);
            xMeshes.Add(output.meshes[0]);
        }

        var meshDir = Path.Combine(targetPrefabDir, "meshes");

        // replace meshes
        foreach (var g in GetAllChildrenWithMeshes())
        {
            var xMesh = xMeshes.Find(x => x.path == g.path);
            if (xMesh == null)
            {
                Debug.LogError($"Mesh not found for: {g.path}");
            }
            xMesh!.SaveAsset(meshDir);

            var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
            if (meshAsset == null)
            {
                Debug.LogError($"Saved mesh assest not found: {g.path}");
            }
            g.ApplyMesh(meshAsset!);
        }

        SavePrefab(targetPrefabDir);
    }

    [DebuggerHidden]
    public void decryptMeshes(string key, float factor = 0.1f)
    {
        // Generate random meshes
        var allmeshes = GetAllMeshes();
        var xMeshes = new List<XMesh>();
        foreach (var x in allmeshes)
        {
            var m = new List<XMesh>();
            m.Add(x);
            var output = FFI.Meshes.Decrypt(m, key, factor);
            xMeshes.Add(output.meshes[0]);
        }

        // replace meshes
        foreach (var g in GetAllChildrenWithMeshes())
        {
            var xMesh = xMeshes.Find(x => x.path == g.path);
            if (xMesh == null)
            {
                Debug.LogError($"Mesh not found for: {g.path}");
            }
            g.ApplyMesh(xMesh!.ToMesh());
        }
    }
}
