using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
internal class XAvatar
{
    internal string json;
    internal string name;

    internal XAvatar(Avatar avatar)
    {
        json = EditorJsonUtility.ToJson(avatar, false);
        name = avatar.name;
    }

    internal XAvatar(Animator animator)
    {
        json = EditorJsonUtility.ToJson(animator.avatar, false);
        name = animator.avatar.name;
    }

    internal static XAvatar? FromGameObject(GameObject gameObject)
    {
        var animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            return null;
        }
        return new XAvatar(animator.avatar);
    }

    internal void Restore(Avatar avatar)
    {
        EditorJsonUtility.FromJsonOverwrite(json, avatar);
        avatar.name = name;
    }

    internal static void Save(Avatar avatar, string savePath)
    {
        if (!savePath.EndsWith(".asset"))
        {
            savePath += ".asset";
        }

        string directory = System.IO.Path.GetDirectoryName(savePath);

        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        Debug.Log("Saving Avatar: " + savePath);

        var newAvatar = Object.Instantiate(avatar);
        AssetDatabase.CreateAsset(newAvatar, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    internal static Avatar? Read(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return null;
        }

        var asset = AssetDatabase.LoadAssetAtPath<Avatar>(path);
        return asset;
    }
}
