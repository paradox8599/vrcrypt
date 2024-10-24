using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class XMenu : EditorWindow
{
    private static Dictionary<string, global::XMesh>? meshes = null;

    private string code = "";
    private GameObject? avatar = null;

    private static void ReadRecursive(GameObject root, string prefix = "")
    {
        foreach (Transform child in root.transform)
        {
            var childGo = child.gameObject;
            childGo.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer? smr);
            var path = $"{prefix}/{childGo.name}";
            if (smr != null)
            {
                Debug.Log($"Adding: {path}, {smr.name}");
                var mesh = global::XMesh.FromMesh(smr.sharedMesh);
                mesh.path = path;
                meshes?.Add(mesh.ToHash(), mesh);
            }
            ReadRecursive(childGo, path);
        }
    }

    [DebuggerHidden]
    [MenuItem("Para/Show")]
    private static void ShowMenu()
    {
        var w = EditorWindow.GetWindow(typeof(XMenu));
        w.titleContent = new GUIContent("Para");
    }

    // On GUI

    void OnGUI()
    {
        GUILayout.Label("Activate", EditorStyles.boldLabel);

        code = EditorGUILayout.TextField("Code", code);
        avatar =
            EditorGUILayout.ObjectField("Avatar Prefab", avatar, typeof(GameObject), true)
            as GameObject;

        if (avatar != null && !IsSelectedAvatarPrefab())
        {
            EditorGUILayout.HelpBox(
                "Please select an avatar prefab from Assets.",
                MessageType.Warning
            );
        }

        // Read

        GUI.enabled = IsSelectedAvatarPrefab();
        if (GUILayout.Button("Read") && IsSelectedAvatarPrefab())
        {
            Debug.Log("Clearing Previous meshes");
            meshes = new Dictionary<string, global::XMesh>();
            ReadRecursive(GameObject.Find("Mamehinata_PC"));
        }

        // List Serializable Meshes and their paths

        GUI.enabled = meshes != null;
        if (GUILayout.Button("List Meshes") && meshes != null)
        {
            Debug.Log("Listing Meshes");
            foreach (var (hash, mesh) in meshes)
            {
                Debug.Log($"{hash}: {mesh.path}");
            }
        }
        GUI.enabled = true;
    }

    bool IsSelectedAvatarPrefab()
    {
        return avatar != null && PrefabUtility.IsPartOfPrefabAsset(avatar);
    }
}
