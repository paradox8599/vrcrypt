using System;
using System.IO;
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

    internal static Avatar? FromGameObject(GameObject gameObject)
    {
        var animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            return null;
        }
        return animator.avatar;
    }

    internal void Restore(Avatar avatar)
    {
        EditorJsonUtility.FromJsonOverwrite(json, avatar);
        avatar.name = name;
    }

    internal static Avatar Save(Avatar avatar, string savePath)
    {
        if (!System.IO.Directory.Exists(savePath))
        {
            System.IO.Directory.CreateDirectory(savePath);
        }

        Debug.Log("Saving Avatar: " + savePath);

        var newAvatar = Object.Instantiate(avatar);
        AssetDatabase.CreateAsset(newAvatar, Path.Combine(savePath, $"{avatar.name}.asset"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return newAvatar;
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
