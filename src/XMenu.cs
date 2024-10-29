using System.Diagnostics;
using UnityEditor;
using UnityEngine;

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

        GUI.enabled = avatar != null;
        if (GUILayout.Button("ffi randomize") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.SaveRandomized(avatar.prefabDir);
        }
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
}
