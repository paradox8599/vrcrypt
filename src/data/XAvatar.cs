using System;
using UnityEditor;
using UnityEngine;

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
}
