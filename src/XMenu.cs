using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class XMenu : EditorWindow
{
    private Dictionary<string, XMesh>? meshes = null;
    private XGameObject? avatar = null;
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
        GUILayout.Label("VRCrypt", EditorStyles.boldLabel);

        AvatarInput();
        if (avatar == null)
            return;

        ReadPathsButton();
        PathList();
    }

    void AvatarInput()
    {
        var ago =
            EditorGUILayout.ObjectField(
                "Avatar Prefab to Encrypt",
                avatar?.obj,
                typeof(GameObject),
                true
            ) as GameObject;
        avatar = XGameObject.New(ago);

        if (this.avatar == null)
        {
            return;
        }

        if (!avatar.isPrefab)
        {
            EditorGUILayout.HelpBox(
                "Please select an avatar prefab from Assets.",
                MessageType.Warning
            );
        }
    }

    void ReadPathsButton()
    {
        if (avatar == null)
            return;
        GUI.enabled = avatar.isPrefab;
        if (GUILayout.Button("Read Paths"))
        {
            Debug.Log("paths");
            foreach (var xgo in avatar.GetAllChildrenWithMeshes())
            {
                Debug.Log(xgo.path);
            }
            // XMesh? m = null;
            // meshes = new Dictionary<string, XMesh>();
            // foreach (var x in xAvatar.GetChildMeshes())
            // {
            //     var hash = x.ToHash();
            //     x.SaveAsset();
            //     meshes.Add(hash, x);
            //     Debug.Log($"Read mesh: {hash}, {x.path}");
            //     m ??= x;
            // }
            // var json = m?.ToJson();
            // Debug.Log($"input: {json}");
            // var output = FFI.read(json!);
            // Debug.Log($"output: {output}");
        }
    }

    void PathList()
    {
        if (avatar == null)
            return;

        GUI.enabled = true;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginVertical();

        foreach (XGameObject go in avatar.GetAllChildrenWithMeshes())
        {
            GUILayout.Label($"{go.obj.name}: {go.path}");
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
