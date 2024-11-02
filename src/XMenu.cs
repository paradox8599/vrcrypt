using System;
using System.Collections.Generic;
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
        GUILayout.Label("VRCrypt", EditorStyles.boldLabel);

        AvatarInput();

        key = GUILayout.TextField(key, 100);

        GUI.enabled = avatar != null;

        GUILayout.BeginHorizontal();
        var factor = 0.01f;
        if (GUILayout.Button("ffi encrypt") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.EncryptMeshesAndSave(avatar.prefabDir, key, factor);
        }

        if (GUILayout.Button("ffi decrypted") && avatar != null)
        {
            var cloned = avatar.InMemoryClone();
            cloned.decryptMeshes(key, factor);
        }
        GUILayout.EndHorizontal();

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

    [Serializable]
    public class ComponentData
    {
        public string? typeFullName;
        public string? jsonData;
    }

    [Serializable]
    public class GameObjectData
    {
        public string? name;
        public List<ComponentData> components = new List<ComponentData>();
    }

    public string SerializeToFile(GameObject gameObject)
    {
        GameObjectData data = new GameObjectData();
        data.name = gameObject.name;

        Component[] components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            ComponentData componentData = new ComponentData
            {
                typeFullName = component.GetType().AssemblyQualifiedName,
                jsonData = EditorJsonUtility.ToJson(component),
            };
            data.components.Add(componentData);
        }
        string jsonString = JsonUtility.ToJson(data, false); // true for pretty print
        return jsonString;
    }

    public GameObject Deserialize(string json)
    {
        GameObjectData data = JsonUtility.FromJson<GameObjectData>(json);
        GameObject newGameObject = new GameObject(data.name);
        foreach (var componentData in data.components)
        {
            Type componentType = Type.GetType(componentData.typeFullName);
            if (componentType == null)
            {
                Debug.LogError($"Could not find type: {componentData.typeFullName}");
                continue;
            }
            Component newComponent = newGameObject.AddComponent(componentType);
            EditorJsonUtility.FromJsonOverwrite(componentData.jsonData, newComponent);
        }
        return newGameObject;
    }
}
