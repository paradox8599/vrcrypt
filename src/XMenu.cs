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

        if (GUILayout.Button("ffi randomize"))
        {
            // var input = new FFI.CreateRandomMeshesInput(avatar.GetAllMeshes(), 0.01f);
            // var output = FFI.CreateRandomMeshes(input);
            // var meshes = output.meshes;
            var meshes = avatar
                .GetAllMeshes()
                .ConvertAll(x => JsonUtility.ToJson(x))
                .ConvertAll(x => JsonUtility.FromJson<XMesh>(x));
            var cloned = avatar.InMemoryClone();

            foreach (var g in cloned.GetAllChildrenWithMeshes())
            {
                var xMesh = meshes.Find(x => x.path == g.path);
                if (xMesh == null)
                {
                    Debug.LogError($"Mesh not found for: {g.path}");
                }
                xMesh!.SaveAsset();
                var asset = xMesh!.LoadAsset();
                if (asset == null)
                {
                    Debug.LogError($"Saved mesh assest not found: {g.path}");
                }
                g.ApplyMesh(asset!);
            }
            // cloned.SavePrefab(avatar.prefabDir);
        }

        if (GUILayout.Button("compare"))
        {
            var x = new XGameObject(avatar.obj.transform.Find("Body").gameObject).xMesh;
            var j1 = JsonUtility.ToJson(x);
            var x2 = JsonUtility.FromJson<XMesh>(j1);
            var m = x2.ToMesh();
            var x3 = new XMesh(m);
            var j2 = JsonUtility.ToJson(x3);
            Debug.Log(j1 == j2);
        }

        // ReadPathsButton();
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
}
