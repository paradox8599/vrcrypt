using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DebuggerHidden = System.Diagnostics.DebuggerHiddenAttribute;

public partial class XGameObject
{
  [DebuggerHidden]
  public List<XMesh> EncryptMeshes(string key, float factor = 0.1f)
  {
    // var allmeshes = GetAllMeshes();
    // var xMeshes = new List<XMesh>();
    // foreach (var x in allmeshes)
    // {
    //   var m = new List<XMesh>();
    //   m.Add(x);
    //   var output = FFI.Meshes.Encrypt(m, key, factor);
    //   xMeshes.Add(output.meshes[0]);
    // }

    var xMeshes = FFI.Meshes.Encrypt(GetAllMeshes(), key, factor).meshes;
    return xMeshes;
  }

  [DebuggerHidden]
  public List<XMesh> DecryptMeshes(string key, float factor = 0.1f)
  {
    var allmeshes = GetAllMeshes();
    var xMeshes = new List<XMesh>();
    foreach (var x in allmeshes)
    {
      var m = new List<XMesh>();
      m.Add(x);
      var output = FFI.Meshes.Decrypt(m, key, factor);
      xMeshes.Add(output.meshes[0]);
    }
    return xMeshes;
  }

  [DebuggerHidden]
  public void EncryptMeshesAndSave(string targetPrefabDir, string key, float factor = 0.1f)
  {
    var xMeshes = EncryptMeshes(key, factor);
    var meshDir = Path.Combine(targetPrefabDir, "meshes");

    // replace meshes

    foreach (var g in GetAllChildrenWithMeshes())
    {
      var xMesh = xMeshes.Find(x => x.path == g.path);
      if (xMesh == null)
      {
        Debug.LogError($"Mesh not found for: {g.path}");
      }
      xMesh!.SaveAsset(meshDir);

      var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
      if (meshAsset == null)
      {
        Debug.LogError($"Saved mesh assest not found: {g.path}");
      }
      g.ApplyMesh(meshAsset!);
    }
  }

  [DebuggerHidden]
  public void DecryptAndApply(string key, float factor = 0.1f)
  {
    var xMeshes = DecryptMeshes(key, factor);

    // replace meshes

    foreach (var g in GetAllChildrenWithMeshes())
    {
      var xMesh = xMeshes.Find(x => x.path == g.path);
      if (xMesh == null)
      {
        Debug.LogError($"Mesh not found for: {g.path}");
      }
      g.ApplyMesh(xMesh!.ToMesh());
    }
  }

  // [DebuggerHidden]
  // public void SaveRandomized(string targetPrefabDir)
  // {
  //   targetPrefabDir = Path.Combine(targetPrefabDir, "vrcrypted");
  //   // Generate random meshes
  //   var allmeshes = GetAllMeshes();
  //   var xMeshes = new List<XMesh>();
  //   foreach (var x in allmeshes)
  //   {
  //     var m = new List<XMesh>();
  //     m.Add(x);
  //     var output = FFI.Meshes.Randomize(m, 0.01f);
  //     xMeshes.Add(output.meshes[0]);
  //   }
  //
  //   var meshDir = Path.Combine(targetPrefabDir, "meshes");
  //
  //   // replace meshes
  //   foreach (var g in GetAllChildrenWithMeshes())
  //   {
  //     var xMesh = xMeshes.Find(x => x.path == g.path);
  //     if (xMesh == null)
  //     {
  //       Debug.LogError($"Mesh not found for: {g.path}");
  //     }
  //     xMesh!.SaveAsset(meshDir);
  //
  //     var meshAsset = XMesh.LoadAsset(Path.Combine(meshDir, $"{xMesh.hash}.asset"));
  //     if (meshAsset == null)
  //     {
  //       Debug.LogError($"Saved mesh assest not found: {g.path}");
  //     }
  //     g.ApplyMesh(meshAsset!);
  //   }
  //
  //   SavePrefab(targetPrefabDir);
  // }
}
