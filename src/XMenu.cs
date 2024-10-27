using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class XMenu : EditorWindow
{
    private XGameObject? avatar = null;

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

    /// Avatar Input Box

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

    /// Read Paths of all children with meshes of the avatar

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
        }
    }

    /// List paths of of all children with meshes

    private Vector2 scrollPosition = Vector2.zero;

    void PathList()
    {
        if (avatar == null)
            return;

        GUI.enabled = true;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginVertical();

        foreach (XGameObject go in avatar.GetAllChildrenWithMeshes())
        {
            GUILayout.Label(go.path);
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
