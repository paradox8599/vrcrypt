using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class XMenu : EditorWindow
{
    private XGameObject? avatar = null;
    private string key = "";

    // Menu
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
        AvatarInput();

        key = GUILayout.TextField(key, 32);

        GUI.enabled = avatar != null;

        if (GUILayout.Button("Encrypt"))
        {
            string exportPath = "Assets/vrcrypted";
            var cloned = avatar!.InMemoryClone();
            var encryptedMeshes = FFI.Meshes.Encrypt(cloned.GetAllMeshes(), key, 0.01f).meshes;

            // save avatar as asset
            var animator = cloned.obj.GetComponent<Animator>();
            var savedAvatar = XAvatar.Save(animator.avatar, exportPath);
            animator.avatar = savedAvatar;

            // save meshes as asset
            var meshPath = Path.Combine(exportPath, "meshes");

            // assign encrypted meshes to clone
            foreach (var xgo in cloned.GetAllChildrenWithMeshes())
            {
                var xMesh = encryptedMeshes.Find(x => x.path == xgo.path);
                var mesh = xMesh.SaveAsset(meshPath);
                xgo.ApplyMesh(mesh);
            }

            cloned.SavePrefab(exportPath);
        }
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
