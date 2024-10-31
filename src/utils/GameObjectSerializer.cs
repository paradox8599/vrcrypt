using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

internal partial class Utils
{
    internal class GameObjectSerializer
    {
        // TODO: make this more complete
        // currently only serialize components for the given game object, not for its children

        [System.Serializable]
        internal class ComponentData
        {
            public string? typeFullName;
            public string? jsonData;
        }

        [System.Serializable]
        internal class GameObjectData
        {
            public string? name;
            public List<ComponentData> components = new List<ComponentData>();
        }

        internal string ToJson(GameObject gameObject)
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

        internal GameObject FromJson(string json)
        {
            GameObjectData data = JsonUtility.FromJson<GameObjectData>(json);
            GameObject newGameObject = new GameObject(data.name);
            foreach (var componentData in data.components)
            {
                System.Type componentType = System.Type.GetType(componentData.typeFullName);
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
}
