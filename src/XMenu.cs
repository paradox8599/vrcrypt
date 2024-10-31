using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class XMenu : EditorWindow
{
    private XGameObject? avatar = null;
    private string key = "";

    [DebuggerHidden]
    [MenuItem("VRCrypt/Show")]
    private static void ShowMenu()
    {
        var w = EditorWindow.GetWindow(typeof(XMenu));
        w.titleContent = new GUIContent("VRCrypt");
    }

    // On GUI

    [DebuggerHidden]
    void OnGUI()
    {
        var factor = 0.01f;
        GUILayout.Label("VRCrypt", EditorStyles.boldLabel);

        AvatarInput();

        key = GUILayout.TextField(key, 100);

        GUI.enabled = avatar != null;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("ffi encrypt") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.SaveEncrypted(avatar.prefabDir, key, factor);
        }

        if (GUILayout.Button("ffi decrypted") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.decrypt(key, factor);
        }
        GUILayout.EndHorizontal();
    }

    /// Avatar Input Box

    [DebuggerHidden]
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
