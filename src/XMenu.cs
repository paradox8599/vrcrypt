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
    private float factor = 0.01f;

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
        GUILayout.Label("VRCrypt", EditorStyles.boldLabel);

        factor = GUILayout.HorizontalSlider(factor * 100, 0, 1) * 0.01f;

        GUILayout.Label("Avatar", EditorStyles.boldLabel);

        if (GUILayout.Button("FFI"))
        {
            var str = FFI.read("hello");
            Debug.Log(str);
        }

        // code = EditorGUILayout.TextField("Code", code);
        avatar =
            EditorGUILayout.ObjectField(
                "Avatar Prefab to Encrypt",
                avatar,
                typeof(GameObject),
                true
            ) as GameObject;

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
        if (GUILayout.Button("Read Meshes") && xAvatar.isPrefab)
        {
            meshes = new Dictionary<string, XMesh>();
            foreach (var xMesh in xAvatar.GetChildMeshes())
            {
                var x = xMesh.ToRandomized(factor);
                var hash = x.ToHash();
                x.path = xMesh.path;
                x.SaveAsset();
                meshes.Add(hash, x);
                Debug.Log($"Read mesh: {hash}, {x.path}");
            }
        }

        GUI.enabled = meshes != null;

        // Write

        if (GUILayout.Button("Write") && meshes != null)
        {
            var a = xAvatar.SavePrefab("vrcrypted", xAvatar.prefabDir)!;
            foreach (var (hash, xMesh) in meshes!)
            {
                Debug.Log($"{hash}, {xMesh.path}");
                var m = xMesh.LoadAsset()!;
                Debug.Log($"Getting child: {xMesh.path}");
                var go = a.GetChildAtPath(xMesh.pathSplit)!;
                Debug.Log($"Child got: {go.obj.name}");
                go.ApplyMesh(m);
                Debug.Log($"Applied mesh: {hash}");
            }
        }

        GUI.enabled = true;

        // List

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
