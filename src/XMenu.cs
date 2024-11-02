using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        AvatarInput();

        key = GUILayout.TextField(key, 100);

        GUI.enabled = avatar != null;

        // encryption

        if (GUILayout.Button("List Mesh paths"))
        {
            foreach (var xg in avatar!.GetAllChildrenWithMeshes())
            {
                if (xg.mesh != null)
                {
                    Debug.Log($"is fbx: {XMesh.IsFbx(xg.mesh)}");
                }
            }
        }

        GUILayout.BeginHorizontal();
        var factor = 0.01f;
        if (GUILayout.Button("ffi encrypt") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.EncryptMeshesAndSave("Assets/Mamehinata/vrcrypt", key, factor);
            cloned.SavePrefab("Assets/Mamehinata/vrcrypt");
        }

        if (GUILayout.Button("ffi decrypted") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.DecryptAndApply(key, factor);
        }
        GUILayout.EndHorizontal();

        // avatar

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Avatar"))
        {
            var animator = avatar!.obj.GetComponent<Animator>();
            XAvatar.Save(animator.avatar, "Assets/avatar.asset");
        }

        if (GUILayout.Button("restore Avatar"))
        {
            var ava = XAvatar.Read("Assets/avatar.asset");
            if (ava == null)
            {
                Debug.Log("avatar is null");
            }
            else
            {
                var animator = avatar!.obj.GetComponent<Animator>();
                animator.avatar = ava;
            }
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
