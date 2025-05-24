using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class XGameObject
{
    [DebuggerHidden]
    public static bool IsPrefab(GameObject obj) =>
        PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;

    public GameObject obj;
    public bool isPrefab => XGameObject.IsPrefab(this.obj);
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

    [DebuggerHidden]
    public XGameObject? SavePrefab(string dir)
    {
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
}
