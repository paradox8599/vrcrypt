using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class XMenu : EditorWindow
{
    private Dictionary<string, XMesh>? meshes = null;
    private GameObject? avatar = null;
    private Vector2 scrollPosition = Vector2.zero;

    [DebuggerHidden]
    [MenuItem("VRCrypt/Show")]
    private static void ShowMenu()
    {
        var w = EditorWindow.GetWindow(typeof(XMenu));
        w.titleContent = new GUIContent("VRCrypt");
    }

    // On GUI

    void OnGUI()
    {
        GUILayout.Label("Activate", EditorStyles.boldLabel);

        // code = EditorGUILayout.TextField("Code", code);
        avatar =
            EditorGUILayout.ObjectField("Avatar Prefab", avatar, typeof(GameObject), true)
            as GameObject;
        if (avatar == null)
        {
            return;
        }

        XGameObject xAvatar = new XGameObject(avatar);
        if (!xAvatar.isPrefab)
        {
            EditorGUILayout.HelpBox(
                "Please select an avatar prefab from Assets.",
                MessageType.Warning
            );
        }

        // Read

        GUI.enabled = xAvatar.isPrefab;
        if (GUILayout.Button("Read") && GUI.enabled)
        {
            meshes = new Dictionary<string, XMesh>();
            foreach (var xMesh in xAvatar.meshes)
            {
                var hash = xMesh.ToHash();
                meshes.Add(hash, xMesh);
                Debug.Log($"Read mesh: {hash}, {xMesh.path}");
            }
        }

        GUI.enabled = meshes != null;

        // Save

        if (GUILayout.Button("Save Meshes") && meshes != null)
        {
            foreach (var xMesh in meshes.Values)
            {
                xMesh.SaveAsset();
            }
        }

        // Write

        if (GUILayout.Button("Write Meshes") && meshes != null)
        {
            // string prefabPath = AssetDatabase.GetAssetPath(avatar);
            // Debug.Log($"Prefab path: {prefabPath}");
            // GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

            // ...

            // PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            // PrefabUtility.UnloadPrefabContents(prefabRoot);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }

        GUI.enabled = true;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginVertical();

        if (meshes != null)
        {
            foreach (var (hash, xMesh) in meshes)
            {
                GUILayout.Label($"{hash}: {xMesh.path}");
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
